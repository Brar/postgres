#ifndef PLCLR_COMP_H
#define PLCLR_COMP_H
#include "plclr.h"

PlClr_function *
plclr_compile_function(FunctionCallInfo fcinfo, HeapTuple procTup, PlClr_function* function, bool forValidator);

#endif // PLCLR_COMP_H
