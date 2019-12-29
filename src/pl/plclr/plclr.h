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

#define STR(s) L ## s
#define CH(c) L ## c

#else

#define STR(s) s
#define CH(c) c

#endif

#endif // PLCLR_H