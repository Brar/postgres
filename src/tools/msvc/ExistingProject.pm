package ExistingProject;

#
# Package that encapsulates an existing project.
# This means that it is mostly a dummy project
# that only exists in order to include it into
# a generated solution file.
#
# src/tools/msvc/ExistingProject.pm
#
use Carp;
use strict;
use warnings;
use File::Basename;

sub new
{
	my ($classname, $projectFile, $solution) = @_;
	confess("Failed to get file extension from '$projectFile'\n") unless basename($projectFile) =~ m/^(?<FileNameBase>.+)\.(?<Extension>[^.]+)$/;

	my $typeguid;
	if (lc($+{Extension}) eq 'csproj') {
		$typeguid = '{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}';
	} elsif (lc($+{Extension}) eq 'vcxproj') {
		$typeguid = '{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}';
	} else {
		confess("Unknown file extension: '$+{Extension}'\n");
	}

	my $self = {
		name                  => $+{FileNameBase},
		path                  => $projectFile,
		typeguid              => $typeguid,
		guid                  => Win32::GuidGen(),
		platform              => $solution->{platform},
	};

	bless($self, $classname);
	return $self;
}

sub Save
{
	# Dummy.
	# The project file already exists
}

1;
