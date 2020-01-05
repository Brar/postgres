#include "postgres.h"
#include "miscadmin.h"
#include "port.h"
#include "plclr_runtime_host.h"
#include "plclr_string.h"
#include "nethost.h"

/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/coreclr_delegates.h */
#include "coreclr_delegates.h"
/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/hostfxr.h */
#include "hostfxr.h"

#include <dlfcn.h>
#include <string.h>

/* Forward declarations */
static void *get_export(void* lib, const char* name);
static load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const clr_char* assembly);
static void plclr_elog(int, char*);

static void
plclr_elog(int elevel, char* message)
{
	elog(elevel, message);
}

typedef struct SetupInfo
{
    void* (*PallocFunctionPtr)(Size);
    void* (*Palloc0FunctionPtr)(Size);
    void* (*RePallocFunctionPtr)(void*, Size);
    void (*PFreeFunctionPtr)(void*);
    void (*ELogFunctionPtr)(int, char*);
} DelegateSetupInfo;

/* Globals to hold hostfxr exports */
hostfxr_initialize_for_runtime_config_fn hostfxr_initialize;
hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate;
hostfxr_close_fn hostfxr_close;

component_entry_point_fn plclr_compile_function = NULL;


int
compile_and_execute(FunctionCompileInfoPtr compileInfo)
{
    return plclr_compile_function(compileInfo, sizeof(FunctionCompileInfo));
}

void
plclr_runtime_host_init(void)
{
    char buffer[MAXPGPATH];
    size_t buffer_size = MAXPGPATH;
#ifdef WIN32
    clr_char wide_buffer[MAXPGPATH];
#else
    clr_char* wide_buffer = (clr_char*)&buffer;
#endif

    int rc;
    void *lib;
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = NULL;
    const clr_char* dotnet_type = STR("PlClr.PlClrMain, PlClr.Managed");
    const clr_char* dotnet_type_method = STR("CompileFunction");

    rc = get_hostfxr_path(wide_buffer, &buffer_size, NULL);
    if (rc != 0)
        elog(ERROR, "Failed to get hostfxr path");

#ifdef WIN32
    utf16le_to_utf8(wide_buffer, buffer, MAXPGPATH);
#endif
    // Load hostfxr and get desired exports
    lib = dlopen(buffer, RTLD_LAZY | RTLD_LOCAL);
    if (lib == NULL)
        elog(ERROR, "Failed to load %s: %s", buffer, dlerror());

    hostfxr_initialize = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
    hostfxr_get_runtime_delegate = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
    hostfxr_close = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

    buffer[0] = '\0';
    get_lib_path(my_exec_path, buffer);
    strcat(buffer, "/managed/PlClr.Managed.runtimeconfig.json");

#ifdef WIN32
    utf8_to_utf16le(buffer, wide_buffer, MAXPGPATH);
#endif
    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(wide_buffer);
    if (load_assembly_and_get_function_pointer == NULL)
        elog(ERROR, "Function get_dotnet_load_assembly(\"%s\") failed", buffer);

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
        NULL,
        NULL,
        (void**)&plclr_compile_function);

    if (rc != 0 || plclr_compile_function == NULL)
        elog(ERROR, "Function load_assembly_and_get_function_pointer() failed: %x", rc);
}

static void*
get_export(void *lib, const char *name)
{
    void *f = dlsym(lib, name);
    if (f == NULL)
        elog(ERROR, "PL/CLR failed to get export %s: %s", name, dlerror());

    return f;
}

static load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const clr_char *config_path)
{
    // Load .NET Core
    void *load_assembly_and_get_function_pointer = NULL;
    hostfxr_handle cxt = NULL;
    int rc;

    rc = hostfxr_initialize(config_path, NULL, &cxt);
    if (rc != 0 || cxt == NULL)
    {
        hostfxr_close(cxt);
        elog(ERROR, "Init failed: %x", rc);
    }

    // Get the load assembly function pointer
    rc = hostfxr_get_runtime_delegate(
        cxt,
        hdt_load_assembly_and_get_function_pointer,
        &load_assembly_and_get_function_pointer);

    if (rc != 0 || load_assembly_and_get_function_pointer == NULL)
        elog(ERROR, "Get delegate failed: %x", rc);

    hostfxr_close(cxt);
    return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
}