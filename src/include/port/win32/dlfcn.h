/* src/include/port/win32/dlfcn.h */

#ifdef WIN32

/* Dummy values that won't get used on Windows,
   just to make the compiler happy */
#define RTLD_LOCAL 0
#define RTLD_LAZY 0x00001

#endif // WIN32
