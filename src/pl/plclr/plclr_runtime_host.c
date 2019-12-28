#include "postgres.h"
#include "plclr_runtime_host.h"
#include "nethost.h"

/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/coreclr_delegates.h */
#include "coreclr_delegates.h"
/* https://github.com/dotnet/runtime/blob/master/src/installer/corehost/cli/hostfxr.h */
#include "hostfxr.h"

#include <dlfcn.h>

/* Forward declarations */
void *get_export(void *, const char *);

// Globals to hold hostfxr exports
hostfxr_initialize_for_runtime_config_fn init_fptr;
hostfxr_get_runtime_delegate_fn get_delegate_fptr;
hostfxr_close_fn close_fptr;


void
plclr_runtime_host_init(void)
{
	size_t buffer_size;
    char_t *buffer;
    int rc;
    void *lib;

	get_hostfxr_path(NULL, &buffer_size, NULL);
	buffer = palloc(buffer_size);
	rc = get_hostfxr_path(buffer, &buffer_size, NULL);
	if (rc != 0)
		elog(ERROR, "PL/CLR: Failed to get hostfxr path");

	// Load hostfxr and get desired exports
    lib = dlopen(buffer, RTLD_LAZY | RTLD_LOCAL);
    if (lib == NULL)
        elog(ERROR, "PL/CLR failed to load %s: %s", buffer, dlerror());

	init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
	get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
	close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");
}

void *get_export(void *h, const char *name)
{
    void *f = dlsym(h, name);
    if (f == NULL)
        elog(ERROR, "PL/CLR failed to get export %s: %s", name, dlerror());

    return f;
}

