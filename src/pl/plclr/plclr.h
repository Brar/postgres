#ifndef PLCLR_H
#define PLCLR_H

/*
 * Include order should be: postgres.h, other postgres headers, plclr.h,
 * other plclr headers.
 */
#ifndef POSTGRES_H
#error postgres.h must be included before plclr.h
#endif

#ifdef WIN32
    #ifdef _WCHAR_T_DEFINED
        typedef wchar_t clr_char;
    #else
        typedef unsigned short clr_char;
    #endif
    #define STR(s) L ## s
    #define CH(c) L ## c
#else
    typedef char clr_char;
    #define STR(s) s
    #define CH(c) c
#endif

typedef struct FunctionCompileInfo
{
    Oid FunctionOid;
    const clr_char* FunctionName;
    const clr_char* FunctionBody;
} FunctionCompileInfo, *FunctionCompileInfoPtr;

void* plclr_compile(FunctionCompileInfoPtr compileInfo);

#endif // PLCLR_H