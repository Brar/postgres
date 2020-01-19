#include "postgres.h"

#include "miscadmin.h"
#include "port.h"
#include "utils/builtins.h"

#include "plclr_managed.h"
#include "plclr_runtime_host.h"
#include "plclr_string.h"

#include "nethost.h"
/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/coreclr_delegates.h */
#include "coreclr_delegates.h"
/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/hostfxr.h */
#include "hostfxr.h"

#include <dlfcn.h>
#include <string.h>

typedef struct PlClrUnmanagedInterface
{
    void* (*PallocFunctionPtr)(Size);
    void* (*Palloc0FunctionPtr)(Size);
    void* (*RePallocFunctionPtr)(void*, Size);
    void (*PFreeFunctionPtr)(void*);
    void (*ELogFunctionPtr)(int, const char*);
    void (*EReportFunctionPtr)(int, int*, void*, void*, void*, void*, Oid*);
	void* (*GetTextFunctionPtr)(Datum);
} PlClrUnmanagedInterface, *PlClrUnmanagedInterfacePtr;

typedef void* (CORECLR_DELEGATE_CALLTYPE *PlClrMainDelegate)(void *arg, int arg_size_in_bytes);

/* Forward declarations */
static void* open_dynamic_library(const clr_char* path);
static char* get_last_dynamic_library_error(void);
static void* get_export(void* lib, const char* name);
static void plclr_elog(int, const char*);
static void plclr_ereport(
	int elevel,
	int* errcode_value,
	void* errmsg_internal_value,
	void* errdetail_internal_value,
	void* errdetail_log_value,
	void* errhint_value,
	Oid* errdatatype_value);
static void* plclr_get_text(Datum);

/* Globals to hold managed exports */
static hostfxr_close_fn hostfxr_close;
static hostfxr_handle cxt;
PlClrManagedInterfacePtr plclrManagedInterface;

void
plclr_runtime_host_init(void)
{
	hostfxr_initialize_for_runtime_config_fn hostfxr_initialize;
	hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate;
	PlClrMainDelegate PlClrMain_Setup;
    PlClrUnmanagedInterfacePtr setupInfo;
    char buffer[MAXPGPATH];
    size_t buffer_size = MAXPGPATH;

	/*
	 * On Windows we need a second buffer to store the UTF-16LE bytes
	 * we need to convert our strings from/to. Elsewhere UTF-8 is fine
	 * so our second buffer simply points to the first one
	 */
#ifdef WIN32
    clr_char wide_buffer[MAXPGPATH];
#else
    clr_char* wide_buffer = (clr_char*)&buffer;
#endif

    int rc;
    void* lib;
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = NULL;
    const clr_char* dotnet_type = STR("PlClr.PlClrMain, PlClr.Managed");
    const clr_char* dotnet_type_method = STR("Setup");
    const clr_char* dotnet_type_delegate = STR("PlClr.PlClrMainDelegate, PlClr.Managed");

    rc = get_hostfxr_path(wide_buffer, &buffer_size, NULL);
    if (rc != 0)
        elog(ERROR, "Failed to get hostfxr path");

    /* Load hostfxr and get desired exports */
    lib = open_dynamic_library(wide_buffer);
    if (lib == NULL)
        elog(ERROR, "Failed to load %s: %s", buffer, get_last_dynamic_library_error());

    /* ReSharper disable CppIncompatiblePointerConversion */
    hostfxr_initialize = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
    hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
    hostfxr_close = (hostfxr_close_fn)get_export(lib, "hostfxr_close");
    /* ReSharper restore CppIncompatiblePointerConversion */

    buffer[0] = '\0';
    get_lib_path(my_exec_path, buffer);
    strncat(buffer, "/managed/PlClr.Managed.runtimeconfig.json", MAXPGPATH - 1);

#ifdef WIN32
    utf8_to_utf16le(buffer, wide_buffer, MAXPGPATH);
#endif

    /* Initialize the host context */
    rc = hostfxr_initialize(wide_buffer, NULL, &cxt);
    if (rc != 0 || cxt == NULL)
    {
        hostfxr_close(cxt);
    	cxt = NULL;
        elog(ERROR, "Init failed: %x", rc);
    }

    /* Get the load assembly function pointer */
    rc = hostfxr_get_runtime_delegate(
        cxt,
        hdt_load_assembly_and_get_function_pointer,
        (void*)&load_assembly_and_get_function_pointer);

    if (rc != 0 || load_assembly_and_get_function_pointer == NULL)
        elog(ERROR, "Get delegate failed: %x", rc);

    buffer[0] = '\0';
    get_lib_path(my_exec_path, buffer);
    strcat(buffer, "/managed/PlClr.Managed.dll");

#ifdef WIN32
    utf8_to_utf16le(buffer, wide_buffer, MAXPGPATH);
#endif
    rc = load_assembly_and_get_function_pointer(
        wide_buffer,
        dotnet_type,
        dotnet_type_method,
        dotnet_type_delegate,
        NULL,
        (void**)&PlClrMain_Setup);
	if (rc != 0 || PlClrMain_Setup == NULL)
		elog(ERROR, "Function load_assembly_and_get_function_pointer() failed: %x", rc);

	setupInfo = palloc(sizeof(PlClrUnmanagedInterface));

	setupInfo->ELogFunctionPtr = plclr_elog;
	setupInfo->PFreeFunctionPtr = pfree;
	setupInfo->Palloc0FunctionPtr = palloc0;
	setupInfo->PallocFunctionPtr = palloc;
	setupInfo->RePallocFunctionPtr = repalloc;
	setupInfo->GetTextFunctionPtr = plclr_get_text;

	plclrManagedInterface = PlClrMain_Setup(setupInfo, sizeof(PlClrUnmanagedInterface));

	if (!plclrManagedInterface)
        elog(ERROR, "PL/CLR main setup failed");	
}

static void*
get_export(void *lib, const char *name)
{
    void *f = dlsym(lib, name);
    if (f == NULL)
        elog(ERROR, "PL/CLR failed to get export %s: %s", name, dlerror());

    return f;
}

#ifdef WIN32
static char last_dynamic_library_error[512];

static void
set_last_dynamic_library_error(void)
{
	DWORD		err = GetLastError();

	/*
	 * We explicitly use FormatMessageA here as the resulting string
	 * will be passed to snprintf and later to elog.
	 */
	if (FormatMessageA(FORMAT_MESSAGE_IGNORE_INSERTS |
					  FORMAT_MESSAGE_FROM_SYSTEM,
					  NULL,
					  err,
					  MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT),
					  last_dynamic_library_error,
					  sizeof(last_dynamic_library_error) - 1,
					  NULL) == 0)
	{
		snprintf(last_dynamic_library_error, sizeof(last_dynamic_library_error) - 1,
				 "unknown error %lu", err);
	}
}
#endif

/*
 * Open a dynamic library in a portable way
 *
 * The windows part is almost a verbatim copy of src/port/dlopen.c
 * except for the fact that it accepts a clr_char* as input
 * parameter (to save us one encoding conversion) and uses
 * LoadLibraryW to safely load the library from a path that
 * could (hypothetically) contain unicode characters.
 */
static void*
open_dynamic_library(const clr_char* path)
{
#ifdef WIN32
	HMODULE	h;
	int prevmode;

	/* Disable popup error messages when loading DLLs */
	prevmode = SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOOPENFILEERRORBOX);
	h = LoadLibraryW(path);
	SetErrorMode(prevmode);

	if (!h)
	{
		set_last_dynamic_library_error();
		return NULL;
	}
	last_dynamic_library_error[0] = 0;
	return (void *) h;

#else
	void* h;
	h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
	return h;
#endif
}

static char *
get_last_dynamic_library_error(void)
{
#ifdef WIN32
	if (last_dynamic_library_error[0])
		return last_dynamic_library_error;
	else
		return NULL;
#else
	return dlerror();
#endif
}

static void
plclr_elog(int elevel, const char* message)
{
	elog(elevel, "%s", message);
	/* hostfxr_close(cxt); */
}

static void
plclr_ereport(int elevel, int* errcode_value, void* errmsg_internal_value, void* errdetail_internal_value, void* errdetail_log_value, void* errhint_value, Oid* errdatatype_value)
{
	if (errcode_value != NULL)
		errcode(*errcode_value);
	if (errmsg_internal_value != NULL)
	{
		char* errmsg_internal_char_ptr = clr_char_to_server_encoding((clr_char*)errmsg_internal_value);
		if (errmsg_internal_char_ptr != errmsg_internal_value)
			pfree(errmsg_internal_value);

		errmsg_internal("%s", errmsg_internal_char_ptr);
	}
	if (errdetail_internal_value != NULL)
	{
		char* errdetail_internal_char_ptr = clr_char_to_server_encoding((clr_char*)errdetail_internal_value);
		if (errdetail_internal_char_ptr != errdetail_internal_value)
			pfree(errdetail_internal_value);

		errdetail_internal("%s", errdetail_internal_char_ptr);
	}
	if (errdetail_log_value != NULL)
	{
		char* errdetail_log_char_ptr = clr_char_to_server_encoding((clr_char*)errdetail_log_value);
		if (errdetail_log_char_ptr != errdetail_log_value)
			pfree(errdetail_log_value);

		errdetail_log("%s", errdetail_log_char_ptr);
	}
	if (errhint_value != NULL)
	{
		char* errhint_char_ptr = clr_char_to_server_encoding((clr_char*)errhint_value);
		if (errhint_char_ptr != errhint_value)
			pfree(errhint_value);

		errhint("%s", errhint_char_ptr);
	}
	if (errdatatype_value != NULL)
		errdatatype(*errdatatype_value);

	ereport(elevel);
	/* hostfxr_close(cxt); */
}

static void*
plclr_get_text(Datum arg)
{
	text* server_encoded_text = DatumGetTextPP(arg);
	clr_char* clr_encoded_text = server_encoding_to_clr_char(VARDATA_ANY(server_encoded_text));

	if ((Pointer) server_encoded_text != DatumGetPointer(arg))
		pfree(server_encoded_text);

	return (void*)clr_encoded_text;
}