#ifndef PLCLR_EXEC_H
#define PLCLR_EXEC_H

extern Datum plclr_exec_function(PlClr_function *func, FunctionCallInfo fcinfo, bool atomic);
extern HeapTuple plclr_exec_trigger(PlClr_function *func, TriggerData *trigdata);
extern void plclr_exec_event_trigger(PlClr_function *func, EventTriggerData *trigdata);

#endif // PLCLR_EXEC_H