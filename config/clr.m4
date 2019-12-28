#
# Autoconf macros for configuring the build of CLR (.NET Core) extension modules
#
# config/clr.m4
#

# PGAC_PATH_DOTNET
# ----------------
# Look for dotnet and set the output variable 'DOTNET' if found,
# fail otherwise.
AC_DEFUN([PGAC_PATH_DOTNET],
[PGAC_PATH_PROGS(DOTNET, [dotnet])
AC_ARG_VAR(DOTNET, [dotnet program])dnl
if test x"$DOTNET" = x""; then
  AC_MSG_ERROR([dotnet not found])
fi
])


# PGAC_CHECK_CLR_EMBED_SETUP
# -----------------------
# Determine the nethost directory of a given .NET Core sdk installation,
# as well as the .NET Core SDK and runtime versions.
AC_DEFUN([PGAC_CHECK_CLR_EMBED_SETUP],
[dotnet_sdk_version=`${DOTNET} --version`
AC_MSG_NOTICE([using .NET Core SDK $dotnet_sdk_version])
# dotnet_sdk_version is typically n.n.n
dotnet_sdk_majorversion=`echo "$dotnet_sdk_version" | sed '[s/^\([0-9]\{1,\}\).*/\1/]'`
dotnet_sdk_minorversion=`echo "$dotnet_sdk_version" | sed '[s/^[0-9]\{1,\}\.\([0-9]\{1,\}\).*/\1/]'`
# Reject unsupported .NET Core versions as soon as practical.
if test "$dotnet_sdk_majorversion" -lt 3; then
  AC_MSG_ERROR([.NET Core version $dotnet_sdk_version is too old (version 3.0 or later is required)])
fi

dotnet_runtime_version=`${DOTNET} --list-runtimes | grep "Microsoft.NETCore.App ${dotnet_sdk_majorversion}.${dotnet_sdk_minorversion}" | sed 's/^Microsoft\.NETCore\.App \(.*\) .*$/\1/'`
dotnet_runtime_majorversion=`echo "$dotnet_runtime_version" | sed '[s/^\([0-9]\{1,\}\).*/\1/]'`
dotnet_runtime_minorversion=`echo "$dotnet_runtime_version" | sed '[s/^[0-9]\{1,\}\.\([0-9]\{1,\}\).*/\1/]'`
dotnet_runtime_patch=`echo "$dotnet_runtime_version" | sed '[s/^[0-9]\{1,\}\.[0-9]\{1,\}\.\([0-9]\{1,\}\).*/\1/]'`
AC_MSG_NOTICE([using .NET Core runtime $dotnet_runtime_version])

AC_MSG_CHECKING([.NET Core installation base directory])
clr_basedir=`${DOTNET} --info | grep 'Base Path:' | sed "s/^ *Base Path: *\(\/.*\)sdk.*$/\1/"`
if test -d $clr_basedir
then
  AC_MSG_RESULT([$clr_basedir])
else
  AC_MSG_ERROR([.NET Core installation base directory not found])
fi

AC_MSG_CHECKING([.NET Core nethost directory])
clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.linux-x64/$dotnet_runtime_version/runtimes/linux-x64/native/
if test -d $clr_nethostdir
then
  AC_MSG_RESULT([$clr_nethostdir])
else
  AC_MSG_ERROR([nethost directory not found])
fi

AC_SUBST(dotnet_sdk_majorversion)[]dnl
AC_SUBST(dotnet_sdk_minorversion)[]dnl
AC_SUBST(dotnet_sdk_version)[]dnl
AC_SUBST(dotnet_runtime_majorversion)[]dnl
AC_SUBST(dotnet_runtime_minorversion)[]dnl
AC_SUBST(dotnet_runtime_patch)[]dnl
AC_SUBST(dotnet_runtime_version)[]dnl
AC_SUBST(clr_nethostdir)[]dnl
])# PGAC_CHECK_CLR_EMBED_SETUP


