/*
 * PL/CLR main entry points
 *
 * src/pl/plclr/plclr_main.c
 */

#include "postgres.h"

#include <access/xact.h>
#include "catalog/pg_proc.h"
#include "catalog/pg_type.h"
#include "commands/event_trigger.h"
#include "commands/trigger.h"
#include "fmgr.h"
#include "funcapi.h"
#include "mb/pg_wchar.h"
#include "utils/builtins.h"
#include "utils/syscache.h"

#include "plclr_runtime_host.h"

/*
 * exported functions
 */

extern void _PG_init(void);

Datum plclr_call_handler(PG_FUNCTION_ARGS);

static clr_char* server_encoding_to_clr_char(const char *input);


PG_MODULE_MAGIC;

PG_FUNCTION_INFO_V1(plclr_call_handler);

void
_PG_init(void)
{
    static bool inited = false;

    if (inited)
        return;

    plclr_runtime_host_init();

    inited = true;
}

Datum
plclr_call_handler(PG_FUNCTION_ARGS)
{
    Datum retval;
    HeapTuple procTup;
    Form_pg_proc procStruct;
    Datum prosrcdatum;
    bool isnull;
    FunctionCompileInfo compileInfo;
	void* functionHandle;
	int			numargs;
	Oid*		argtypes;
	char**		argnames;
	char*		argmodes;
	Sleep(30000);
    PG_TRY();
    {
        if (CALLED_AS_TRIGGER(fcinfo))
            retval = (Datum) 0;
        else if (CALLED_AS_EVENT_TRIGGER(fcinfo))
        {
            retval = (Datum) 0;
        }
        else
        {
            compileInfo.FunctionOid = fcinfo->flinfo->fn_oid;

            procTup = SearchSysCache1(PROCOID, ObjectIdGetDatum(compileInfo.FunctionOid));
            if (!HeapTupleIsValid(procTup))
                elog(ERROR, "cache lookup failed for function %u", compileInfo.FunctionOid);
            procStruct = (Form_pg_proc) GETSTRUCT(procTup);
        	compileInfo.ReturnValueType = procStruct->prorettype;
        	compileInfo.ReturnsSet = procStruct->proretset;
            compileInfo.FunctionName = server_encoding_to_clr_char(NameStr(procStruct->proname));

            prosrcdatum = SysCacheGetAttr(PROCOID, procTup, Anum_pg_proc_prosrc, &isnull);
            if (isnull)
                elog(ERROR, "null prosrc");
            compileInfo.FunctionBody = server_encoding_to_clr_char(TextDatumGetCString(prosrcdatum));

        	numargs = get_func_arg_info(procTup, &argtypes, &argnames, &argmodes);

        	if(numargs > 0)
        	{
	            compileInfo.NumberOfArguments = numargs;
        		compileInfo.ArgumentTypes = argtypes;

	            if (argnames == NULL)
		            compileInfo.ArgumentNames = NULL;
	            else
	            {
		            clr_char** unicode_argnames = palloc(numargs * sizeof(clr_char*));
		            for (int i = numargs - 1; i >= 0; i--)
		            {
			            unicode_argnames[i] = server_encoding_to_clr_char(argnames[i]);
		            }
		            compileInfo.ArgumentNames = unicode_argnames;
	            }

	            if (argmodes == NULL)
        			compileInfo.ArgumentModes = NULL;
				else
        			compileInfo.ArgumentModes = argmodes;
        	}
			else
			{
	            compileInfo.NumberOfArguments = 0;
        		compileInfo.ArgumentTypes = NULL;
		        compileInfo.ArgumentNames = NULL;
        		compileInfo.ArgumentModes = NULL;
			}

        	functionHandle = plclr_compile((FunctionCompileInfoPtr)&compileInfo);
            retval = (Datum)0;

			ReleaseSysCache(procTup);
         }
    }
    PG_CATCH();
    {
        PG_RE_THROW();
    }
    PG_END_TRY();

    return retval;
}

static clr_char*
server_encoding_to_clr_char(const char *input)
{
	int			db_encoding = GetDatabaseEncoding();
	size_t		input_length = strlen(input);
	size_t		output_length = input_length;
	clr_char*	output;

#ifdef WIN32
	unsigned	codepage;
	codepage = pg_enc2name_tbl[db_encoding].codepage;

	if (codepage != 0)
	{
		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
		output_length = MultiByteToWideChar(codepage, 0, input, input_length, output, input_length);
	}
	else if (db_encoding != PG_UTF8)
#else
	if (db_encoding != PG_UTF8)
#endif
	{
		char	   *utf8;

		/*
		 * XXX pg_do_encoding_conversion() requires a transaction.  In the
		 * absence of one, hope for the input to be valid UTF8.
		 */
		if (IsTransactionState())
		{
			utf8 = (char *) pg_do_encoding_conversion((unsigned char *) input,
													  input_length,
													  db_encoding,
													  PG_UTF8);
			if (utf8 != input)
				input_length = strlen(utf8);
		}
		else
			utf8 = (char *) input;

		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
#ifdef WIN32
		output_length = MultiByteToWideChar(CP_UTF8, 0, utf8, input_length, output, input_length);
#else
		memcpy(output, input, input_length);
#endif

		if (utf8 != input)
			pfree(utf8);
	}
	/* If our input is already UTF-8 we have to and copy it to a new
	 * palloc'd output anyways as that's what our caller expects.
	 */
	else
	{
		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
		memcpy(output, input, input_length);
	}

	output[output_length] = (clr_char) 0;

	if (output_length == 0 && input_length > 0)
	{
		pfree(output);
		return NULL;			/* error */
	}

	return output;
}
