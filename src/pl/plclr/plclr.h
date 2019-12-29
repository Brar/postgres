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
        typedef wchar_t char_t;
    #else
        typedef unsigned short char_t;
    #endif
    #define STR(s) L ## s
    #define CH(c) L ## c
#else
    typedef char char_t;
    #define STR(s) s
    #define CH(c) c
#endif

typedef struct FunctionCompileInfo
{
    Oid FunctionOid;
    const char_t* FunctionName;
    const char_t* FunctionBody;
} FunctionCompileInfo, *FunctionCompileInfoPtr;

int compile_and_execute(FunctionCompileInfoPtr compileInfo);

#endif // PLCLR_H