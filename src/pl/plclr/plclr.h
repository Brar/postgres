#ifndef PLCLR_H
#define PLCLR_H

/*
 * Include order should be: postgres.h, other postgres headers, plclr.h,
 * other plclr headers.
 */
#ifndef POSTGRES_H
#error postgres.h must be included before plclr.h
#endif

#include "executor/spi.h"
#include "utils/expandedrecord.h"


#ifdef WIN32
    #ifdef _WCHAR_T_DEFINED
        typedef wchar_t clr_char;
    #else
        typedef unsigned short clr_char;
    #endif
    #define STR(s) L ## s
    #define CH(c) L ## c
#else
    typedef char clr_char;
    #define STR(s) s
    #define CH(c) c
#endif

/*
 * Hash lookup key for functions
 */
typedef struct PlClr_func_hashkey
{
	Oid			funcOid;

	bool		isTrigger;		/* true if called as a trigger */

	/* be careful that pad bytes in this struct get zeroed! */

	/*
	 * For a trigger function, the OID of the trigger is part of the hash key
	 * --- we want to compile the trigger function separately for each trigger
	 * it is used with, in case the rowtype or transition table names are
	 * different.  Zero if not called as a trigger.
	 */
	Oid			trigOid;

	/*
	 * We must include the input collation as part of the hash key too,
	 * because we have to generate different plans (with different Param
	 * collations) for different collation settings.
	 */
	Oid			inputCollation;

	/*
	 * We include actual argument types in the hash key to support polymorphic
	 * PLpgSQL functions.  Be careful that extra positions are zeroed!
	 */
	Oid			argtypes[FUNC_MAX_ARGS];
} PlClr_func_hashkey;

/*
 * Trigger type
 */
typedef enum PlClr_trigtype
{
	PLCLR_DML_TRIGGER,
	PLCLR_EVENT_TRIGGER,
	PLCLR_NOT_TRIGGER
} PlClr_trigtype;

/*
 * Complete compiled function
 */
typedef struct PlClr_function
{
	char	   *fn_signature;
	Oid			fn_oid;
	TransactionId fn_xmin;
	ItemPointerData fn_tid;
	PlClr_trigtype fn_is_trigger;
	Oid			fn_input_collation;
	PlClr_func_hashkey *fn_hashkey;	/* back-link to hashtable key */
	MemoryContext fn_cxt;

	Oid			fn_rettype;
	int			fn_rettyplen;
	bool		fn_retbyval;
	bool		fn_retistuple;
	bool		fn_retisdomain;
	bool		fn_retset;
	bool		fn_readonly;
	char		fn_prokind;

	int			fn_nargs;
	int			fn_argvarnos[FUNC_MAX_ARGS];
	int			out_param_varno;
	int			found_varno;
	int			new_varno;
	int			old_varno;

	unsigned long use_count;
} PlClr_function;

#endif // PLCLR_H