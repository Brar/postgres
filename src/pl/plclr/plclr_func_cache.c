/*-------------------------------------------------------------------------
 *
 * plclr_func_cache.c		- Function caching part of the PL/CLR
 *			  procedural language
 *
 * Portions Copyright (c) 1996-2020, PostgreSQL Global Development Group
 * Portions Copyright (c) 1994, Regents of the University of California
 *
 *
 * IDENTIFICATION
 *	  src/pl/plclr/src/plclr_func_cache.c
 *
 *-------------------------------------------------------------------------
 */


/*
 * This file is a copy of the caching part of src/pl/plpgsql/src/pl_comp.c with mere renaming of structs and functions
 * but literally no own code.
 * It might be a good idea to flesh out an interface for this to make it available for other PL implementations
 */
#include "postgres.h"

#include "access/htup_details.h"
#include "catalog/pg_proc.h"
#include "catalog/pg_type.h"
#include "commands/trigger.h"
#include "funcapi.h"
#include "utils/hsearch.h"
#include "utils/syscache.h"

#include "plclr_func_cache.h"

/* ----------
 * Hash table for compiled functions
 * ----------
 */
static HTAB *plclr_HashTable = NULL;


typedef struct plclr_hashent
{
	PlClr_func_hashkey key;
	PlClr_function *function;
} plclr_HashEnt;

#define FUNCS_PER_USER		128 /* initial table size */

/* ----------
 * static prototypes
 * ----------
 */
static void compute_function_hashkey(FunctionCallInfo fcinfo,
									 Form_pg_proc procStruct,
									 PlClr_func_hashkey *hashkey,
									 bool forValidator);
static void plclr_resolve_polymorphic_argtypes(int numargs,
												 Oid *argtypes, char *argmodes,
												 Node *call_expr, bool forValidator,
												 const char *proname);
static PlClr_function *plclr_HashTableLookup(PlClr_func_hashkey *func_key);
static void delete_function(PlClr_function *func);
static void plclr_HashTableDelete(PlClr_function *function);
static void plclr_free_function_memory(PlClr_function *func);

/* ----------
 * plclr_get_func_from_cache	Try to get an already compiled PL/CLR function from our internal hash table
 *
 * If forValidator is true, we're only compiling for validation purposes,
 * and so some checks are skipped.
 *
 * Note: it's important for this to fall through quickly if the function
 * has already been compiled.
 * ----------
 */
PlClr_function *
plclr_get_func_from_cache(FunctionCallInfo fcinfo, HeapTuple procTup, bool forValidator, bool* isValid)
{
	Form_pg_proc procStruct;
	PlClr_function *function;
	PlClr_func_hashkey hashkey;
	bool		hashkey_valid = false;

	procStruct = (Form_pg_proc) GETSTRUCT(procTup);

	/*
	 * See if there's already a cache entry for the current FmgrInfo. If not,
	 * try to find one in the hash table.
	 */
	function = (PlClr_function *) fcinfo->flinfo->fn_extra;

recheck:
	*isValid = false;
	if (!function)
	{
		/* Compute hashkey using function signature and actual arg types */
		compute_function_hashkey(fcinfo, procStruct, &hashkey, forValidator);
		hashkey_valid = true;

		/* And do the lookup */
		function = plclr_HashTableLookup(&hashkey);
	}

	if (function)
	{
		/* We have a compiled function, but is it still valid? */
		if (function->fn_xmin != HeapTupleHeaderGetRawXmin(procTup->t_data) ||
			!ItemPointerEquals(&function->fn_tid, &procTup->t_self))
		{
			/*
			 * Nope, so remove it from hashtable and try to drop associated
			 * storage (if not done already).
			 */
			delete_function(function);

			/*
			 * If the function isn't in active use then we can overwrite the
			 * func struct with new data, allowing any other existing fn_extra
			 * pointers to make use of the new definition on their next use.
			 * If it is in use then just leave it alone and make a new one.
			 * (The active invocations will run to completion using the
			 * previous definition, and then the cache entry will just be
			 * leaked; doesn't seem worth adding code to clean it up, given
			 * what a corner case this is.)
			 *
			 * If we found the function struct via fn_extra then it's possible
			 * a replacement has already been made, so go back and recheck the
			 * hashtable.
			 */
			if (function->use_count != 0)
			{
				function = NULL;
				if (!hashkey_valid)
					goto recheck;
			}
			function = NULL;
		}
		else
			*isValid = true;
	}

	/*
	 * Finally return the cached function or NULL
	 */
	return function;
}

/*
 * Compute the hashkey for a given function invocation
 *
 * The hashkey is returned into the caller-provided storage at *hashkey.
 */
static void
compute_function_hashkey(FunctionCallInfo fcinfo,
						 Form_pg_proc procStruct,
						 PlClr_func_hashkey *hashkey,
						 bool forValidator)
{
	/* Make sure any unused bytes of the struct are zero */
	MemSet(hashkey, 0, sizeof(PlClr_func_hashkey));

	/* get function OID */
	hashkey->funcOid = fcinfo->flinfo->fn_oid;

	/* get call context */
	hashkey->isTrigger = CALLED_AS_TRIGGER(fcinfo);

	/*
	 * if trigger, get its OID.  In validation mode we do not know what
	 * relation or transition table names are intended to be used, so we leave
	 * trigOid zero; the hash entry built in this case will never really be
	 * used.
	 */
	if (hashkey->isTrigger && !forValidator)
	{
		TriggerData *trigdata = (TriggerData *) fcinfo->context;

		hashkey->trigOid = trigdata->tg_trigger->tgoid;
	}

	/* get input collation, if known */
	hashkey->inputCollation = fcinfo->fncollation;

	if (procStruct->pronargs > 0)
	{
		/* get the argument types */
		memcpy(hashkey->argtypes, procStruct->proargtypes.values,
			   procStruct->pronargs * sizeof(Oid));

		/* resolve any polymorphic argument types */
		plclr_resolve_polymorphic_argtypes(procStruct->pronargs,
											 hashkey->argtypes,
											 NULL,
											 fcinfo->flinfo->fn_expr,
											 forValidator,
											 NameStr(procStruct->proname));
	}
}

/*
 * This is the same as the standard resolve_polymorphic_argtypes() function,
 * but with a special case for validation: assume that polymorphic arguments
 * are integer, integer-array or integer-range.  Also, we go ahead and report
 * the error if we can't resolve the types.
 */
static void
plclr_resolve_polymorphic_argtypes(int numargs,
									 Oid *argtypes, char *argmodes,
									 Node *call_expr, bool forValidator,
									 const char *proname)
{
	int			i;

	if (!forValidator)
	{
		/* normal case, pass to standard routine */
		if (!resolve_polymorphic_argtypes(numargs, argtypes, argmodes,
										  call_expr))
			ereport(ERROR,
					(errcode(ERRCODE_FEATURE_NOT_SUPPORTED),
					 errmsg("could not determine actual argument "
							"type for polymorphic function \"%s\"",
							proname)));
	}
	else
	{
		/* special validation case */
		for (i = 0; i < numargs; i++)
		{
			switch (argtypes[i])
			{
				case ANYELEMENTOID:
				case ANYNONARRAYOID:
				case ANYENUMOID:	/* XXX dubious */
					argtypes[i] = INT4OID;
					break;
				case ANYARRAYOID:
					argtypes[i] = INT4ARRAYOID;
					break;
				case ANYRANGEOID:
					argtypes[i] = INT4RANGEOID;
					break;
				default:
					break;
			}
		}
	}
}

/*
 * delete_function - clean up as much as possible of a stale function cache
 *
 * We can't release the PlClr_function struct itself, because of the
 * possibility that there are fn_extra pointers to it.  We can release
 * the subsidiary storage, but only if there are no active evaluations
 * in progress.  Otherwise we'll just leak that storage.  Since the
 * case would only occur if a pg_proc update is detected during a nested
 * recursive call on the function, a leak seems acceptable.
 *
 * Note that this can be called more than once if there are multiple fn_extra
 * pointers to the same function cache.  Hence be careful not to do things
 * twice.
 */
static void
delete_function(PlClr_function *func)
{
	/* remove function from hash table (might be done already) */
	plclr_HashTableDelete(func);

	/* release the function's storage if safe and not done already */
	if (func->use_count == 0)
		plclr_free_function_memory(func);
}

void
plclr_HashTableInit(void)
{
	HASHCTL		ctl;

	/* don't allow double-initialization */
	Assert(plpgsql_HashTable == NULL);

	memset(&ctl, 0, sizeof(ctl));
	ctl.keysize = sizeof(PlClr_func_hashkey);
	ctl.entrysize = sizeof(plclr_HashEnt);
	plclr_HashTable = hash_create("PL/CLR function hash",
									FUNCS_PER_USER,
									&ctl,
									HASH_ELEM | HASH_BLOBS);
}

static PlClr_function *
plclr_HashTableLookup(PlClr_func_hashkey *func_key)
{
	plclr_HashEnt *hentry;

	hentry = (plclr_HashEnt *) hash_search(plclr_HashTable,
											 (void *) func_key,
											 HASH_FIND,
											 NULL);
	if (hentry)
		return hentry->function;
	else
		return NULL;
}

static void
plclr_HashTableDelete(PlClr_function *function)
{
	plclr_HashEnt *hentry;

	/* do nothing if not in table */
	if (function->fn_hashkey == NULL)
		return;

	hentry = (plclr_HashEnt *) hash_search(plclr_HashTable,
											 (void *) function->fn_hashkey,
											 HASH_REMOVE,
											 NULL);
	if (hentry == NULL)
		elog(WARNING, "trying to delete function that does not exist");

	/* remove back link, which no longer points to allocated storage */
	function->fn_hashkey = NULL;
}


static void
plclr_free_function_memory(PlClr_function *func)
{
	/* Better not call this on an in-use function */
	Assert(func->use_count == 0);

	/*
	 * And finally, release all memory except the PLpgSQL_function struct
	 * itself (which has to be kept around because there may be multiple
	 * fn_extra pointers to it).
	 */
	if (func->fn_cxt)
		MemoryContextDelete(func->fn_cxt);
	func->fn_cxt = NULL;
}
