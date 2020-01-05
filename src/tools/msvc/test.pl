#!/usr/bin/perl
use strict;
use warnings;

chdir("../../..")    if (-f "../../../configure");
chdir("../../../..") if (-f "../../../../configure");

our $config;
unless ($config)
{

	# suppress warning about harmless redeclaration of $config
	no warnings 'misc';
	do "src/tools/msvc/config_default.pl";
	do "src/tools/msvc/config.pl" if (-f "src/tools/msvc/config.pl");
}

my $conf = "debug";
my $target = 'C:\\a\\pg';

PublishPlClrProjects();

sub PublishPlClrProjects
{
	my $dotnet = 'dotnet';
	if (-d $config->{clr})
	{
		$dotnet = $config->{clr} . "\\dotnet";
	}
	my $dir = __FILE__;
	print "$dir\n";

	my $managedBasePath = 'src/pl/plclr/managed';
	my $solutionFileName = "$managedBasePath/PlClrManaged.sln";
	my $F;
	open($F, '<:encoding(UTF-8)', $solutionFileName) || die "Could not open file $solutionFileName";
	LINE: while (<$F>) {
		chomp;
		next LINE if !m/^Project\("\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}}"\) = "[^"]+", "(?<ProjectPath>[^"]+)", "\{{0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}}"$/;
		print `$dotnet publish --nologo $managedBasePath/$+{ProjectPath} --output $target/lib --configuration $conf`;
	}
	my $txt = <$F>;
	close($F);
}
