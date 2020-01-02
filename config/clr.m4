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

dotnet_info=`${DOTNET} --info`
AC_MSG_CHECKING([.NET Core installation base directory])
clr_basedir=`echo "$dotnet_info" | grep 'Base Path:' | sed "s/^ *Base Path: *\(\/.*\)sdk.*$/\1/"`
if test -d $clr_basedir
then
  AC_MSG_RESULT([$clr_basedir])
else
  AC_MSG_ERROR([.NET Core installation base directory not found])
fi

AC_MSG_CHECKING([system runtime identifier])
system_runtime_identifier=`echo "$dotnet_info" | grep 'RID:' | sed "s/^ *RID: *\(\.*\)/\1/"`
AC_MSG_RESULT([$system_runtime_identifier])

clr_runtime_identifier=$system_runtime_identifier

AC_MSG_CHECKING([.NET Core nethost directory])
case $clr_runtime_identifier in
  alpine)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl-x64 linux-musl linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.9 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.9-x64 alpine.3.8-x64 alpine.3.7-x64 alpine.3.6-x64 linux-musl-x64 alpine.3.10 alpine.3.9 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine-x64 linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.11)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.10 alpine.3.9 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.11-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.10-x64 alpine.3.9-x64 alpine.3.8-x64 alpine.3.7-x64 alpine.3.6-x64 linux-musl-x64 alpine.3.11 alpine.3.10 alpine.3.9 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine-x64 linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.6)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.6-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl-x64 alpine.3.6 alpine-x64 linux-musl linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.7)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.6 linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.7-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.6-x64 linux-musl-x64 alpine.3.7 alpine.3.6 linux-musl alpine-x64 linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.8)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.7 alpine.3.6 linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.8-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.7-x64 alpine.3.6-x64 linux-musl-x64 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine-x64 linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.9)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  alpine.3.9-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in alpine.3.8-x64 alpine.3.7-x64 alpine.3.6-x64 linux-musl-x64 alpine.3.9 alpine.3.8 alpine.3.7 alpine.3.6 linux-musl alpine-x64 linux-x64 unix-x64 alpine linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm unix-arm android linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 unix-arm64 android linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android.21)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in android linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android.21-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in android-arm android.21 linux-arm unix-arm android linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  android.21-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in android-arm64 linux-arm64 android.21 unix-arm64 android linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  any)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in base
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  aot)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  arch)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  arch-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 linux arch unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  base)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in 
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 rhel-x64 unix-x64 centos linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos.7)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in centos rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos.7-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in centos-x64 rhel.7-x64 linux-x64 centos.7 rhel-x64 unix-x64 centos rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos.8)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in centos rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  centos.8-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in centos-x64 rhel.8-x64 linux-x64 centos.8 rhel-x64 unix-x64 centos rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm unix-arm debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 unix-arm64 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-armel unix-armel debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x86 unix-x86 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm debian.10 linux-arm unix-arm debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm64 linux-arm64 unix-arm64 debian.10 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-armel linux-armel unix-armel debian.10 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x64 debian.10 linux-x64 unix-x64 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x86 debian.10 linux-x86 unix-x86 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm linux-arm debian.8 unix-arm debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm64 linux-arm64 unix-arm64 debian.8 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-armel linux-armel unix-armel debian.8 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x64 linux-x64 debian.8 unix-x64 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.8-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x86 linux-x86 debian.8 unix-x86 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm linux-arm debian.9 unix-arm debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm64 linux-arm64 unix-arm64 debian.9 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-armel linux-armel unix-armel debian.9 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x64 linux-x64 debian.9 unix-x64 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  debian.9-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x86 linux-x86 debian.9 unix-x86 debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 unix-arm64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.23)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.23-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.23 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.23-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.23 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.24)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.24-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.24 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.24-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.24 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.25)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.25-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.25 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.25-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.25 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.26)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.26-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.26 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.26-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.26 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.27)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.27-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.27 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.27-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.27 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.28)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.28-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.28 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.28-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.28 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.29)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.29-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.29 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.29-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.29 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.30)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.30-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.30 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.30-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.30 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.31)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.31-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.31 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.31-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.31 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.32)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.32-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-arm64 linux-arm64 unix-arm64 fedora.32 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  fedora.32-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in fedora-x64 fedora.32 linux-x64 unix-x64 fedora linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-x64 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.11)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.11-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd-x64 freebsd.11 unix-x64 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.12)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd.11 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.12-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd.11-x64 freebsd-x64 freebsd.12 freebsd.11 unix-x64 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.13)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd.12 freebsd.11 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  freebsd.13-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in freebsd.12-x64 freebsd.11-x64 freebsd-x64 freebsd.13 freebsd.12 freebsd.11 unix-x64 freebsd unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  gentoo)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  gentoo-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 gentoo linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-arm linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-arm64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-armel linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl linux-arm unix-arm linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 linux-musl unix-arm64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-armel linux-musl unix-armel linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl linux-x64 unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-musl-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-musl linux-x86 unix-x86 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linux-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-x86 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.04-x64 linuxmint.17 ubuntu.14.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17 ubuntu.14.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17-x64 ubuntu.14.04-x64 linuxmint.17.1 linuxmint.17 ubuntu.14.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17.1 linuxmint.17 ubuntu.14.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17.1-x64 linuxmint.17-x64 ubuntu.14.04-x64 linuxmint.17.2 linuxmint.17.1 linuxmint.17 ubuntu.14.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17.2 linuxmint.17.1 linuxmint.17 ubuntu.14.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.17.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.17.2-x64 linuxmint.17.1-x64 linuxmint.17-x64 ubuntu.14.04-x64 linuxmint.17.3 linuxmint.17.2 linuxmint.17.1 linuxmint.17 ubuntu.14.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04-x64 linuxmint.18 ubuntu.16.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18 ubuntu.16.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18-x64 ubuntu.16.04-x64 linuxmint.18.1 linuxmint.18 ubuntu.16.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18.1 linuxmint.18 ubuntu.16.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18.1-x64 linuxmint.18-x64 ubuntu.16.04-x64 linuxmint.18.2 linuxmint.18.1 linuxmint.18 ubuntu.16.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18.2 linuxmint.18.1 linuxmint.18 ubuntu.16.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.18.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.18.2-x64 linuxmint.18.1-x64 linuxmint.18-x64 ubuntu.16.04-x64 linuxmint.18.3 linuxmint.18.2 linuxmint.18.1 linuxmint.18 ubuntu.16.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04-x64 linuxmint.19 ubuntu.18.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.19 ubuntu.18.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.19-x64 ubuntu.18.04-x64 linuxmint.19.1 linuxmint.19 ubuntu.18.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.19.1 linuxmint.19 ubuntu.18.04 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  linuxmint.19.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linuxmint.19.1-x64 linuxmint.19-x64 ubuntu.18.04-x64 linuxmint.19.2 linuxmint.19.1 linuxmint.19 ubuntu.18.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 rhel-x64 unix-x64 linux rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7 linux rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7-x64 linux-x64 rhel-x64 unix-x64 ol-x64 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.0-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.0-x64 rhel.7-x64 linux-x64 ol.7-x64 rhel.7.0 rhel-x64 unix-x64 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.1 rhel.7.0 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.1-x64 rhel.7.0-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.2 rhel.7.1 rhel.7.0 ol.7.1 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 ol.7.1-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.2 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.2 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7.2 ol.7.1 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 ol.7.2-x64 ol.7.1-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.4)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.4-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.4-x64 rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 ol.7.3-x64 ol.7.2-x64 ol.7.1-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.4 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.5)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7.4 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.5-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.5-x64 rhel.7.4-x64 rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 ol.7.4-x64 ol.7.3-x64 ol.7.2-x64 ol.7.1-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.5 ol.7.4 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.6)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.6 rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7.5 ol.7.4 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.7.6-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.6-x64 rhel.7.5-x64 rhel.7.4-x64 rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 ol.7.5-x64 ol.7.4-x64 ol.7.3-x64 ol.7.2-x64 ol.7.1-x64 ol.7.0-x64 rhel.7-x64 linux-x64 rhel.7.6 rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 ol.7-x64 rhel-x64 unix-x64 ol.7.6 ol.7.5 ol.7.4 ol.7.3 ol.7.2 ol.7.1 ol.7.0 rhel.7 ol-x64 linux ol.7 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.8)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8 linux rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.8-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8-x64 linux-x64 rhel-x64 unix-x64 ol-x64 rhel.8 linux ol.8 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.8.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8.0 rhel.8 linux ol.8 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ol.8.0-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8.0-x64 rhel.8-x64 linux-x64 ol.8-x64 rhel.8.0 rhel-x64 unix-x64 ol.8.0 rhel.8 ol-x64 linux ol.8 rhel unix base any ol
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.13.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.13.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.13.2 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.15.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.15.0-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.15.0 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.15.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.15.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.15.1 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.42.1 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.42.2 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  opensuse.42.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in opensuse.42.3 opensuse-x64 linux-x64 opensuse unix-x64 linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.11)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.10 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.11-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.10-x64 osx.10.11 osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.12)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.11 osx.10.10 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.12-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.11-x64 osx.10.10-x64 osx.10.12 osx.10.11 osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.13)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.12 osx.10.11 osx.10.10 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.13-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.12-x64 osx.10.11-x64 osx.10.10-x64 osx.10.13 osx.10.12 osx.10.11 osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.14)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.13 osx.10.12 osx.10.11 osx.10.10 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.14-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.13-x64 osx.10.12-x64 osx.10.11-x64 osx.10.10-x64 osx.10.14 osx.10.13 osx.10.12 osx.10.11 osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.15)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.14 osx.10.13 osx.10.12 osx.10.11 osx.10.10 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  osx.10.15-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in osx.10.14-x64 osx.10.13-x64 osx.10.12-x64 osx.10.11-x64 osx.10.10-x64 osx.10.15 osx.10.14 osx.10.13 osx.10.12 osx.10.11 osx.10.10 unix-x64 osx-x64 unix base osx any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 unix-arm64 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.6)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.6-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 rhel-x64 unix-x64 rhel.6 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.0-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7-x64 linux-x64 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.1 rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.1-x64 rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.2 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.2 rhel.7.1 rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.4)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.4-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.5)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.5-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.4-x64 rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.6)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.7.6-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.7.5-x64 rhel.7.4-x64 rhel.7.3-x64 rhel.7.2-x64 rhel.7.1-x64 rhel.7.0-x64 rhel.7-x64 linux-x64 rhel.7.6 rhel.7.5 rhel.7.4 rhel.7.3 rhel.7.2 rhel.7.1 rhel.7.0 rhel-x64 unix-x64 rhel.7 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-arm64 rhel-arm64 unix-arm64 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 rhel-x64 unix-x64 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.0-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8-arm64 linux-arm64 rhel-arm64 unix-arm64 rhel.8.0 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.0-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8-x64 linux-x64 rhel.8.0 rhel-x64 unix-x64 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8.0 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.1-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8.0-arm64 rhel.8-arm64 linux-arm64 rhel-arm64 unix-arm64 rhel.8.1 rhel.8.0 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  rhel.8.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in rhel.8.0-x64 rhel.8-x64 linux-x64 rhel.8.1 rhel.8.0 rhel-x64 unix-x64 rhel.8 linux rhel unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 unix-x64 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x64 sles-x64 unix-x64 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12-x64 sles.12.1 linux-x64 sles-x64 unix-x64 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.2)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.1 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.2-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.1-x64 sles.12-x64 sles.12.2 sles.12.1 linux-x64 sles-x64 unix-x64 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.3)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.2 sles.12.1 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.3-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.2-x64 sles.12.1-x64 sles.12-x64 sles.12.3 sles.12.2 sles.12.1 linux-x64 sles-x64 unix-x64 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.4)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.3 sles.12.2 sles.12.1 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.12.4-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.3-x64 sles.12.2-x64 sles.12.1-x64 sles.12-x64 sles.12.4 sles.12.3 sles.12.2 sles.12.1 linux-x64 sles-x64 unix-x64 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.15)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.4 sles.12.3 sles.12.2 sles.12.1 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.15-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.4-x64 sles.12.3-x64 sles.12.2-x64 sles.12.1-x64 sles.12-x64 sles.12.4 sles.12.3 sles.12.2 sles.12.1 linux-x64 sles-x64 unix-x64 sles.15 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.15.1)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.4 sles.12.3 sles.12.2 sles.12.1 sles.15 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  sles.15.1-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in sles.12.4-x64 sles.12.3-x64 sles.12.2-x64 sles.12.1-x64 sles.15-x64 sles.12-x64 sles.15.1 sles.12.4 sles.12.3 sles.12.2 sles.12.1 linux-x64 sles-x64 unix-x64 sles.15 sles.12 linux sles unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-armel unix-armel tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in linux-x86 unix-x86 tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.4.0.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.4.0.0-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen.4.0.0 tizen-armel linux-armel unix-armel tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.4.0.0-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen.4.0.0 tizen-x86 linux-x86 unix-x86 tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.5.0.0)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen.4.0.0 tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.5.0.0-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen.4.0.0-armel tizen.5.0.0 tizen.4.0.0 tizen-armel linux-armel unix-armel tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  tizen.5.0.0-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in tizen.4.0.0-x86 tizen.5.0.0 tizen.4.0.0 tizen-x86 linux-x86 unix-x86 tizen linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.14.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.14.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.15.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.15.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.04-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.10-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.10 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.16.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.16.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.04-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.04 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.10-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.10 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.17.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.17.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.04-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.10-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.10 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.18.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.18.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.04)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.04-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.04 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.04-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.04 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.04-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.04 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.04-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.04 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.10)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.10-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.10 ubuntu-arm debian-arm linux-arm unix-arm ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.10-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.10 ubuntu-arm64 debian-arm64 linux-arm64 unix-arm64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.10-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.10 ubuntu-x64 debian-x64 linux-x64 unix-x64 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  ubuntu.19.10-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ubuntu.19.10 ubuntu-x86 debian-x86 linux-x86 unix-x86 ubuntu debian linux unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix-arm)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix-arm64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix-armel)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix-x64)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
  unix-x86)
    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in unix base any
      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;
esac

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


