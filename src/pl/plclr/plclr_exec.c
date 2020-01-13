#include "postgres.h"
#include "commands/event_trigger.h"

#include "plclr.h"
#include "plclr_exec.h"

Datum
plclr_exec_function(PlClr_function *func, FunctionCallInfo fcinfo, bool atomic)
{
	return (Datum) 0;
}


HeapTuple plclr_exec_trigger(PlClr_function *func, TriggerData *trigdata)
{
	return NULL;
}

void plclr_exec_event_trigger(PlClr_function *func, EventTriggerData *trigdata)
{

}
