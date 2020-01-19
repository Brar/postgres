/*
 * PL/CLR main entry points
 *
 * src/pl/plclr/plclr_main.c
 */

#include "postgres.h"
#include "catalog/pg_proc.h"
#include "commands/event_trigger.h"
#include "utils/syscache.h"

#include "plclr.h"
#include "plclr_comp.h"
#include "plclr_exec.h"
#include "plclr_func_cache.h"
#include "plclr_runtime_host.h"

/*
 * exported functions
 */
extern void _PG_init(void);
Datum plclr_call_handler(PG_FUNCTION_ARGS);

/* ----------
 * static prototypes
 * ----------
 */
static PlClr_function * get_function(FunctionCallInfo fcinfo, bool forValidator);

PG_MODULE_MAGIC;

PG_FUNCTION_INFO_V1(plclr_call_handler);

void
_PG_init(void)
{
	static bool inited = false;

	if (inited)
		return;
	
	plclr_HashTableInit();
	plclr_runtime_host_init();

	inited = true;
}

Datum
plclr_call_handler(PG_FUNCTION_ARGS)
{
	bool		nonatomic;
	PlClr_function *func;
	// ToDo: Figure out what PLpgSQL_execstate is for and if we need something similar
	//PLpgSQL_execstate *save_cur_estate;
	Datum		retval;
	int			rc;

	Sleep(15000);
	nonatomic = fcinfo->context &&
		IsA(fcinfo->context, CallContext) &&
		!castNode(CallContext, fcinfo->context)->atomic;

	/*
	 * Connect to SPI manager
	 */
	if ((rc = SPI_connect_ext(nonatomic ? SPI_OPT_NONATOMIC : 0)) != SPI_OK_CONNECT)
		elog(ERROR, "SPI_connect failed: %s", SPI_result_code_string(rc));

	/* Find or compile the function */
	func = get_function(fcinfo, false);

	///* Must save and restore prior value of cur_estate */
	//save_cur_estate = func->cur_estate;

	/* Mark the function as busy, so it can't be deleted from under us */
	func->use_count++;

	PG_TRY();
	{
		/*
		 * Determine if called as function or trigger and call appropriate
		 * subhandler
		 */
		if (CALLED_AS_TRIGGER(fcinfo))
			retval = PointerGetDatum(plclr_exec_trigger(func,
														  (TriggerData *) fcinfo->context));
		else if (CALLED_AS_EVENT_TRIGGER(fcinfo))
		{
			plclr_exec_event_trigger(func,
									   (EventTriggerData *) fcinfo->context);
			retval = (Datum) 0;
		}
		else
			retval = plclr_exec_function(func, fcinfo, !nonatomic);
	}
	PG_FINALLY();
	{
		/* Decrement use-count, restore cur_estate */
		func->use_count--;
		//func->cur_estate = save_cur_estate;
	}
	PG_END_TRY();

	/*
	 * Disconnect from SPI manager
	 */
	if ((rc = SPI_finish()) != SPI_OK_FINISH)
		elog(ERROR, "SPI_finish failed: %s", SPI_result_code_string(rc));

	return retval;
}

static PlClr_function *
get_function(FunctionCallInfo fcinfo, bool forValidator)
{
	Oid			funcOid = fcinfo->flinfo->fn_oid;
	HeapTuple	procTup;
	PlClr_function* function;
	bool isValid;

	procTup = SearchSysCache1(PROCOID, ObjectIdGetDatum(funcOid));
	if (!HeapTupleIsValid(procTup))
		elog(ERROR, "cache lookup failed for function %u", funcOid);

	function = plclr_get_func_from_cache(fcinfo, procTup, forValidator, &isValid);

	if (!isValid || function == NULL)
	{
		function = plclr_compile_function(fcinfo, procTup, function, forValidator);
	}

	ReleaseSysCache(procTup);

	fcinfo->flinfo->fn_extra = (void *) function;

	return function;
}
