#!/usr/bin/perl
use strict;
use warnings;
use FindBin qw($Bin);
use lib "$Bin/../../../backend/catalog";
use Catalog;

my $catname = 'pg_type';
my $catalog = Catalog::ParseHeader("$Bin/../../../include/catalog/$catname.h");
my $schema  = $catalog->{$catname};
my $data = Catalog::ParseData("$Bin/../../../include/catalog/$catname.dat", $schema, 0);
my $mapping = {};

foreach my $row (@$data)
{
	$$mapping{$row->{oid}*1} = $row->{typname};
}

foreach my $oid (sort {$a <=> $b} keys %$mapping) {
	print "// $oid => typeof($$mapping{$oid}), // $$mapping{$oid}\n";
}
