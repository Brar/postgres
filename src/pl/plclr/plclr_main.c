/*
 * PL/CLR main entry points
 *
 * src/pl/plclr/plclr_main.c
 */

#include "postgres.h"
#include "catalog/pg_proc.h"
#include "catalog/pg_type.h"
#include "commands/trigger.h"
#include "commands/event_trigger.h"
#include "fmgr.h"
#include "utils/builtins.h"
#include "utils/syscache.h"
#include "plclr_runtime_host.h"

/*
 * exported functions
 */

extern void _PG_init(void);

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
            compileInfo.FunctionName = NameStr(procStruct->proname);

            prosrcdatum = SysCacheGetAttr(PROCOID, procTup, Anum_pg_proc_prosrc, &isnull);
            if (isnull)
                elog(ERROR, "null prosrc");
            compileInfo.FunctionBody = TextDatumGetCString(prosrcdatum);

            retval = (Datum)compile_and_execute((FunctionCompileInfoPtr)&compileInfo);
         }
    }
    PG_CATCH();
    {
        PG_RE_THROW();
    }
    PG_END_TRY();

    return retval;
}
