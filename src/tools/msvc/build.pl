# -*-perl-*- hey - emacs - this is a perl file

# src/tools/msvc/build.pl

use strict;

use File::Basename;
use File::Spec;
BEGIN { use lib File::Spec->rel2abs(dirname(__FILE__)); }

use Cwd;

use Mkvcbuild;

chdir('../../..') if (-d '../msvc' && -d '../../../src');
die 'Must run from root or msvc directory'
  unless (-d 'src/tools/msvc' && -d 'src');

# buildenv.pl is for specifying the build environment settings
# it should contain lines like:
# $ENV{PATH} = "c:/path/to/bison/bin;$ENV{PATH}";

if (-e "src/tools/msvc/buildenv.pl")
{
	do "./src/tools/msvc/buildenv.pl";
}
elsif (-e "./buildenv.pl")
{
	do "./buildenv.pl";
}

# set up the project
our $config;
do "./src/tools/msvc/config_default.pl";
do "./src/tools/msvc/config.pl" if (-f "src/tools/msvc/config.pl");

my $vcver = Mkvcbuild::mkvcbuild($config);

# check what sort of build we are doing

my $bconf     = $ENV{CONFIG}   || "Release";
my $msbflags  = $ENV{MSBFLAGS} || "";
my $buildwhat = $ARGV[1]       || "";
if (uc($ARGV[0]) eq 'DEBUG')
{
	$bconf = "Debug";
}
elsif (uc($ARGV[0]) ne "RELEASE")
{
	$buildwhat = $ARGV[0] || "";
}

# Restore managed packages if we build plclr
if ($config->{clr}) {
	my $dotnet = 'dotnet';
	if (-d $config->{clr})
	{
		$dotnet = $config->{clr} . "\\dotnet";
	}

	print `$dotnet restore src/pl/plclr/managed/PlClr.Managed.sln`;

	# Build managed packages here as a workaround for insufficient project dependency
	# setup. See comment in the plclr section of Mkvcbuild.pm
	if (lc($buildwhat) eq 'plclr') {
		print `$dotnet build --nologo --no-restore --configuration $bconf src/pl/plclr/managed/PlClr.Managed.sln`;
	}
}

# ... and do it

if ($buildwhat)
{
	system(
		"msbuild $buildwhat.vcxproj /verbosity:normal $msbflags /p:Configuration=$bconf"
	);
}
else
{
	system(
		"msbuild pgsql.sln /verbosity:normal $msbflags /p:Configuration=$bconf"
	);
}

# report status

my $status = $? >> 8;

exit $status;
