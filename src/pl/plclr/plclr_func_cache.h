#ifndef PLCLR_FUNC_CACHE_H
#define PLCLR_FUNC_CACHE_H
#include "plclr.h"

extern PlClr_function* plclr_get_func_from_cache(FunctionCallInfo fcinfo, HeapTuple procTup, bool forValidator, bool* isValid);
extern void plclr_HashTableInit(void);

#endif // PLCLR_FUNC_CACHE_H
