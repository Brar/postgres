#ifndef PLCLR_MANAGED_H
#define PLCLR_MANAGED_H
#include "plclr.h"

typedef struct PlClrFunctionCompileInfo
{
    Oid FunctionOid;
    const clr_char* FunctionName;
    const clr_char* FunctionBody;
	Oid ReturnValueType;
	BOOL ReturnsSet;
	BOOL IsStrict;
	int NumberOfArguments;
	void* ArgumentTypes;
	void* ArgumentNames;
	void* ArgumentModes;
} PlClrFunctionCompileInfo, *PlClrFunctionCompileInfoPtr;

typedef struct PlClrFunctionCallInfo
{
	void* ExecuteDelegatePtr;
	int NumberOfArguments;
	void* ArgumentValues;
} PlClrFunctionCallInfo, *PlClrFunctionCallInfoPtr;

typedef struct PlClrManagedInterface
{
    void* (*CompilePtr)(PlClrFunctionCompileInfoPtr, int);
    void* (*ExecutePtr)(PlClrFunctionCallInfoPtr, int);
} PlClrManagedInterface, *PlClrManagedInterfacePtr;

extern PlClrManagedInterfacePtr plclrManagedInterface;

#endif // PLCLR_MANAGED_H
