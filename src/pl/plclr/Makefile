# src/pl/plclr/Makefile

subdir = src/pl/plclr
top_builddir = ../../..
include $(top_builddir)/src/Makefile.global

override CPPFLAGS := -I. -I$(srcdir) -I$(clr_nethostdir) $(CPPFLAGS)

rpathdir = $(clr_nethostdir)

override LDFLAGS := $(LDFLAGS) -L $(clr_nethostdir) -lnethost

PGFILEDESC = "PL/CLR - procedural language"

ifeq ($(enable_debug),1)
DOTNET_CONFIG = Debug
else
DOTNET_CONFIG = Release
endif

DOTNET_FLAGS = --nologo --configuration $(DOTNET_CONFIG) --framework netcoreapp$(dotnet_runtime_majorversion).$(dotnet_runtime_minorversion)

NAME = plclr

OBJS = \
	$(WIN32RES) \
	plclr_comp.o \
	plclr_exec.o \
	plclr_func_cache.o \
	plclr_handler.o \
	plclr_runtime_host.o \
	plclr_string.o

DATA += plclr.control plclr--1.0.sql plclr--unpackaged--1.0.sql

SHLIB_LINK = $(filter -lintl,$(LIBS))

REGRESS_OPTS = --dbname=$(PL_TESTDB) --load-extension=plclr

REGRESS = plclr_test

include $(top_srcdir)/src/Makefile.shlib

all: all-lib managed-lib

# Ensure parallel safety if a build is started in this directory
$(OBJS): | submake-generated-headers

managed-lib:
	sed -i "s/<TargetFramework>netcoreapp.\..<\/TargetFramework>/<TargetFramework>netcoreapp$(dotnet_runtime_majorversion)\.$(dotnet_runtime_minorversion)<\/TargetFramework>/gi" managed/src/PlClr.Managed/PlClr.Managed.csproj
	sed -i "s/<TargetFramework>netcoreapp.\..<\/TargetFramework>/<TargetFramework>netcoreapp$(dotnet_runtime_majorversion)\.$(dotnet_runtime_minorversion)<\/TargetFramework>/gi" managed/test/PlClr.Managed.Tests/PlClr.Managed.Tests.csproj
	$(DOTNET) build managed/src/PlClr.Managed/PlClr.Managed.csproj $(DOTNET_FLAGS)
	$(DOTNET) build managed/test/PlClr.Managed.Tests/PlClr.Managed.Tests.csproj $(DOTNET_FLAGS)

.PHONY: managed-lib

install: all install-lib install-data

installdirs: installdirs-lib
	$(MKDIR_P) '$(DESTDIR)$(datadir)/extension' '$(DESTDIR)$(includedir_server)' '$(DESTDIR)$(pgxsdir)/src/pl/plclr' '$(DESTDIR)$(libdir)/managed'

uninstall: uninstall-lib uninstall-data

install-data: installdirs
	$(INSTALL_DATA) $(addprefix $(srcdir)/, $(DATA)) '$(DESTDIR)$(datadir)/extension/'

uninstall-data:
	rm -f $(addprefix '$(DESTDIR)$(datadir)/extension'/, $(notdir $(DATA)))

.PHONY: install-data uninstall-data

managed-check-install: temp-install
	$(DOTNET) publish managed/src/PlClr.Managed/PlClr.Managed.csproj $(DOTNET_FLAGS) -o $(abs_top_builddir)/tmp_install$(libdir)/managed

.PHONY: managed-check-install

check: submake managed-check-install
	$(DOTNET) test managed/test/PlClr.Managed.Tests/PlClr.Managed.Tests.csproj $(DOTNET_FLAGS)
	$(pg_regress_check) $(REGRESS_OPTS) $(REGRESS)

installcheck: submake
	$(pg_regress_installcheck) $(REGRESS_OPTS) $(REGRESS)

.PHONY: submake
submake:
	$(MAKE) -C $(top_builddir)/src/test/regress pg_regress$(X)

clean distclean: clean-lib
	rm -f $(OBJS)
	rm -rf $(pg_regress_clean_files)
	$(DOTNET) clean managed/PlClr.Managed.sln $(DOTNET_FLAGS)
