#include "postgres.h"
#include "commands/event_trigger.h"

#include "plclr.h"
#include "plclr_exec.h"
#include "plclr_managed.h"

Datum
plclr_exec_function(PlClr_function *func, FunctionCallInfo fcinfo, bool atomic)
{
	PlClrFunctionCallInfo callInfo;
	callInfo.NumberOfArguments = fcinfo->nargs;
	callInfo.ArgumentValues = fcinfo->args;
	callInfo.ExecuteDelegatePtr = func->action;
	
	return (Datum)plclrManagedInterface->ExecutePtr(&callInfo, sizeof(PlClrFunctionCallInfo));
}


HeapTuple plclr_exec_trigger(PlClr_function *func, TriggerData *trigdata)
{
	return NULL;
}

void plclr_exec_event_trigger(PlClr_function *func, EventTriggerData *trigdata)
{

}
