/*
 * PL/CLR main entry points
 *
 * src/pl/plclr/plclr_main.c
 */

#include "postgres.h"
#include "commands/trigger.h"
#include "commands/event_trigger.h"
#include "fmgr.h"
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
    Datum        retval;

     PG_TRY();
     {
         if (CALLED_AS_TRIGGER(fcinfo))
             retval = (Datum) 0;
         else if (CALLED_AS_EVENT_TRIGGER(fcinfo))
         {
             retval = (Datum) 0;
         }
         else
             retval = (Datum) 0;
     }
     PG_CATCH();
     {
         PG_RE_THROW();
     }
     PG_END_TRY();

    return retval;
}
