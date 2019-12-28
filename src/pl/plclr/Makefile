# src/pl/plclr/Makefile

subdir = src/pl/plclr
top_builddir = ../../..
include $(top_builddir)/src/Makefile.global

override CPPFLAGS := -I. -I$(srcdir) -I$(clr_nethostdir) $(CPPFLAGS)

override LDFLAGS := $(LDFLAGS) -L $(clr_nethostdir) -lnethost

rpathdir = $(clr_nethostdir)

PGFILEDESC = "PL/CLR - procedural language"

NAME = plclr

OBJS = \
	$(WIN32RES) \
	plclr_main.o \
	plclr_runtime_host.o

DATA += plclr.control plclr--1.0.sql plclr--unpackaged--1.0.sql

SHLIB_LINK = $(filter -lintl,$(LIBS))

REGRESS_OPTS = --dbname=$(PL_TESTDB)

REGRESS = plclr_test

include $(top_srcdir)/src/Makefile.shlib

all: all-lib

# Ensure parallel safety if a build is started in this directory
$(OBJS): | submake-generated-headers

install: all install-lib install-data

installdirs: installdirs-lib
	$(MKDIR_P) '$(DESTDIR)$(datadir)/extension' '$(DESTDIR)$(includedir_server)' '$(DESTDIR)$(pgxsdir)/src/pl/plclr'

uninstall: uninstall-lib uninstall-data

install-data: installdirs
	$(INSTALL_DATA) $(addprefix $(srcdir)/, $(DATA)) '$(DESTDIR)$(datadir)/extension/'

uninstall-data:
	rm -f $(addprefix '$(DESTDIR)$(datadir)/extension'/, $(notdir $(DATA)))

.PHONY: install-data uninstall-data

check: submake
	$(pg_regress_check) $(REGRESS_OPTS) $(REGRESS)

installcheck: submake
	$(pg_regress_installcheck) $(REGRESS_OPTS) $(REGRESS)

.PHONY: submake
submake:
	$(MAKE) -C $(top_builddir)/src/test/regress pg_regress$(X)

clean distclean: clean-lib
	rm -f $(OBJS)
	rm -rf $(pg_regress_clean_files)
