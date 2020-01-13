#ifndef PLCLR_MANAGED_H
#define PLCLR_MANAGED_H
#include "plclr.h"

typedef struct PlClr_FunctionCompileInfo
{
    Oid FunctionOid;
    const clr_char* FunctionName;
    const clr_char* FunctionBody;
	Oid ReturnValueType;
	bool ReturnsSet;
	int NumberOfArguments;
	void* ArgumentTypes;
	void* ArgumentNames;
	void* ArgumentModes;
} PlClr_FunctionCompileInfo, *PlClr_FunctionCompileInfoPtr;

typedef struct PlClr_FunctionCallInfo
{
	void* ExecuteDelegatePtr;
	int NumberOfArguments;
	void* ArgumentValues;
} PlClr_FunctionCallInfo, *PlClr_FunctionCallInfoPtr;

void* plclr_compile_managed(PlClr_FunctionCompileInfoPtr compileInfo);

#endif // PLCLR_MANAGED_H
