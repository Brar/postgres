using System.Collections.Generic;

namespace PlClr
{
    public static partial class ServerTypes
    {
        private static readonly Dictionary<uint, TypeInfo> TypeInfoCache = new Dictionary<uint, TypeInfo>();

        //static ServerTypes()
        //{
        //    /* Currently generated semi-automatically via the following query:
        //        SELECT
        //        CASE t.typtype
	       //         WHEN 'b' THEN 'var typeInfoForOid'||t.oid||' = new TypeInfo('||t.oid||'U, "'||t.typname||'", "'||t.typnamespace::regnamespace||'", '||t.typlen||', '||t.typbyval||');'
	       //         WHEN 'c' THEN 'var typeInfoForOid'||t.oid||' = new CompositeTypeInfo('||t.oid||'U, "'||t.typname||'", "'||t.typnamespace::regnamespace||'", '||t.typlen||', '||t.typbyval||', new []{ '||string_agg('new AttributeInfo("'||a.attname||'", '||a.attnum||', typeInfoForOid'||a.atttypid||', '||a.attnotnull||')', ', ')||' });'
	       //         ELSE 'var typeInfoForOid'||t.oid||' = new TypeInfo('||t.oid||'U, "'||t.typname||'", "'||t.typnamespace::regnamespace||'", '||t.typlen||', '||t.typbyval||');'
        //        END AS init,
        //        '{ '||t.oid||'U, typeInfoForOid'||t.oid||' },' AS lookup_init,
        //        CASE typtype
	       //         WHEN 'b' THEN 1
	       //         WHEN 'p' THEN 2
	       //         WHEN 'e' THEN 3
	       //         WHEN 'a' THEN 4
	       //         WHEN 'd' THEN 5
	       //         WHEN 'r' THEN 6
	       //         WHEN 'c' THEN 7
	       //         ELSE 99999
        //        END AS sort_order
        //        FROM pg_type t
        //        LEFT OUTER JOIN pg_attribute a ON t.typrelid = a.attrelid
        //        WHERE t.oid < 20000
        //        GROUP BY t.oid, t.typname, t.typnamespace, t.typlen, t.typtype, t.typbyval
        //        ORDER BY sort_order, t.oid;
        //     */

        //    var typeInfoForOid16 = new TypeInfo(16U, "bool", "pg_catalog", 1, true);
        //    var typeInfoForOid17 = new TypeInfo(17U, "bytea", "pg_catalog", -1, false);
        //    var typeInfoForOid18 = new TypeInfo(18U, "char", "pg_catalog", 1, true);
        //    var typeInfoForOid19 = new TypeInfo(19U, "name", "pg_catalog", 64, false);
        //    var typeInfoForOid20 = new TypeInfo(20U, "int8", "pg_catalog", 8, true);
        //    var typeInfoForOid21 = new TypeInfo(21U, "int2", "pg_catalog", 2, true);
        //    var typeInfoForOid22 = new TypeInfo(22U, "int2vector", "pg_catalog", -1, false);
        //    var typeInfoForOid23 = new TypeInfo(23U, "int4", "pg_catalog", 4, true);
        //    var typeInfoForOid24 = new TypeInfo(24U, "regproc", "pg_catalog", 4, true);
        //    var typeInfoForOid25 = new TypeInfo(25U, "text", "pg_catalog", -1, false);
        //    var typeInfoForOid26 = new TypeInfo(26U, "oid", "pg_catalog", 4, true);
        //    var typeInfoForOid27 = new TypeInfo(27U, "tid", "pg_catalog", 6, false);
        //    var typeInfoForOid28 = new TypeInfo(28U, "xid", "pg_catalog", 4, true);
        //    var typeInfoForOid29 = new TypeInfo(29U, "cid", "pg_catalog", 4, true);
        //    var typeInfoForOid30 = new TypeInfo(30U, "oidvector", "pg_catalog", -1, false);
        //    var typeInfoForOid114 = new TypeInfo(114U, "json", "pg_catalog", -1, false);
        //    var typeInfoForOid142 = new TypeInfo(142U, "xml", "pg_catalog", -1, false);
        //    var typeInfoForOid143 = new TypeInfo(143U, "_xml", "pg_catalog", -1, false);
        //    var typeInfoForOid194 = new TypeInfo(194U, "pg_node_tree", "pg_catalog", -1, false);
        //    var typeInfoForOid199 = new TypeInfo(199U, "_json", "pg_catalog", -1, false);
        //    var typeInfoForOid600 = new TypeInfo(600U, "point", "pg_catalog", 16, false);
        //    var typeInfoForOid601 = new TypeInfo(601U, "lseg", "pg_catalog", 32, false);
        //    var typeInfoForOid602 = new TypeInfo(602U, "path", "pg_catalog", -1, false);
        //    var typeInfoForOid603 = new TypeInfo(603U, "box", "pg_catalog", 32, false);
        //    var typeInfoForOid604 = new TypeInfo(604U, "polygon", "pg_catalog", -1, false);
        //    var typeInfoForOid628 = new TypeInfo(628U, "line", "pg_catalog", 24, false);
        //    var typeInfoForOid629 = new TypeInfo(629U, "_line", "pg_catalog", -1, false);
        //    var typeInfoForOid650 = new TypeInfo(650U, "cidr", "pg_catalog", -1, false);
        //    var typeInfoForOid651 = new TypeInfo(651U, "_cidr", "pg_catalog", -1, false);
        //    var typeInfoForOid700 = new TypeInfo(700U, "float4", "pg_catalog", 4, true);
        //    var typeInfoForOid701 = new TypeInfo(701U, "float8", "pg_catalog", 8, true);
        //    var typeInfoForOid718 = new TypeInfo(718U, "circle", "pg_catalog", 24, false);
        //    var typeInfoForOid719 = new TypeInfo(719U, "_circle", "pg_catalog", -1, false);
        //    var typeInfoForOid774 = new TypeInfo(774U, "macaddr8", "pg_catalog", 8, false);
        //    var typeInfoForOid775 = new TypeInfo(775U, "_macaddr8", "pg_catalog", -1, false);
        //    var typeInfoForOid790 = new TypeInfo(790U, "money", "pg_catalog", 8, true);
        //    var typeInfoForOid791 = new TypeInfo(791U, "_money", "pg_catalog", -1, false);
        //    var typeInfoForOid829 = new TypeInfo(829U, "macaddr", "pg_catalog", 6, false);
        //    var typeInfoForOid869 = new TypeInfo(869U, "inet", "pg_catalog", -1, false);
        //    var typeInfoForOid1000 = new TypeInfo(1000U, "_bool", "pg_catalog", -1, false);
        //    var typeInfoForOid1001 = new TypeInfo(1001U, "_bytea", "pg_catalog", -1, false);
        //    var typeInfoForOid1002 = new TypeInfo(1002U, "_char", "pg_catalog", -1, false);
        //    var typeInfoForOid1003 = new TypeInfo(1003U, "_name", "pg_catalog", -1, false);
        //    var typeInfoForOid1005 = new TypeInfo(1005U, "_int2", "pg_catalog", -1, false);
        //    var typeInfoForOid1006 = new TypeInfo(1006U, "_int2vector", "pg_catalog", -1, false);
        //    var typeInfoForOid1007 = new TypeInfo(1007U, "_int4", "pg_catalog", -1, false);
        //    var typeInfoForOid1008 = new TypeInfo(1008U, "_regproc", "pg_catalog", -1, false);
        //    var typeInfoForOid1009 = new TypeInfo(1009U, "_text", "pg_catalog", -1, false);
        //    var typeInfoForOid1010 = new TypeInfo(1010U, "_tid", "pg_catalog", -1, false);
        //    var typeInfoForOid1011 = new TypeInfo(1011U, "_xid", "pg_catalog", -1, false);
        //    var typeInfoForOid1012 = new TypeInfo(1012U, "_cid", "pg_catalog", -1, false);
        //    var typeInfoForOid1013 = new TypeInfo(1013U, "_oidvector", "pg_catalog", -1, false);
        //    var typeInfoForOid1014 = new TypeInfo(1014U, "_bpchar", "pg_catalog", -1, false);
        //    var typeInfoForOid1015 = new TypeInfo(1015U, "_varchar", "pg_catalog", -1, false);
        //    var typeInfoForOid1016 = new TypeInfo(1016U, "_int8", "pg_catalog", -1, false);
        //    var typeInfoForOid1017 = new TypeInfo(1017U, "_point", "pg_catalog", -1, false);
        //    var typeInfoForOid1018 = new TypeInfo(1018U, "_lseg", "pg_catalog", -1, false);
        //    var typeInfoForOid1019 = new TypeInfo(1019U, "_path", "pg_catalog", -1, false);
        //    var typeInfoForOid1020 = new TypeInfo(1020U, "_box", "pg_catalog", -1, false);
        //    var typeInfoForOid1021 = new TypeInfo(1021U, "_float4", "pg_catalog", -1, false);
        //    var typeInfoForOid1022 = new TypeInfo(1022U, "_float8", "pg_catalog", -1, false);
        //    var typeInfoForOid1027 = new TypeInfo(1027U, "_polygon", "pg_catalog", -1, false);
        //    var typeInfoForOid1028 = new TypeInfo(1028U, "_oid", "pg_catalog", -1, false);
        //    var typeInfoForOid1033 = new TypeInfo(1033U, "aclitem", "pg_catalog", 12, false);
        //    var typeInfoForOid1034 = new TypeInfo(1034U, "_aclitem", "pg_catalog", -1, false);
        //    var typeInfoForOid1040 = new TypeInfo(1040U, "_macaddr", "pg_catalog", -1, false);
        //    var typeInfoForOid1041 = new TypeInfo(1041U, "_inet", "pg_catalog", -1, false);
        //    var typeInfoForOid1042 = new TypeInfo(1042U, "bpchar", "pg_catalog", -1, false);
        //    var typeInfoForOid1043 = new TypeInfo(1043U, "varchar", "pg_catalog", -1, false);
        //    var typeInfoForOid1082 = new TypeInfo(1082U, "date", "pg_catalog", 4, true);
        //    var typeInfoForOid1083 = new TypeInfo(1083U, "time", "pg_catalog", 8, true);
        //    var typeInfoForOid1114 = new TypeInfo(1114U, "timestamp", "pg_catalog", 8, true);
        //    var typeInfoForOid1115 = new TypeInfo(1115U, "_timestamp", "pg_catalog", -1, false);
        //    var typeInfoForOid1182 = new TypeInfo(1182U, "_date", "pg_catalog", -1, false);
        //    var typeInfoForOid1183 = new TypeInfo(1183U, "_time", "pg_catalog", -1, false);
        //    var typeInfoForOid1184 = new TypeInfo(1184U, "timestamptz", "pg_catalog", 8, true);
        //    var typeInfoForOid1185 = new TypeInfo(1185U, "_timestamptz", "pg_catalog", -1, false);
        //    var typeInfoForOid1186 = new TypeInfo(1186U, "interval", "pg_catalog", 16, false);
        //    var typeInfoForOid1187 = new TypeInfo(1187U, "_interval", "pg_catalog", -1, false);
        //    var typeInfoForOid1231 = new TypeInfo(1231U, "_numeric", "pg_catalog", -1, false);
        //    var typeInfoForOid1263 = new TypeInfo(1263U, "_cstring", "pg_catalog", -1, false);
        //    var typeInfoForOid1266 = new TypeInfo(1266U, "timetz", "pg_catalog", 12, false);
        //    var typeInfoForOid1270 = new TypeInfo(1270U, "_timetz", "pg_catalog", -1, false);
        //    var typeInfoForOid1560 = new TypeInfo(1560U, "bit", "pg_catalog", -1, false);
        //    var typeInfoForOid1561 = new TypeInfo(1561U, "_bit", "pg_catalog", -1, false);
        //    var typeInfoForOid1562 = new TypeInfo(1562U, "varbit", "pg_catalog", -1, false);
        //    var typeInfoForOid1563 = new TypeInfo(1563U, "_varbit", "pg_catalog", -1, false);
        //    var typeInfoForOid1700 = new TypeInfo(1700U, "numeric", "pg_catalog", -1, false);
        //    var typeInfoForOid1790 = new TypeInfo(1790U, "refcursor", "pg_catalog", -1, false);
        //    var typeInfoForOid2201 = new TypeInfo(2201U, "_refcursor", "pg_catalog", -1, false);
        //    var typeInfoForOid2202 = new TypeInfo(2202U, "regprocedure", "pg_catalog", 4, true);
        //    var typeInfoForOid2203 = new TypeInfo(2203U, "regoper", "pg_catalog", 4, true);
        //    var typeInfoForOid2204 = new TypeInfo(2204U, "regoperator", "pg_catalog", 4, true);
        //    var typeInfoForOid2205 = new TypeInfo(2205U, "regclass", "pg_catalog", 4, true);
        //    var typeInfoForOid2206 = new TypeInfo(2206U, "regtype", "pg_catalog", 4, true);
        //    var typeInfoForOid2207 = new TypeInfo(2207U, "_regprocedure", "pg_catalog", -1, false);
        //    var typeInfoForOid2208 = new TypeInfo(2208U, "_regoper", "pg_catalog", -1, false);
        //    var typeInfoForOid2209 = new TypeInfo(2209U, "_regoperator", "pg_catalog", -1, false);
        //    var typeInfoForOid2210 = new TypeInfo(2210U, "_regclass", "pg_catalog", -1, false);
        //    var typeInfoForOid2211 = new TypeInfo(2211U, "_regtype", "pg_catalog", -1, false);
        //    var typeInfoForOid2949 = new TypeInfo(2949U, "_txid_snapshot", "pg_catalog", -1, false);
        //    var typeInfoForOid2950 = new TypeInfo(2950U, "uuid", "pg_catalog", 16, false);
        //    var typeInfoForOid2951 = new TypeInfo(2951U, "_uuid", "pg_catalog", -1, false);
        //    var typeInfoForOid2970 = new TypeInfo(2970U, "txid_snapshot", "pg_catalog", -1, false);
        //    var typeInfoForOid3220 = new TypeInfo(3220U, "pg_lsn", "pg_catalog", 8, true);
        //    var typeInfoForOid3221 = new TypeInfo(3221U, "_pg_lsn", "pg_catalog", -1, false);
        //    var typeInfoForOid3361 = new TypeInfo(3361U, "pg_ndistinct", "pg_catalog", -1, false);
        //    var typeInfoForOid3402 = new TypeInfo(3402U, "pg_dependencies", "pg_catalog", -1, false);
        //    var typeInfoForOid3614 = new TypeInfo(3614U, "tsvector", "pg_catalog", -1, false);
        //    var typeInfoForOid3615 = new TypeInfo(3615U, "tsquery", "pg_catalog", -1, false);
        //    var typeInfoForOid3642 = new TypeInfo(3642U, "gtsvector", "pg_catalog", -1, false);
        //    var typeInfoForOid3643 = new TypeInfo(3643U, "_tsvector", "pg_catalog", -1, false);
        //    var typeInfoForOid3644 = new TypeInfo(3644U, "_gtsvector", "pg_catalog", -1, false);
        //    var typeInfoForOid3645 = new TypeInfo(3645U, "_tsquery", "pg_catalog", -1, false);
        //    var typeInfoForOid3734 = new TypeInfo(3734U, "regconfig", "pg_catalog", 4, true);
        //    var typeInfoForOid3735 = new TypeInfo(3735U, "_regconfig", "pg_catalog", -1, false);
        //    var typeInfoForOid3769 = new TypeInfo(3769U, "regdictionary", "pg_catalog", 4, true);
        //    var typeInfoForOid3770 = new TypeInfo(3770U, "_regdictionary", "pg_catalog", -1, false);
        //    var typeInfoForOid3802 = new TypeInfo(3802U, "jsonb", "pg_catalog", -1, false);
        //    var typeInfoForOid3807 = new TypeInfo(3807U, "_jsonb", "pg_catalog", -1, false);
        //    var typeInfoForOid3905 = new TypeInfo(3905U, "_int4range", "pg_catalog", -1, false);
        //    var typeInfoForOid3907 = new TypeInfo(3907U, "_numrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3909 = new TypeInfo(3909U, "_tsrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3911 = new TypeInfo(3911U, "_tstzrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3913 = new TypeInfo(3913U, "_daterange", "pg_catalog", -1, false);
        //    var typeInfoForOid3927 = new TypeInfo(3927U, "_int8range", "pg_catalog", -1, false);
        //    var typeInfoForOid4072 = new TypeInfo(4072U, "jsonpath", "pg_catalog", -1, false);
        //    var typeInfoForOid4073 = new TypeInfo(4073U, "_jsonpath", "pg_catalog", -1, false);
        //    var typeInfoForOid4089 = new TypeInfo(4089U, "regnamespace", "pg_catalog", 4, true);
        //    var typeInfoForOid4090 = new TypeInfo(4090U, "_regnamespace", "pg_catalog", -1, false);
        //    var typeInfoForOid4096 = new TypeInfo(4096U, "regrole", "pg_catalog", 4, true);
        //    var typeInfoForOid4097 = new TypeInfo(4097U, "_regrole", "pg_catalog", -1, false);
        //    var typeInfoForOid5017 = new TypeInfo(5017U, "pg_mcv_list", "pg_catalog", -1, false);
        //    var typeInfoForOid12381 = new TypeInfo(12381U, "_cardinal_number", "information_schema", -1, false);
        //    var typeInfoForOid12384 = new TypeInfo(12384U, "_character_data", "information_schema", -1, false);
        //    var typeInfoForOid12386 = new TypeInfo(12386U, "_sql_identifier", "information_schema", -1, false);
        //    var typeInfoForOid12391 = new TypeInfo(12391U, "_time_stamp", "information_schema", -1, false);
        //    var typeInfoForOid12393 = new TypeInfo(12393U, "_yes_or_no", "information_schema", -1, false);
        //    var typeInfoForOid32 = new TypeInfo(32U, "pg_ddl_command", "pg_catalog", 8, true);
        //    var typeInfoForOid269 = new TypeInfo(269U, "table_am_handler", "pg_catalog", 4, true);
        //    var typeInfoForOid325 = new TypeInfo(325U, "index_am_handler", "pg_catalog", 4, true);
        //    var typeInfoForOid705 = new TypeInfo(705U, "unknown", "pg_catalog", -2, false);
        //    var typeInfoForOid2249 = new TypeInfo(2249U, "record", "pg_catalog", -1, false);
        //    var typeInfoForOid2275 = new TypeInfo(2275U, "cstring", "pg_catalog", -2, false);
        //    var typeInfoForOid2276 = new TypeInfo(2276U, "any", "pg_catalog", 4, true);
        //    var typeInfoForOid2277 = new TypeInfo(2277U, "anyarray", "pg_catalog", -1, false);
        //    var typeInfoForOid2278 = new TypeInfo(2278U, "void", "pg_catalog", 4, true);
        //    var typeInfoForOid2279 = new TypeInfo(2279U, "trigger", "pg_catalog", 4, true);
        //    var typeInfoForOid2280 = new TypeInfo(2280U, "language_handler", "pg_catalog", 4, true);
        //    var typeInfoForOid2281 = new TypeInfo(2281U, "internal", "pg_catalog", 8, true);
        //    var typeInfoForOid2282 = new TypeInfo(2282U, "opaque", "pg_catalog", 4, true);
        //    var typeInfoForOid2283 = new TypeInfo(2283U, "anyelement", "pg_catalog", 4, true);
        //    var typeInfoForOid2287 = new TypeInfo(2287U, "_record", "pg_catalog", -1, false);
        //    var typeInfoForOid2776 = new TypeInfo(2776U, "anynonarray", "pg_catalog", 4, true);
        //    var typeInfoForOid3115 = new TypeInfo(3115U, "fdw_handler", "pg_catalog", 4, true);
        //    var typeInfoForOid3310 = new TypeInfo(3310U, "tsm_handler", "pg_catalog", 4, true);
        //    var typeInfoForOid3500 = new TypeInfo(3500U, "anyenum", "pg_catalog", 4, true);
        //    var typeInfoForOid3831 = new TypeInfo(3831U, "anyrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3838 = new TypeInfo(3838U, "event_trigger", "pg_catalog", 4, true);
        //    var typeInfoForOid12382 = new TypeInfo(12382U, "cardinal_number", "information_schema", 4, true);
        //    var typeInfoForOid12385 = new TypeInfo(12385U, "character_data", "information_schema", -1, false);
        //    var typeInfoForOid12387 = new TypeInfo(12387U, "sql_identifier", "information_schema", 64, false);
        //    var typeInfoForOid12392 = new TypeInfo(12392U, "time_stamp", "information_schema", 8, true);
        //    var typeInfoForOid12394 = new TypeInfo(12394U, "yes_or_no", "information_schema", -1, false);
        //    var typeInfoForOid3904 = new TypeInfo(3904U, "int4range", "pg_catalog", -1, false);
        //    var typeInfoForOid3906 = new TypeInfo(3906U, "numrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3908 = new TypeInfo(3908U, "tsrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3910 = new TypeInfo(3910U, "tstzrange", "pg_catalog", -1, false);
        //    var typeInfoForOid3912 = new TypeInfo(3912U, "daterange", "pg_catalog", -1, false);
        //    var typeInfoForOid3926 = new TypeInfo(3926U, "int8range", "pg_catalog", -1, false);
        //    var typeInfoForOid71 = new CompositeTypeInfo(71U, "pg_type", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("typmodout", 20, typeInfoForOid24, true),
        //            new AttributeInfo("typcollation", 28, typeInfoForOid26, true),
        //            new AttributeInfo("typsend", 18, typeInfoForOid24, true),
        //            new AttributeInfo("typreceive", 17, typeInfoForOid24, true),
        //            new AttributeInfo("typoutput", 16, typeInfoForOid24, true),
        //            new AttributeInfo("typinput", 15, typeInfoForOid24, true),
        //            new AttributeInfo("typarray", 14, typeInfoForOid26, true),
        //            new AttributeInfo("typelem", 13, typeInfoForOid26, true),
        //            new AttributeInfo("typrelid", 12, typeInfoForOid26, true),
        //            new AttributeInfo("typdelim", 11, typeInfoForOid18, true),
        //            new AttributeInfo("typisdefined", 10, typeInfoForOid16, true),
        //            new AttributeInfo("typispreferred", 9, typeInfoForOid16, true),
        //            new AttributeInfo("typcategory", 8, typeInfoForOid18, true),
        //            new AttributeInfo("typtype", 7, typeInfoForOid18, true),
        //            new AttributeInfo("typbyval", 6, typeInfoForOid16, true),
        //            new AttributeInfo("typlen", 5, typeInfoForOid21, true),
        //            new AttributeInfo("typowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("typnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("typname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("typmodin", 19, typeInfoForOid24, true),
        //            new AttributeInfo("typdefaultbin", 29, typeInfoForOid194, false),
        //            new AttributeInfo("typdefault", 30, typeInfoForOid25, false),
        //            new AttributeInfo("typacl", 31, typeInfoForOid1034, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("typtypmod", 26, typeInfoForOid23, true),
        //            new AttributeInfo("typbasetype", 25, typeInfoForOid26, true),
        //            new AttributeInfo("typnotnull", 24, typeInfoForOid16, true),
        //            new AttributeInfo("typstorage", 23, typeInfoForOid18, true),
        //            new AttributeInfo("typalign", 22, typeInfoForOid18, true),
        //            new AttributeInfo("typanalyze", 21, typeInfoForOid24, true),
        //            new AttributeInfo("typndims", 27, typeInfoForOid23, true)
        //        });
        //    var typeInfoForOid75 = new CompositeTypeInfo(75U, "pg_attribute", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("attmissingval", 25, typeInfoForOid2277, false),
        //            new AttributeInfo("attfdwoptions", 24, typeInfoForOid1009, false),
        //            new AttributeInfo("attoptions", 23, typeInfoForOid1009, false),
        //            new AttributeInfo("attacl", 22, typeInfoForOid1034, false),
        //            new AttributeInfo("attcollation", 21, typeInfoForOid26, true),
        //            new AttributeInfo("attinhcount", 20, typeInfoForOid23, true),
        //            new AttributeInfo("attislocal", 19, typeInfoForOid16, true),
        //            new AttributeInfo("attisdropped", 18, typeInfoForOid16, true),
        //            new AttributeInfo("attgenerated", 17, typeInfoForOid18, true),
        //            new AttributeInfo("attidentity", 16, typeInfoForOid18, true),
        //            new AttributeInfo("atthasmissing", 15, typeInfoForOid16, true),
        //            new AttributeInfo("atthasdef", 14, typeInfoForOid16, true),
        //            new AttributeInfo("attnotnull", 13, typeInfoForOid16, true),
        //            new AttributeInfo("attalign", 12, typeInfoForOid18, true),
        //            new AttributeInfo("attstorage", 11, typeInfoForOid18, true),
        //            new AttributeInfo("attbyval", 10, typeInfoForOid16, true),
        //            new AttributeInfo("atttypmod", 9, typeInfoForOid23, true),
        //            new AttributeInfo("attcacheoff", 8, typeInfoForOid23, true),
        //            new AttributeInfo("attndims", 7, typeInfoForOid23, true),
        //            new AttributeInfo("attnum", 6, typeInfoForOid21, true),
        //            new AttributeInfo("attlen", 5, typeInfoForOid21, true),
        //            new AttributeInfo("attstattarget", 4, typeInfoForOid23, true),
        //            new AttributeInfo("atttypid", 3, typeInfoForOid26, true),
        //            new AttributeInfo("attname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("attrelid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid81 = new CompositeTypeInfo(81U, "pg_proc", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("proisstrict", 13, typeInfoForOid16, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("proacl", 29, typeInfoForOid1034, false),
        //            new AttributeInfo("proconfig", 28, typeInfoForOid1009, false),
        //            new AttributeInfo("probin", 27, typeInfoForOid25, false),
        //            new AttributeInfo("prosrc", 26, typeInfoForOid25, true),
        //            new AttributeInfo("protrftypes", 25, typeInfoForOid1028, false),
        //            new AttributeInfo("proargdefaults", 24, typeInfoForOid194, false),
        //            new AttributeInfo("proargnames", 23, typeInfoForOid1009, false),
        //            new AttributeInfo("proargmodes", 22, typeInfoForOid1002, false),
        //            new AttributeInfo("proallargtypes", 21, typeInfoForOid1028, false),
        //            new AttributeInfo("proargtypes", 20, typeInfoForOid30, true),
        //            new AttributeInfo("prorettype", 19, typeInfoForOid26, true),
        //            new AttributeInfo("pronargdefaults", 18, typeInfoForOid21, true),
        //            new AttributeInfo("pronargs", 17, typeInfoForOid21, true),
        //            new AttributeInfo("proparallel", 16, typeInfoForOid18, true),
        //            new AttributeInfo("provolatile", 15, typeInfoForOid18, true),
        //            new AttributeInfo("proretset", 14, typeInfoForOid16, true),
        //            new AttributeInfo("proleakproof", 12, typeInfoForOid16, true),
        //            new AttributeInfo("prosecdef", 11, typeInfoForOid16, true),
        //            new AttributeInfo("prokind", 10, typeInfoForOid18, true),
        //            new AttributeInfo("prosupport", 9, typeInfoForOid24, true),
        //            new AttributeInfo("provariadic", 8, typeInfoForOid26, true),
        //            new AttributeInfo("prorows", 7, typeInfoForOid700, true),
        //            new AttributeInfo("procost", 6, typeInfoForOid700, true),
        //            new AttributeInfo("prolang", 5, typeInfoForOid26, true),
        //            new AttributeInfo("proowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("pronamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("proname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid83 = new CompositeTypeInfo(83U, "pg_class", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("relnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("relpartbound", 33, typeInfoForOid194, false),
        //            new AttributeInfo("reloptions", 32, typeInfoForOid1009, false),
        //            new AttributeInfo("relacl", 31, typeInfoForOid1034, false),
        //            new AttributeInfo("relminmxid", 30, typeInfoForOid28, true),
        //            new AttributeInfo("relfrozenxid", 29, typeInfoForOid28, true),
        //            new AttributeInfo("relrewrite", 28, typeInfoForOid26, true),
        //            new AttributeInfo("relispartition", 27, typeInfoForOid16, true),
        //            new AttributeInfo("relreplident", 26, typeInfoForOid18, true),
        //            new AttributeInfo("relispopulated", 25, typeInfoForOid16, true),
        //            new AttributeInfo("relforcerowsecurity", 24, typeInfoForOid16, true),
        //            new AttributeInfo("relrowsecurity", 23, typeInfoForOid16, true),
        //            new AttributeInfo("relhassubclass", 22, typeInfoForOid16, true),
        //            new AttributeInfo("relhastriggers", 21, typeInfoForOid16, true),
        //            new AttributeInfo("relhasrules", 20, typeInfoForOid16, true),
        //            new AttributeInfo("relchecks", 19, typeInfoForOid21, true),
        //            new AttributeInfo("relnatts", 18, typeInfoForOid21, true),
        //            new AttributeInfo("relkind", 17, typeInfoForOid18, true),
        //            new AttributeInfo("relpersistence", 16, typeInfoForOid18, true),
        //            new AttributeInfo("relisshared", 15, typeInfoForOid16, true),
        //            new AttributeInfo("relhasindex", 14, typeInfoForOid16, true),
        //            new AttributeInfo("reltoastrelid", 13, typeInfoForOid26, true),
        //            new AttributeInfo("relallvisible", 12, typeInfoForOid23, true),
        //            new AttributeInfo("reltuples", 11, typeInfoForOid700, true),
        //            new AttributeInfo("relpages", 10, typeInfoForOid23, true),
        //            new AttributeInfo("reltablespace", 9, typeInfoForOid26, true),
        //            new AttributeInfo("relfilenode", 8, typeInfoForOid26, true),
        //            new AttributeInfo("relam", 7, typeInfoForOid26, true),
        //            new AttributeInfo("relowner", 6, typeInfoForOid26, true),
        //            new AttributeInfo("reloftype", 5, typeInfoForOid26, true),
        //            new AttributeInfo("reltype", 4, typeInfoForOid26, true),
        //            new AttributeInfo("relname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid1248 = new CompositeTypeInfo(1248U, "pg_database", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("datname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("datdba", 3, typeInfoForOid26, true),
        //            new AttributeInfo("encoding", 4, typeInfoForOid23, true),
        //            new AttributeInfo("datcollate", 5, typeInfoForOid19, true),
        //            new AttributeInfo("datctype", 6, typeInfoForOid19, true),
        //            new AttributeInfo("datistemplate", 7, typeInfoForOid16, true),
        //            new AttributeInfo("datallowconn", 8, typeInfoForOid16, true),
        //            new AttributeInfo("datconnlimit", 9, typeInfoForOid23, true),
        //            new AttributeInfo("datlastsysoid", 10, typeInfoForOid26, true),
        //            new AttributeInfo("datfrozenxid", 11, typeInfoForOid28, true),
        //            new AttributeInfo("datminmxid", 12, typeInfoForOid28, true),
        //            new AttributeInfo("dattablespace", 13, typeInfoForOid26, true),
        //            new AttributeInfo("datacl", 14, typeInfoForOid1034, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid2842 = new CompositeTypeInfo(2842U, "pg_authid", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("rolconnlimit", 10, typeInfoForOid23, true),
        //            new AttributeInfo("rolbypassrls", 9, typeInfoForOid16, true),
        //            new AttributeInfo("rolreplication", 8, typeInfoForOid16, true),
        //            new AttributeInfo("rolcanlogin", 7, typeInfoForOid16, true),
        //            new AttributeInfo("rolcreatedb", 6, typeInfoForOid16, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("rolcreaterole", 5, typeInfoForOid16, true),
        //            new AttributeInfo("rolinherit", 4, typeInfoForOid16, true),
        //            new AttributeInfo("rolsuper", 3, typeInfoForOid16, true),
        //            new AttributeInfo("rolname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("rolvaliduntil", 12, typeInfoForOid1184, false),
        //            new AttributeInfo("rolpassword", 11, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid2843 = new CompositeTypeInfo(2843U, "pg_auth_members", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("roleid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("member", 2, typeInfoForOid26, true),
        //            new AttributeInfo("grantor", 3, typeInfoForOid26, true),
        //            new AttributeInfo("admin_option", 4, typeInfoForOid16, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid4066 = new CompositeTypeInfo(4066U, "pg_shseclabel", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("label", 4, typeInfoForOid25, true),
        //            new AttributeInfo("provider", 3, typeInfoForOid25, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid6101 = new CompositeTypeInfo(6101U, "pg_subscription", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("subenabled", 5, typeInfoForOid16, true),
        //            new AttributeInfo("subowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("subconninfo", 6, typeInfoForOid25, true),
        //            new AttributeInfo("subsynccommit", 8, typeInfoForOid25, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("subdbid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("subname", 3, typeInfoForOid19, true),
        //            new AttributeInfo("subpublications", 9, typeInfoForOid1009, true),
        //            new AttributeInfo("subslotname", 7, typeInfoForOid19, true)
        //        });
        //    var typeInfoForOid12000 = new CompositeTypeInfo(12000U, "pg_attrdef", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("adbin", 4, typeInfoForOid194, true),
        //            new AttributeInfo("adnum", 3, typeInfoForOid21, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("adrelid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12001 = new CompositeTypeInfo(12001U, "pg_constraint", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("conindid", 10, typeInfoForOid26, true),
        //            new AttributeInfo("conparentid", 11, typeInfoForOid26, true),
        //            new AttributeInfo("confrelid", 12, typeInfoForOid26, true),
        //            new AttributeInfo("confupdtype", 13, typeInfoForOid18, true),
        //            new AttributeInfo("confdeltype", 14, typeInfoForOid18, true),
        //            new AttributeInfo("confmatchtype", 15, typeInfoForOid18, true),
        //            new AttributeInfo("conislocal", 16, typeInfoForOid16, true),
        //            new AttributeInfo("coninhcount", 17, typeInfoForOid23, true),
        //            new AttributeInfo("connoinherit", 18, typeInfoForOid16, true),
        //            new AttributeInfo("conkey", 19, typeInfoForOid1005, false),
        //            new AttributeInfo("confkey", 20, typeInfoForOid1005, false),
        //            new AttributeInfo("conpfeqop", 21, typeInfoForOid1028, false),
        //            new AttributeInfo("conppeqop", 22, typeInfoForOid1028, false),
        //            new AttributeInfo("conffeqop", 23, typeInfoForOid1028, false),
        //            new AttributeInfo("conexclop", 24, typeInfoForOid1028, false),
        //            new AttributeInfo("conbin", 25, typeInfoForOid194, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("conname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("connamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("contype", 4, typeInfoForOid18, true),
        //            new AttributeInfo("condeferrable", 5, typeInfoForOid16, true),
        //            new AttributeInfo("condeferred", 6, typeInfoForOid16, true),
        //            new AttributeInfo("convalidated", 7, typeInfoForOid16, true),
        //            new AttributeInfo("conrelid", 8, typeInfoForOid26, true),
        //            new AttributeInfo("contypid", 9, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12002 = new CompositeTypeInfo(12002U, "pg_inherits", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("inhparent", 2, typeInfoForOid26, true),
        //            new AttributeInfo("inhrelid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("inhseqno", 3, typeInfoForOid23, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true)
        //        });
        //    var typeInfoForOid12003 = new CompositeTypeInfo(12003U, "pg_index", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("indnatts", 3, typeInfoForOid21, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("indpred", 20, typeInfoForOid194, false),
        //            new AttributeInfo("indexprs", 19, typeInfoForOid194, false),
        //            new AttributeInfo("indoption", 18, typeInfoForOid22, true),
        //            new AttributeInfo("indclass", 17, typeInfoForOid30, true),
        //            new AttributeInfo("indcollation", 16, typeInfoForOid30, true),
        //            new AttributeInfo("indkey", 15, typeInfoForOid22, true),
        //            new AttributeInfo("indisreplident", 14, typeInfoForOid16, true),
        //            new AttributeInfo("indislive", 13, typeInfoForOid16, true),
        //            new AttributeInfo("indisready", 12, typeInfoForOid16, true),
        //            new AttributeInfo("indcheckxmin", 11, typeInfoForOid16, true),
        //            new AttributeInfo("indisvalid", 10, typeInfoForOid16, true),
        //            new AttributeInfo("indisclustered", 9, typeInfoForOid16, true),
        //            new AttributeInfo("indimmediate", 8, typeInfoForOid16, true),
        //            new AttributeInfo("indisexclusion", 7, typeInfoForOid16, true),
        //            new AttributeInfo("indisprimary", 6, typeInfoForOid16, true),
        //            new AttributeInfo("indisunique", 5, typeInfoForOid16, true),
        //            new AttributeInfo("indnkeyatts", 4, typeInfoForOid21, true),
        //            new AttributeInfo("indrelid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("indexrelid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12004 = new CompositeTypeInfo(12004U, "pg_operator", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oprjoin", 15, typeInfoForOid24, true),
        //            new AttributeInfo("oprrest", 14, typeInfoForOid24, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("oprcode", 13, typeInfoForOid24, true),
        //            new AttributeInfo("oprnegate", 12, typeInfoForOid26, true),
        //            new AttributeInfo("oprcom", 11, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("oprname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oprnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("oprowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("oprkind", 5, typeInfoForOid18, true),
        //            new AttributeInfo("oprresult", 10, typeInfoForOid26, true),
        //            new AttributeInfo("oprright", 9, typeInfoForOid26, true),
        //            new AttributeInfo("oprleft", 8, typeInfoForOid26, true),
        //            new AttributeInfo("oprcanhash", 7, typeInfoForOid16, true),
        //            new AttributeInfo("oprcanmerge", 6, typeInfoForOid16, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true)
        //        });
        //    var typeInfoForOid12005 = new CompositeTypeInfo(12005U, "pg_opfamily", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("opfmethod", 2, typeInfoForOid26, true),
        //            new AttributeInfo("opfname", 3, typeInfoForOid19, true),
        //            new AttributeInfo("opfnamespace", 4, typeInfoForOid26, true),
        //            new AttributeInfo("opfowner", 5, typeInfoForOid26, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12006 = new CompositeTypeInfo(12006U, "pg_opclass", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("opckeytype", 9, typeInfoForOid26, true),
        //            new AttributeInfo("opcdefault", 8, typeInfoForOid16, true),
        //            new AttributeInfo("opcintype", 7, typeInfoForOid26, true),
        //            new AttributeInfo("opcfamily", 6, typeInfoForOid26, true),
        //            new AttributeInfo("opcowner", 5, typeInfoForOid26, true),
        //            new AttributeInfo("opcnamespace", 4, typeInfoForOid26, true),
        //            new AttributeInfo("opcname", 3, typeInfoForOid19, true),
        //            new AttributeInfo("opcmethod", 2, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12007 = new CompositeTypeInfo(12007U, "pg_am", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("amtype", 4, typeInfoForOid18, true),
        //            new AttributeInfo("amhandler", 3, typeInfoForOid24, true),
        //            new AttributeInfo("amname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12008 = new CompositeTypeInfo(12008U, "pg_amop", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("amopmethod", 8, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("amopsortfamily", 9, typeInfoForOid26, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("amopopr", 7, typeInfoForOid26, true),
        //            new AttributeInfo("amoppurpose", 6, typeInfoForOid18, true),
        //            new AttributeInfo("amopstrategy", 5, typeInfoForOid21, true),
        //            new AttributeInfo("amoprighttype", 4, typeInfoForOid26, true),
        //            new AttributeInfo("amoplefttype", 3, typeInfoForOid26, true),
        //            new AttributeInfo("amopfamily", 2, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12009 = new CompositeTypeInfo(12009U, "pg_amproc", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("amprocfamily", 2, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("amproclefttype", 3, typeInfoForOid26, true),
        //            new AttributeInfo("amprocrighttype", 4, typeInfoForOid26, true),
        //            new AttributeInfo("amprocnum", 5, typeInfoForOid21, true),
        //            new AttributeInfo("amproc", 6, typeInfoForOid24, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12010 = new CompositeTypeInfo(12010U, "pg_language", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("lanplcallfoid", 6, typeInfoForOid26, true),
        //            new AttributeInfo("lanpltrusted", 5, typeInfoForOid16, true),
        //            new AttributeInfo("laninline", 7, typeInfoForOid26, true),
        //            new AttributeInfo("lanispl", 4, typeInfoForOid16, true),
        //            new AttributeInfo("lanowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("lanname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("lanvalidator", 8, typeInfoForOid26, true),
        //            new AttributeInfo("lanacl", 9, typeInfoForOid1034, false)
        //        });
        //    var typeInfoForOid12011 = new CompositeTypeInfo(12011U, "pg_largeobject_metadata", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("lomacl", 3, typeInfoForOid1034, false),
        //            new AttributeInfo("lomowner", 2, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12012 = new CompositeTypeInfo(12012U, "pg_largeobject", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("data", 3, typeInfoForOid17, true),
        //            new AttributeInfo("pageno", 2, typeInfoForOid23, true),
        //            new AttributeInfo("loid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12013 = new CompositeTypeInfo(12013U, "pg_aggregate", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("aggtranstype", 17, typeInfoForOid26, true),
        //            new AttributeInfo("aggfinalmodify", 14, typeInfoForOid18, true),
        //            new AttributeInfo("aggmfinalmodify", 15, typeInfoForOid18, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("aggminitval", 22, typeInfoForOid25, false),
        //            new AttributeInfo("agginitval", 21, typeInfoForOid25, false),
        //            new AttributeInfo("aggmtransspace", 20, typeInfoForOid23, true),
        //            new AttributeInfo("aggmtranstype", 19, typeInfoForOid26, true),
        //            new AttributeInfo("aggtransspace", 18, typeInfoForOid23, true),
        //            new AttributeInfo("aggsortop", 16, typeInfoForOid26, true),
        //            new AttributeInfo("aggfnoid", 1, typeInfoForOid24, true),
        //            new AttributeInfo("aggkind", 2, typeInfoForOid18, true),
        //            new AttributeInfo("aggnumdirectargs", 3, typeInfoForOid21, true),
        //            new AttributeInfo("aggtransfn", 4, typeInfoForOid24, true),
        //            new AttributeInfo("aggfinalfn", 5, typeInfoForOid24, true),
        //            new AttributeInfo("aggcombinefn", 6, typeInfoForOid24, true),
        //            new AttributeInfo("aggserialfn", 7, typeInfoForOid24, true),
        //            new AttributeInfo("aggdeserialfn", 8, typeInfoForOid24, true),
        //            new AttributeInfo("aggmtransfn", 9, typeInfoForOid24, true),
        //            new AttributeInfo("aggminvtransfn", 10, typeInfoForOid24, true),
        //            new AttributeInfo("aggmfinalfn", 11, typeInfoForOid24, true),
        //            new AttributeInfo("aggfinalextra", 12, typeInfoForOid16, true),
        //            new AttributeInfo("aggmfinalextra", 13, typeInfoForOid16, true)
        //        });
        //    var typeInfoForOid12014 = new CompositeTypeInfo(12014U, "pg_statistic_ext", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("stxrelid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("stxname", 3, typeInfoForOid19, true),
        //            new AttributeInfo("stxnamespace", 4, typeInfoForOid26, true),
        //            new AttributeInfo("stxowner", 5, typeInfoForOid26, true),
        //            new AttributeInfo("stxstattarget", 6, typeInfoForOid23, true),
        //            new AttributeInfo("stxkeys", 7, typeInfoForOid22, true),
        //            new AttributeInfo("stxkind", 8, typeInfoForOid1002, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12015 = new CompositeTypeInfo(12015U, "pg_statistic_ext_data", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("stxdmcv", 4, typeInfoForOid5017, false),
        //            new AttributeInfo("stxddependencies", 3, typeInfoForOid3402, false),
        //            new AttributeInfo("stxdndistinct", 2, typeInfoForOid3361, false),
        //            new AttributeInfo("stxoid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12016 = new CompositeTypeInfo(12016U, "pg_statistic", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("stakind5", 11, typeInfoForOid21, true),
        //            new AttributeInfo("staop1", 12, typeInfoForOid26, true),
        //            new AttributeInfo("staop2", 13, typeInfoForOid26, true),
        //            new AttributeInfo("staop3", 14, typeInfoForOid26, true),
        //            new AttributeInfo("staop4", 15, typeInfoForOid26, true),
        //            new AttributeInfo("starelid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("staattnum", 2, typeInfoForOid21, true),
        //            new AttributeInfo("stacoll1", 17, typeInfoForOid26, true),
        //            new AttributeInfo("stacoll2", 18, typeInfoForOid26, true),
        //            new AttributeInfo("stacoll3", 19, typeInfoForOid26, true),
        //            new AttributeInfo("stacoll4", 20, typeInfoForOid26, true),
        //            new AttributeInfo("stacoll5", 21, typeInfoForOid26, true),
        //            new AttributeInfo("stanumbers1", 22, typeInfoForOid1021, false),
        //            new AttributeInfo("stanumbers2", 23, typeInfoForOid1021, false),
        //            new AttributeInfo("stanumbers3", 24, typeInfoForOid1021, false),
        //            new AttributeInfo("stanumbers4", 25, typeInfoForOid1021, false),
        //            new AttributeInfo("stanumbers5", 26, typeInfoForOid1021, false),
        //            new AttributeInfo("stavalues1", 27, typeInfoForOid2277, false),
        //            new AttributeInfo("stavalues2", 28, typeInfoForOid2277, false),
        //            new AttributeInfo("stavalues3", 29, typeInfoForOid2277, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("stavalues5", 31, typeInfoForOid2277, false),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("stavalues4", 30, typeInfoForOid2277, false),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("stainherit", 3, typeInfoForOid16, true),
        //            new AttributeInfo("stanullfrac", 4, typeInfoForOid700, true),
        //            new AttributeInfo("stawidth", 5, typeInfoForOid23, true),
        //            new AttributeInfo("stadistinct", 6, typeInfoForOid700, true),
        //            new AttributeInfo("stakind1", 7, typeInfoForOid21, true),
        //            new AttributeInfo("stakind2", 8, typeInfoForOid21, true),
        //            new AttributeInfo("stakind3", 9, typeInfoForOid21, true),
        //            new AttributeInfo("staop5", 16, typeInfoForOid26, true),
        //            new AttributeInfo("stakind4", 10, typeInfoForOid21, true)
        //        });
        //    var typeInfoForOid12017 = new CompositeTypeInfo(12017U, "pg_rewrite", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ev_action", 8, typeInfoForOid194, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("ev_qual", 7, typeInfoForOid194, true),
        //            new AttributeInfo("is_instead", 6, typeInfoForOid16, true),
        //            new AttributeInfo("ev_enabled", 5, typeInfoForOid18, true),
        //            new AttributeInfo("ev_type", 4, typeInfoForOid18, true),
        //            new AttributeInfo("ev_class", 3, typeInfoForOid26, true),
        //            new AttributeInfo("rulename", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12018 = new CompositeTypeInfo(12018U, "pg_trigger", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("tgnewtable", 18, typeInfoForOid19, false),
        //            new AttributeInfo("tgconstrrelid", 8, typeInfoForOid26, true),
        //            new AttributeInfo("tgisinternal", 7, typeInfoForOid16, true),
        //            new AttributeInfo("tgenabled", 6, typeInfoForOid18, true),
        //            new AttributeInfo("tgtype", 5, typeInfoForOid21, true),
        //            new AttributeInfo("tgfoid", 4, typeInfoForOid26, true),
        //            new AttributeInfo("tgname", 3, typeInfoForOid19, true),
        //            new AttributeInfo("tgrelid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tgoldtable", 17, typeInfoForOid19, false),
        //            new AttributeInfo("tgqual", 16, typeInfoForOid194, false),
        //            new AttributeInfo("tgargs", 15, typeInfoForOid17, true),
        //            new AttributeInfo("tgattr", 14, typeInfoForOid22, true),
        //            new AttributeInfo("tgnargs", 13, typeInfoForOid21, true),
        //            new AttributeInfo("tginitdeferred", 12, typeInfoForOid16, true),
        //            new AttributeInfo("tgdeferrable", 11, typeInfoForOid16, true),
        //            new AttributeInfo("tgconstraint", 10, typeInfoForOid26, true),
        //            new AttributeInfo("tgconstrindid", 9, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12019 = new CompositeTypeInfo(12019U, "pg_event_trigger", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("evtenabled", 6, typeInfoForOid18, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("evtname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("evtevent", 3, typeInfoForOid19, true),
        //            new AttributeInfo("evtowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("evtfoid", 5, typeInfoForOid26, true),
        //            new AttributeInfo("evttags", 7, typeInfoForOid1009, false),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12020 = new CompositeTypeInfo(12020U, "pg_description", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("objsubid", 3, typeInfoForOid23, true),
        //            new AttributeInfo("description", 4, typeInfoForOid25, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12021 = new CompositeTypeInfo(12021U, "pg_cast", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("castmethod", 6, typeInfoForOid18, true),
        //            new AttributeInfo("castcontext", 5, typeInfoForOid18, true),
        //            new AttributeInfo("castfunc", 4, typeInfoForOid26, true),
        //            new AttributeInfo("casttarget", 3, typeInfoForOid26, true),
        //            new AttributeInfo("castsource", 2, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12022 = new CompositeTypeInfo(12022U, "pg_enum", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("enumtypid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("enumlabel", 4, typeInfoForOid19, true),
        //            new AttributeInfo("enumsortorder", 3, typeInfoForOid700, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12023 = new CompositeTypeInfo(12023U, "pg_namespace", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("nspname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("nspowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("nspacl", 4, typeInfoForOid1034, false),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12024 = new CompositeTypeInfo(12024U, "pg_conversion", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("connamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("condefault", 8, typeInfoForOid16, true),
        //            new AttributeInfo("conproc", 7, typeInfoForOid24, true),
        //            new AttributeInfo("contoencoding", 6, typeInfoForOid23, true),
        //            new AttributeInfo("conforencoding", 5, typeInfoForOid23, true),
        //            new AttributeInfo("conowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("conname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12025 = new CompositeTypeInfo(12025U, "pg_depend", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("classid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("refclassid", 4, typeInfoForOid26, true),
        //            new AttributeInfo("objsubid", 3, typeInfoForOid23, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("refobjid", 5, typeInfoForOid26, true),
        //            new AttributeInfo("refobjsubid", 6, typeInfoForOid23, true),
        //            new AttributeInfo("deptype", 7, typeInfoForOid18, true),
        //            new AttributeInfo("objid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12026 = new CompositeTypeInfo(12026U, "pg_db_role_setting", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("setconfig", 3, typeInfoForOid1009, false),
        //            new AttributeInfo("setrole", 2, typeInfoForOid26, true),
        //            new AttributeInfo("setdatabase", 1, typeInfoForOid26, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12027 = new CompositeTypeInfo(12027U, "pg_tablespace", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("spcname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("spcowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("spcacl", 4, typeInfoForOid1034, false),
        //            new AttributeInfo("spcoptions", 5, typeInfoForOid1009, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12028 = new CompositeTypeInfo(12028U, "pg_pltemplate", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("tmplname", 1, typeInfoForOid19, true),
        //            new AttributeInfo("tmpltrusted", 2, typeInfoForOid16, true),
        //            new AttributeInfo("tmpldbacreate", 3, typeInfoForOid16, true),
        //            new AttributeInfo("tmplhandler", 4, typeInfoForOid25, true),
        //            new AttributeInfo("tmplinline", 5, typeInfoForOid25, false),
        //            new AttributeInfo("tmplvalidator", 6, typeInfoForOid25, false),
        //            new AttributeInfo("tmpllibrary", 7, typeInfoForOid25, true),
        //            new AttributeInfo("tmplacl", 8, typeInfoForOid1034, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12029 = new CompositeTypeInfo(12029U, "pg_shdepend", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("deptype", 7, typeInfoForOid18, true),
        //            new AttributeInfo("refobjid", 6, typeInfoForOid26, true),
        //            new AttributeInfo("refclassid", 5, typeInfoForOid26, true),
        //            new AttributeInfo("objsubid", 4, typeInfoForOid23, true),
        //            new AttributeInfo("objid", 3, typeInfoForOid26, true),
        //            new AttributeInfo("classid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("dbid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12030 = new CompositeTypeInfo(12030U, "pg_shdescription", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("description", 3, typeInfoForOid25, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12031 = new CompositeTypeInfo(12031U, "pg_ts_config", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("cfgparser", 5, typeInfoForOid26, true),
        //            new AttributeInfo("cfgowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("cfgnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("cfgname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12032 = new CompositeTypeInfo(12032U, "pg_ts_config_map", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("mapcfg", 1, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("maptokentype", 2, typeInfoForOid23, true),
        //            new AttributeInfo("mapseqno", 3, typeInfoForOid23, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("mapdict", 4, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12033 = new CompositeTypeInfo(12033U, "pg_ts_dict", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("dictnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("dictinitoption", 6, typeInfoForOid25, false),
        //            new AttributeInfo("dicttemplate", 5, typeInfoForOid26, true),
        //            new AttributeInfo("dictowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("dictname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12034 = new CompositeTypeInfo(12034U, "pg_ts_parser", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("prsnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("prsname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("prslextype", 8, typeInfoForOid24, true),
        //            new AttributeInfo("prsheadline", 7, typeInfoForOid24, true),
        //            new AttributeInfo("prsend", 6, typeInfoForOid24, true),
        //            new AttributeInfo("prstoken", 5, typeInfoForOid24, true),
        //            new AttributeInfo("prsstart", 4, typeInfoForOid24, true)
        //        });
        //    var typeInfoForOid12035 = new CompositeTypeInfo(12035U, "pg_ts_template", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tmpllexize", 5, typeInfoForOid24, true),
        //            new AttributeInfo("tmplinit", 4, typeInfoForOid24, true),
        //            new AttributeInfo("tmplnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("tmplname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12036 = new CompositeTypeInfo(12036U, "pg_extension", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("extcondition", 8, typeInfoForOid1009, false),
        //            new AttributeInfo("extconfig", 7, typeInfoForOid1028, false),
        //            new AttributeInfo("extversion", 6, typeInfoForOid25, true),
        //            new AttributeInfo("extrelocatable", 5, typeInfoForOid16, true),
        //            new AttributeInfo("extnamespace", 4, typeInfoForOid26, true),
        //            new AttributeInfo("extowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("extname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12037 = new CompositeTypeInfo(12037U, "pg_foreign_data_wrapper", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("fdwoptions", 7, typeInfoForOid1009, false),
        //            new AttributeInfo("fdwacl", 6, typeInfoForOid1034, false),
        //            new AttributeInfo("fdwvalidator", 5, typeInfoForOid26, true),
        //            new AttributeInfo("fdwhandler", 4, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("fdwname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("fdwowner", 3, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12038 = new CompositeTypeInfo(12038U, "pg_foreign_server", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("srvowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("srvfdw", 4, typeInfoForOid26, true),
        //            new AttributeInfo("srvtype", 5, typeInfoForOid25, false),
        //            new AttributeInfo("srvversion", 6, typeInfoForOid25, false),
        //            new AttributeInfo("srvacl", 7, typeInfoForOid1034, false),
        //            new AttributeInfo("srvoptions", 8, typeInfoForOid1009, false),
        //            new AttributeInfo("srvname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12039 = new CompositeTypeInfo(12039U, "pg_user_mapping", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("umuser", 2, typeInfoForOid26, true),
        //            new AttributeInfo("umserver", 3, typeInfoForOid26, true),
        //            new AttributeInfo("umoptions", 4, typeInfoForOid1009, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12040 = new CompositeTypeInfo(12040U, "pg_foreign_table", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("ftoptions", 3, typeInfoForOid1009, false),
        //            new AttributeInfo("ftserver", 2, typeInfoForOid26, true),
        //            new AttributeInfo("ftrelid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12041 = new CompositeTypeInfo(12041U, "pg_policy", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("polwithcheck", 8, typeInfoForOid194, false),
        //            new AttributeInfo("polqual", 7, typeInfoForOid194, false),
        //            new AttributeInfo("polroles", 6, typeInfoForOid1028, true),
        //            new AttributeInfo("polpermissive", 5, typeInfoForOid16, true),
        //            new AttributeInfo("polcmd", 4, typeInfoForOid18, true),
        //            new AttributeInfo("polrelid", 3, typeInfoForOid26, true),
        //            new AttributeInfo("polname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12042 = new CompositeTypeInfo(12042U, "pg_replication_origin", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("roname", 2, typeInfoForOid25, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("roident", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12043 = new CompositeTypeInfo(12043U, "pg_default_acl", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("defaclrole", 2, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("defaclacl", 5, typeInfoForOid1034, true),
        //            new AttributeInfo("defaclobjtype", 4, typeInfoForOid18, true),
        //            new AttributeInfo("defaclnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12044 = new CompositeTypeInfo(12044U, "pg_init_privs", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("objsubid", 3, typeInfoForOid23, true),
        //            new AttributeInfo("initprivs", 5, typeInfoForOid1034, true),
        //            new AttributeInfo("privtype", 4, typeInfoForOid18, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12045 = new CompositeTypeInfo(12045U, "pg_seclabel", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("label", 5, typeInfoForOid25, true),
        //            new AttributeInfo("provider", 4, typeInfoForOid25, true),
        //            new AttributeInfo("objsubid", 3, typeInfoForOid23, true),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12046 = new CompositeTypeInfo(12046U, "pg_collation", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("collctype", 9, typeInfoForOid19, true),
        //            new AttributeInfo("collversion", 10, typeInfoForOid25, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("collcollate", 8, typeInfoForOid19, true),
        //            new AttributeInfo("collencoding", 7, typeInfoForOid23, true),
        //            new AttributeInfo("collisdeterministic", 6, typeInfoForOid16, true),
        //            new AttributeInfo("collprovider", 5, typeInfoForOid18, true),
        //            new AttributeInfo("collowner", 4, typeInfoForOid26, true),
        //            new AttributeInfo("collnamespace", 3, typeInfoForOid26, true),
        //            new AttributeInfo("collname", 2, typeInfoForOid19, true)
        //        });
        //    var typeInfoForOid12047 = new CompositeTypeInfo(12047U, "pg_partitioned_table", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("partexprs", 8, typeInfoForOid194, false),
        //            new AttributeInfo("partcollation", 7, typeInfoForOid30, true),
        //            new AttributeInfo("partclass", 6, typeInfoForOid30, true),
        //            new AttributeInfo("partattrs", 5, typeInfoForOid22, true),
        //            new AttributeInfo("partdefid", 4, typeInfoForOid26, true),
        //            new AttributeInfo("partnatts", 3, typeInfoForOid21, true),
        //            new AttributeInfo("partstrat", 2, typeInfoForOid18, true),
        //            new AttributeInfo("partrelid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12048 = new CompositeTypeInfo(12048U, "pg_range", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("rngsubdiff", 6, typeInfoForOid24, true),
        //            new AttributeInfo("rngcanonical", 5, typeInfoForOid24, true),
        //            new AttributeInfo("rngsubopc", 4, typeInfoForOid26, true),
        //            new AttributeInfo("rngcollation", 3, typeInfoForOid26, true),
        //            new AttributeInfo("rngsubtype", 2, typeInfoForOid26, true),
        //            new AttributeInfo("rngtypid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true)
        //        });
        //    var typeInfoForOid12049 = new CompositeTypeInfo(12049U, "pg_transform", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("trftosql", 5, typeInfoForOid24, true),
        //            new AttributeInfo("trffromsql", 4, typeInfoForOid24, true),
        //            new AttributeInfo("trflang", 3, typeInfoForOid26, true),
        //            new AttributeInfo("trftype", 2, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12050 = new CompositeTypeInfo(12050U, "pg_sequence", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("seqstart", 3, typeInfoForOid20, true),
        //            new AttributeInfo("seqincrement", 4, typeInfoForOid20, true),
        //            new AttributeInfo("seqmax", 5, typeInfoForOid20, true),
        //            new AttributeInfo("seqmin", 6, typeInfoForOid20, true),
        //            new AttributeInfo("seqcache", 7, typeInfoForOid20, true),
        //            new AttributeInfo("seqcycle", 8, typeInfoForOid16, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("seqtypid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("seqrelid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12051 = new CompositeTypeInfo(12051U, "pg_publication", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("pubname", 2, typeInfoForOid19, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("pubtruncate", 8, typeInfoForOid16, true),
        //            new AttributeInfo("pubdelete", 7, typeInfoForOid16, true),
        //            new AttributeInfo("pubupdate", 6, typeInfoForOid16, true),
        //            new AttributeInfo("pubinsert", 5, typeInfoForOid16, true),
        //            new AttributeInfo("puballtables", 4, typeInfoForOid16, true),
        //            new AttributeInfo("pubowner", 3, typeInfoForOid26, true),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12052 = new CompositeTypeInfo(12052U, "pg_publication_rel", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 1, typeInfoForOid26, true),
        //            new AttributeInfo("prpubid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("prrelid", 3, typeInfoForOid26, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12053 = new CompositeTypeInfo(12053U, "pg_subscription_rel", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("srsublsn", 4, typeInfoForOid3220, true),
        //            new AttributeInfo("srsubstate", 3, typeInfoForOid18, true),
        //            new AttributeInfo("srrelid", 2, typeInfoForOid26, true),
        //            new AttributeInfo("srsubid", 1, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12054 = new CompositeTypeInfo(12054U, "pg_toast_2600", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12055 = new CompositeTypeInfo(12055U, "pg_toast_2604", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12056 = new CompositeTypeInfo(12056U, "pg_toast_3456", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12057 = new CompositeTypeInfo(12057U, "pg_toast_2606", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12058 = new CompositeTypeInfo(12058U, "pg_toast_826", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12059 = new CompositeTypeInfo(12059U, "pg_toast_2609", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12060 = new CompositeTypeInfo(12060U, "pg_toast_3466", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12061 = new CompositeTypeInfo(12061U, "pg_toast_3079", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12062 = new CompositeTypeInfo(12062U, "pg_toast_2328", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12063 = new CompositeTypeInfo(12063U, "pg_toast_1417", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12064 = new CompositeTypeInfo(12064U, "pg_toast_3118", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12065 = new CompositeTypeInfo(12065U, "pg_toast_3394", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12066 = new CompositeTypeInfo(12066U, "pg_toast_2612", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12067 = new CompositeTypeInfo(12067U, "pg_toast_2615", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12068 = new CompositeTypeInfo(12068U, "pg_toast_3350", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12069 = new CompositeTypeInfo(12069U, "pg_toast_3256", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12070 = new CompositeTypeInfo(12070U, "pg_toast_1255", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12071 = new CompositeTypeInfo(12071U, "pg_toast_2618", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12072 = new CompositeTypeInfo(12072U, "pg_toast_3596", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12073 = new CompositeTypeInfo(12073U, "pg_toast_2619", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12074 = new CompositeTypeInfo(12074U, "pg_toast_3381", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12075 = new CompositeTypeInfo(12075U, "pg_toast_3429", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12076 = new CompositeTypeInfo(12076U, "pg_toast_2620", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12077 = new CompositeTypeInfo(12077U, "pg_toast_3600", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12078 = new CompositeTypeInfo(12078U, "pg_toast_1247", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12079 = new CompositeTypeInfo(12079U, "pg_toast_1418", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12080 = new CompositeTypeInfo(12080U, "pg_toast_1260", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12081 = new CompositeTypeInfo(12081U, "pg_toast_1262", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12082 = new CompositeTypeInfo(12082U, "pg_toast_2964", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12083 = new CompositeTypeInfo(12083U, "pg_toast_1136", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12084 = new CompositeTypeInfo(12084U, "pg_toast_6000", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12085 = new CompositeTypeInfo(12085U, "pg_toast_2396", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true)
        //        });
        //    var typeInfoForOid12086 = new CompositeTypeInfo(12086U, "pg_toast_3592", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true)
        //        });
        //    var typeInfoForOid12087 = new CompositeTypeInfo(12087U, "pg_toast_6100", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false)
        //        });
        //    var typeInfoForOid12088 = new CompositeTypeInfo(12088U, "pg_toast_1213", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12090 = new CompositeTypeInfo(12090U, "pg_roles", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("oid", 13, typeInfoForOid26, false),
        //            new AttributeInfo("rolconfig", 12, typeInfoForOid1009, false),
        //            new AttributeInfo("rolbypassrls", 11, typeInfoForOid16, false),
        //            new AttributeInfo("rolvaliduntil", 10, typeInfoForOid1184, false),
        //            new AttributeInfo("rolpassword", 9, typeInfoForOid25, false),
        //            new AttributeInfo("rolconnlimit", 8, typeInfoForOid23, false),
        //            new AttributeInfo("rolreplication", 7, typeInfoForOid16, false),
        //            new AttributeInfo("rolcanlogin", 6, typeInfoForOid16, false),
        //            new AttributeInfo("rolcreatedb", 5, typeInfoForOid16, false),
        //            new AttributeInfo("rolcreaterole", 4, typeInfoForOid16, false),
        //            new AttributeInfo("rolinherit", 3, typeInfoForOid16, false),
        //            new AttributeInfo("rolsuper", 2, typeInfoForOid16, false),
        //            new AttributeInfo("rolname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12094 = new CompositeTypeInfo(12094U, "pg_shadow", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("useconfig", 9, typeInfoForOid1009, false),
        //            new AttributeInfo("valuntil", 8, typeInfoForOid1184, false),
        //            new AttributeInfo("passwd", 7, typeInfoForOid25, false),
        //            new AttributeInfo("usebypassrls", 6, typeInfoForOid16, false),
        //            new AttributeInfo("userepl", 5, typeInfoForOid16, false),
        //            new AttributeInfo("usesuper", 4, typeInfoForOid16, false),
        //            new AttributeInfo("usecreatedb", 3, typeInfoForOid16, false),
        //            new AttributeInfo("usesysid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("usename", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12098 = new CompositeTypeInfo(12098U, "pg_group", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("grolist", 3, typeInfoForOid1028, false),
        //            new AttributeInfo("grosysid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("groname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12101 = new CompositeTypeInfo(12101U, "pg_user", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("useconfig", 9, typeInfoForOid1009, false),
        //            new AttributeInfo("valuntil", 8, typeInfoForOid1184, false),
        //            new AttributeInfo("passwd", 7, typeInfoForOid25, false),
        //            new AttributeInfo("usebypassrls", 6, typeInfoForOid16, false),
        //            new AttributeInfo("userepl", 5, typeInfoForOid16, false),
        //            new AttributeInfo("usesuper", 4, typeInfoForOid16, false),
        //            new AttributeInfo("usecreatedb", 3, typeInfoForOid16, false),
        //            new AttributeInfo("usesysid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("usename", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12104 = new CompositeTypeInfo(12104U, "pg_policies", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("roles", 5, typeInfoForOid1003, false),
        //            new AttributeInfo("with_check", 8, typeInfoForOid25, false),
        //            new AttributeInfo("qual", 7, typeInfoForOid25, false),
        //            new AttributeInfo("cmd", 6, typeInfoForOid25, false),
        //            new AttributeInfo("permissive", 4, typeInfoForOid25, false),
        //            new AttributeInfo("policyname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12108 = new CompositeTypeInfo(12108U, "pg_rules", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("definition", 4, typeInfoForOid25, false),
        //            new AttributeInfo("rulename", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12112 = new CompositeTypeInfo(12112U, "pg_views", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("definition", 4, typeInfoForOid25, false),
        //            new AttributeInfo("viewowner", 3, typeInfoForOid19, false),
        //            new AttributeInfo("viewname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12116 = new CompositeTypeInfo(12116U, "pg_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("rowsecurity", 8, typeInfoForOid16, false),
        //            new AttributeInfo("hastriggers", 7, typeInfoForOid16, false),
        //            new AttributeInfo("hasrules", 6, typeInfoForOid16, false),
        //            new AttributeInfo("hasindexes", 5, typeInfoForOid16, false),
        //            new AttributeInfo("tablespace", 4, typeInfoForOid19, false),
        //            new AttributeInfo("tableowner", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12120 = new CompositeTypeInfo(12120U, "pg_matviews", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("definition", 7, typeInfoForOid25, false),
        //            new AttributeInfo("ispopulated", 6, typeInfoForOid16, false),
        //            new AttributeInfo("hasindexes", 5, typeInfoForOid16, false),
        //            new AttributeInfo("tablespace", 4, typeInfoForOid19, false),
        //            new AttributeInfo("matviewowner", 3, typeInfoForOid19, false),
        //            new AttributeInfo("matviewname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12124 = new CompositeTypeInfo(12124U, "pg_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("indexdef", 5, typeInfoForOid25, false),
        //            new AttributeInfo("tablespace", 4, typeInfoForOid19, false),
        //            new AttributeInfo("indexname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12128 = new CompositeTypeInfo(12128U, "pg_sequences", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("last_value", 11, typeInfoForOid20, false),
        //            new AttributeInfo("cache_size", 10, typeInfoForOid20, false),
        //            new AttributeInfo("cycle", 9, typeInfoForOid16, false),
        //            new AttributeInfo("increment_by", 8, typeInfoForOid20, false),
        //            new AttributeInfo("max_value", 7, typeInfoForOid20, false),
        //            new AttributeInfo("min_value", 6, typeInfoForOid20, false),
        //            new AttributeInfo("start_value", 5, typeInfoForOid20, false),
        //            new AttributeInfo("data_type", 4, typeInfoForOid2206, false),
        //            new AttributeInfo("sequenceowner", 3, typeInfoForOid19, false),
        //            new AttributeInfo("sequencename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12132 = new CompositeTypeInfo(12132U, "pg_stats", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("elem_count_histogram", 14, typeInfoForOid1021, false),
        //            new AttributeInfo("most_common_elem_freqs", 13, typeInfoForOid1021, false),
        //            new AttributeInfo("most_common_elems", 12, typeInfoForOid2277, false),
        //            new AttributeInfo("correlation", 11, typeInfoForOid700, false),
        //            new AttributeInfo("histogram_bounds", 10, typeInfoForOid2277, false),
        //            new AttributeInfo("most_common_freqs", 9, typeInfoForOid1021, false),
        //            new AttributeInfo("most_common_vals", 8, typeInfoForOid2277, false),
        //            new AttributeInfo("n_distinct", 7, typeInfoForOid700, false),
        //            new AttributeInfo("avg_width", 6, typeInfoForOid23, false),
        //            new AttributeInfo("null_frac", 5, typeInfoForOid700, false),
        //            new AttributeInfo("inherited", 4, typeInfoForOid16, false),
        //            new AttributeInfo("attname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12136 = new CompositeTypeInfo(12136U, "pg_stats_ext", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("most_common_base_freqs", 13, typeInfoForOid1022, false),
        //            new AttributeInfo("most_common_freqs", 12, typeInfoForOid1022, false),
        //            new AttributeInfo("most_common_val_nulls", 11, typeInfoForOid1000, false),
        //            new AttributeInfo("most_common_vals", 10, typeInfoForOid1009, false),
        //            new AttributeInfo("dependencies", 9, typeInfoForOid3402, false),
        //            new AttributeInfo("n_distinct", 8, typeInfoForOid3361, false),
        //            new AttributeInfo("kinds", 7, typeInfoForOid1002, false),
        //            new AttributeInfo("attnames", 6, typeInfoForOid1003, false),
        //            new AttributeInfo("statistics_owner", 5, typeInfoForOid19, false),
        //            new AttributeInfo("statistics_name", 4, typeInfoForOid19, false),
        //            new AttributeInfo("statistics_schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("tablename", 2, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12140 = new CompositeTypeInfo(12140U, "pg_publication_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tablename", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("pubname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12144 = new CompositeTypeInfo(12144U, "pg_locks", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("transactionid", 7, typeInfoForOid28, false),
        //            new AttributeInfo("fastpath", 15, typeInfoForOid16, false),
        //            new AttributeInfo("granted", 14, typeInfoForOid16, false),
        //            new AttributeInfo("mode", 13, typeInfoForOid25, false),
        //            new AttributeInfo("pid", 12, typeInfoForOid23, false),
        //            new AttributeInfo("virtualtransaction", 11, typeInfoForOid25, false),
        //            new AttributeInfo("objsubid", 10, typeInfoForOid21, false),
        //            new AttributeInfo("objid", 9, typeInfoForOid26, false),
        //            new AttributeInfo("classid", 8, typeInfoForOid26, false),
        //            new AttributeInfo("virtualxid", 6, typeInfoForOid25, false),
        //            new AttributeInfo("tuple", 5, typeInfoForOid21, false),
        //            new AttributeInfo("page", 4, typeInfoForOid23, false),
        //            new AttributeInfo("relation", 3, typeInfoForOid26, false),
        //            new AttributeInfo("database", 2, typeInfoForOid26, false),
        //            new AttributeInfo("locktype", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12147 = new CompositeTypeInfo(12147U, "pg_cursors", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("creation_time", 6, typeInfoForOid1184, false),
        //            new AttributeInfo("is_scrollable", 5, typeInfoForOid16, false),
        //            new AttributeInfo("is_binary", 4, typeInfoForOid16, false),
        //            new AttributeInfo("is_holdable", 3, typeInfoForOid16, false),
        //            new AttributeInfo("statement", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12150 = new CompositeTypeInfo(12150U, "pg_available_extensions", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("comment", 4, typeInfoForOid25, false),
        //            new AttributeInfo("installed_version", 3, typeInfoForOid25, false),
        //            new AttributeInfo("default_version", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12153 = new CompositeTypeInfo(12153U, "pg_available_extension_versions", "pg_catalog", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("comment", 8, typeInfoForOid25, false),
        //            new AttributeInfo("requires", 7, typeInfoForOid1003, false),
        //            new AttributeInfo("schema", 6, typeInfoForOid19, false),
        //            new AttributeInfo("relocatable", 5, typeInfoForOid16, false),
        //            new AttributeInfo("superuser", 4, typeInfoForOid16, false),
        //            new AttributeInfo("installed", 3, typeInfoForOid16, false),
        //            new AttributeInfo("version", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12156 = new CompositeTypeInfo(12156U, "pg_prepared_xacts", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("database", 5, typeInfoForOid19, false),
        //            new AttributeInfo("owner", 4, typeInfoForOid19, false),
        //            new AttributeInfo("prepared", 3, typeInfoForOid1184, false),
        //            new AttributeInfo("gid", 2, typeInfoForOid25, false),
        //            new AttributeInfo("transaction", 1, typeInfoForOid28, false)
        //        });
        //    var typeInfoForOid12160 = new CompositeTypeInfo(12160U, "pg_prepared_statements", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("from_sql", 5, typeInfoForOid16, false),
        //            new AttributeInfo("parameter_types", 4, typeInfoForOid2211, false),
        //            new AttributeInfo("prepare_time", 3, typeInfoForOid1184, false),
        //            new AttributeInfo("statement", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12163 = new CompositeTypeInfo(12163U, "pg_seclabels", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("label", 8, typeInfoForOid25, false),
        //            new AttributeInfo("provider", 7, typeInfoForOid25, false),
        //            new AttributeInfo("objname", 6, typeInfoForOid25, false),
        //            new AttributeInfo("objnamespace", 5, typeInfoForOid26, false),
        //            new AttributeInfo("objtype", 4, typeInfoForOid25, false),
        //            new AttributeInfo("objsubid", 3, typeInfoForOid23, false),
        //            new AttributeInfo("classoid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("objoid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12167 = new CompositeTypeInfo(12167U, "pg_settings", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("pending_restart", 17, typeInfoForOid16, false),
        //            new AttributeInfo("sourceline", 16, typeInfoForOid23, false),
        //            new AttributeInfo("sourcefile", 15, typeInfoForOid25, false),
        //            new AttributeInfo("reset_val", 14, typeInfoForOid25, false),
        //            new AttributeInfo("boot_val", 13, typeInfoForOid25, false),
        //            new AttributeInfo("enumvals", 12, typeInfoForOid1009, false),
        //            new AttributeInfo("max_val", 11, typeInfoForOid25, false),
        //            new AttributeInfo("min_val", 10, typeInfoForOid25, false),
        //            new AttributeInfo("source", 9, typeInfoForOid25, false),
        //            new AttributeInfo("vartype", 8, typeInfoForOid25, false),
        //            new AttributeInfo("context", 7, typeInfoForOid25, false),
        //            new AttributeInfo("extra_desc", 6, typeInfoForOid25, false),
        //            new AttributeInfo("short_desc", 5, typeInfoForOid25, false),
        //            new AttributeInfo("category", 4, typeInfoForOid25, false),
        //            new AttributeInfo("unit", 3, typeInfoForOid25, false),
        //            new AttributeInfo("setting", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12172 = new CompositeTypeInfo(12172U, "pg_file_settings", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("error", 7, typeInfoForOid25, false),
        //            new AttributeInfo("applied", 6, typeInfoForOid16, false),
        //            new AttributeInfo("setting", 5, typeInfoForOid25, false),
        //            new AttributeInfo("name", 4, typeInfoForOid25, false),
        //            new AttributeInfo("seqno", 3, typeInfoForOid23, false),
        //            new AttributeInfo("sourceline", 2, typeInfoForOid23, false),
        //            new AttributeInfo("sourcefile", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12175 = new CompositeTypeInfo(12175U, "pg_hba_file_rules", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("error", 9, typeInfoForOid25, false),
        //            new AttributeInfo("options", 8, typeInfoForOid1009, false),
        //            new AttributeInfo("auth_method", 7, typeInfoForOid25, false),
        //            new AttributeInfo("netmask", 6, typeInfoForOid25, false),
        //            new AttributeInfo("address", 5, typeInfoForOid25, false),
        //            new AttributeInfo("user_name", 4, typeInfoForOid1009, false),
        //            new AttributeInfo("database", 3, typeInfoForOid1009, false),
        //            new AttributeInfo("type", 2, typeInfoForOid25, false),
        //            new AttributeInfo("line_number", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12178 = new CompositeTypeInfo(12178U, "pg_timezone_abbrevs", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_dst", 3, typeInfoForOid16, false),
        //            new AttributeInfo("utc_offset", 2, typeInfoForOid1186, false),
        //            new AttributeInfo("abbrev", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12181 = new CompositeTypeInfo(12181U, "pg_timezone_names", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_dst", 4, typeInfoForOid16, false),
        //            new AttributeInfo("utc_offset", 3, typeInfoForOid1186, false),
        //            new AttributeInfo("abbrev", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12184 = new CompositeTypeInfo(12184U, "pg_config", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("setting", 2, typeInfoForOid25, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12187 = new CompositeTypeInfo(12187U, "pg_shmem_allocations", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("allocated_size", 4, typeInfoForOid20, false),
        //            new AttributeInfo("size", 3, typeInfoForOid20, false),
        //            new AttributeInfo("off", 2, typeInfoForOid20, false),
        //            new AttributeInfo("name", 1, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12190 = new CompositeTypeInfo(12190U, "pg_stat_all_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("autoanalyze_count", 22, typeInfoForOid20, false),
        //            new AttributeInfo("analyze_count", 21, typeInfoForOid20, false),
        //            new AttributeInfo("autovacuum_count", 20, typeInfoForOid20, false),
        //            new AttributeInfo("vacuum_count", 19, typeInfoForOid20, false),
        //            new AttributeInfo("last_autoanalyze", 18, typeInfoForOid1184, false),
        //            new AttributeInfo("last_analyze", 17, typeInfoForOid1184, false),
        //            new AttributeInfo("last_autovacuum", 16, typeInfoForOid1184, false),
        //            new AttributeInfo("last_vacuum", 15, typeInfoForOid1184, false),
        //            new AttributeInfo("n_mod_since_analyze", 14, typeInfoForOid20, false),
        //            new AttributeInfo("n_dead_tup", 13, typeInfoForOid20, false),
        //            new AttributeInfo("n_live_tup", 12, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12194 = new CompositeTypeInfo(12194U, "pg_stat_xact_all_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12198 = new CompositeTypeInfo(12198U, "pg_stat_sys_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("autoanalyze_count", 22, typeInfoForOid20, false),
        //            new AttributeInfo("analyze_count", 21, typeInfoForOid20, false),
        //            new AttributeInfo("autovacuum_count", 20, typeInfoForOid20, false),
        //            new AttributeInfo("vacuum_count", 19, typeInfoForOid20, false),
        //            new AttributeInfo("last_autoanalyze", 18, typeInfoForOid1184, false),
        //            new AttributeInfo("last_analyze", 17, typeInfoForOid1184, false),
        //            new AttributeInfo("last_autovacuum", 16, typeInfoForOid1184, false),
        //            new AttributeInfo("last_vacuum", 15, typeInfoForOid1184, false),
        //            new AttributeInfo("n_mod_since_analyze", 14, typeInfoForOid20, false),
        //            new AttributeInfo("n_dead_tup", 13, typeInfoForOid20, false),
        //            new AttributeInfo("n_live_tup", 12, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12202 = new CompositeTypeInfo(12202U, "pg_stat_xact_sys_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12205 = new CompositeTypeInfo(12205U, "pg_stat_user_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false),
        //            new AttributeInfo("autoanalyze_count", 22, typeInfoForOid20, false),
        //            new AttributeInfo("analyze_count", 21, typeInfoForOid20, false),
        //            new AttributeInfo("autovacuum_count", 20, typeInfoForOid20, false),
        //            new AttributeInfo("vacuum_count", 19, typeInfoForOid20, false),
        //            new AttributeInfo("last_autoanalyze", 18, typeInfoForOid1184, false),
        //            new AttributeInfo("last_analyze", 17, typeInfoForOid1184, false),
        //            new AttributeInfo("last_autovacuum", 16, typeInfoForOid1184, false),
        //            new AttributeInfo("last_vacuum", 15, typeInfoForOid1184, false),
        //            new AttributeInfo("n_mod_since_analyze", 14, typeInfoForOid20, false),
        //            new AttributeInfo("n_dead_tup", 13, typeInfoForOid20, false),
        //            new AttributeInfo("n_live_tup", 12, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12209 = new CompositeTypeInfo(12209U, "pg_stat_xact_user_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("n_tup_del", 10, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_upd", 9, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_ins", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_fetch", 7, typeInfoForOid20, false),
        //            new AttributeInfo("n_tup_hot_upd", 11, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("seq_tup_read", 5, typeInfoForOid20, false),
        //            new AttributeInfo("seq_scan", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12212 = new CompositeTypeInfo(12212U, "pg_statio_all_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tidx_blks_hit", 11, typeInfoForOid20, false),
        //            new AttributeInfo("tidx_blks_read", 10, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_hit", 9, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_read", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12216 = new CompositeTypeInfo(12216U, "pg_statio_sys_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tidx_blks_hit", 11, typeInfoForOid20, false),
        //            new AttributeInfo("tidx_blks_read", 10, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_hit", 9, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_read", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12219 = new CompositeTypeInfo(12219U, "pg_statio_user_tables", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tidx_blks_hit", 11, typeInfoForOid20, false),
        //            new AttributeInfo("tidx_blks_read", 10, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_hit", 9, typeInfoForOid20, false),
        //            new AttributeInfo("toast_blks_read", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12222 = new CompositeTypeInfo(12222U, "pg_stat_all_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_tup_fetch", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_read", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12226 = new CompositeTypeInfo(12226U, "pg_stat_sys_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_tup_fetch", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_read", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12229 = new CompositeTypeInfo(12229U, "pg_stat_user_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_tup_fetch", 8, typeInfoForOid20, false),
        //            new AttributeInfo("idx_tup_read", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_scan", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12232 = new CompositeTypeInfo(12232U, "pg_statio_all_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12236 = new CompositeTypeInfo(12236U, "pg_statio_sys_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12239 = new CompositeTypeInfo(12239U, "pg_statio_user_indexes", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("idx_blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("idx_blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("indexrelname", 5, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 4, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("indexrelid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12242 = new CompositeTypeInfo(12242U, "pg_statio_all_sequences", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12246 = new CompositeTypeInfo(12246U, "pg_statio_sys_sequences", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12249 = new CompositeTypeInfo(12249U, "pg_statio_user_sequences", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("blks_hit", 5, typeInfoForOid20, false),
        //            new AttributeInfo("blks_read", 4, typeInfoForOid20, false),
        //            new AttributeInfo("relname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("relid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12252 = new CompositeTypeInfo(12252U, "pg_stat_activity", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("backend_type", 20, typeInfoForOid25, false),
        //            new AttributeInfo("query", 19, typeInfoForOid25, false),
        //            new AttributeInfo("backend_xmin", 18, typeInfoForOid28, false),
        //            new AttributeInfo("backend_xid", 17, typeInfoForOid28, false),
        //            new AttributeInfo("state", 16, typeInfoForOid25, false),
        //            new AttributeInfo("wait_event", 15, typeInfoForOid25, false),
        //            new AttributeInfo("wait_event_type", 14, typeInfoForOid25, false),
        //            new AttributeInfo("state_change", 13, typeInfoForOid1184, false),
        //            new AttributeInfo("query_start", 12, typeInfoForOid1184, false),
        //            new AttributeInfo("xact_start", 11, typeInfoForOid1184, false),
        //            new AttributeInfo("backend_start", 10, typeInfoForOid1184, false),
        //            new AttributeInfo("client_port", 9, typeInfoForOid23, false),
        //            new AttributeInfo("client_hostname", 8, typeInfoForOid25, false),
        //            new AttributeInfo("client_addr", 7, typeInfoForOid869, false),
        //            new AttributeInfo("application_name", 6, typeInfoForOid25, false),
        //            new AttributeInfo("usename", 5, typeInfoForOid19, false),
        //            new AttributeInfo("usesysid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 3, typeInfoForOid23, false),
        //            new AttributeInfo("datname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12256 = new CompositeTypeInfo(12256U, "pg_stat_replication", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("spill_bytes", 23, typeInfoForOid20, false),
        //            new AttributeInfo("spill_count", 22, typeInfoForOid20, false),
        //            new AttributeInfo("spill_txns", 21, typeInfoForOid20, false),
        //            new AttributeInfo("reply_time", 20, typeInfoForOid1184, false),
        //            new AttributeInfo("sync_state", 19, typeInfoForOid25, false),
        //            new AttributeInfo("sync_priority", 18, typeInfoForOid23, false),
        //            new AttributeInfo("replay_lag", 17, typeInfoForOid1186, false),
        //            new AttributeInfo("flush_lag", 16, typeInfoForOid1186, false),
        //            new AttributeInfo("write_lag", 15, typeInfoForOid1186, false),
        //            new AttributeInfo("replay_lsn", 14, typeInfoForOid3220, false),
        //            new AttributeInfo("flush_lsn", 13, typeInfoForOid3220, false),
        //            new AttributeInfo("write_lsn", 12, typeInfoForOid3220, false),
        //            new AttributeInfo("sent_lsn", 11, typeInfoForOid3220, false),
        //            new AttributeInfo("state", 10, typeInfoForOid25, false),
        //            new AttributeInfo("backend_xmin", 9, typeInfoForOid28, false),
        //            new AttributeInfo("backend_start", 8, typeInfoForOid1184, false),
        //            new AttributeInfo("client_port", 7, typeInfoForOid23, false),
        //            new AttributeInfo("client_hostname", 6, typeInfoForOid25, false),
        //            new AttributeInfo("client_addr", 5, typeInfoForOid869, false),
        //            new AttributeInfo("application_name", 4, typeInfoForOid25, false),
        //            new AttributeInfo("usename", 3, typeInfoForOid19, false),
        //            new AttributeInfo("usesysid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12260 = new CompositeTypeInfo(12260U, "pg_stat_wal_receiver", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("conninfo", 14, typeInfoForOid25, false),
        //            new AttributeInfo("sender_port", 13, typeInfoForOid23, false),
        //            new AttributeInfo("sender_host", 12, typeInfoForOid25, false),
        //            new AttributeInfo("slot_name", 11, typeInfoForOid25, false),
        //            new AttributeInfo("latest_end_time", 10, typeInfoForOid1184, false),
        //            new AttributeInfo("latest_end_lsn", 9, typeInfoForOid3220, false),
        //            new AttributeInfo("last_msg_receipt_time", 8, typeInfoForOid1184, false),
        //            new AttributeInfo("last_msg_send_time", 7, typeInfoForOid1184, false),
        //            new AttributeInfo("received_tli", 6, typeInfoForOid23, false),
        //            new AttributeInfo("received_lsn", 5, typeInfoForOid3220, false),
        //            new AttributeInfo("receive_start_tli", 4, typeInfoForOid23, false),
        //            new AttributeInfo("receive_start_lsn", 3, typeInfoForOid3220, false),
        //            new AttributeInfo("status", 2, typeInfoForOid25, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12263 = new CompositeTypeInfo(12263U, "pg_stat_subscription", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("received_lsn", 5, typeInfoForOid3220, false),
        //            new AttributeInfo("latest_end_time", 9, typeInfoForOid1184, false),
        //            new AttributeInfo("latest_end_lsn", 8, typeInfoForOid3220, false),
        //            new AttributeInfo("last_msg_receipt_time", 7, typeInfoForOid1184, false),
        //            new AttributeInfo("last_msg_send_time", 6, typeInfoForOid1184, false),
        //            new AttributeInfo("relid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 3, typeInfoForOid23, false),
        //            new AttributeInfo("subname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("subid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12266 = new CompositeTypeInfo(12266U, "pg_stat_ssl", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("client_serial", 8, typeInfoForOid1700, false),
        //            new AttributeInfo("client_dn", 7, typeInfoForOid25, false),
        //            new AttributeInfo("compression", 6, typeInfoForOid16, false),
        //            new AttributeInfo("bits", 5, typeInfoForOid23, false),
        //            new AttributeInfo("cipher", 4, typeInfoForOid25, false),
        //            new AttributeInfo("version", 3, typeInfoForOid25, false),
        //            new AttributeInfo("ssl", 2, typeInfoForOid16, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false),
        //            new AttributeInfo("issuer_dn", 9, typeInfoForOid25, false)
        //        });
        //    var typeInfoForOid12269 = new CompositeTypeInfo(12269U, "pg_stat_gssapi", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("encrypted", 4, typeInfoForOid16, false),
        //            new AttributeInfo("principal", 3, typeInfoForOid25, false),
        //            new AttributeInfo("gss_authenticated", 2, typeInfoForOid16, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12272 = new CompositeTypeInfo(12272U, "pg_replication_slots", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("confirmed_flush_lsn", 12, typeInfoForOid3220, false),
        //            new AttributeInfo("restart_lsn", 11, typeInfoForOid3220, false),
        //            new AttributeInfo("catalog_xmin", 10, typeInfoForOid28, false),
        //            new AttributeInfo("xmin", 9, typeInfoForOid28, false),
        //            new AttributeInfo("active_pid", 8, typeInfoForOid23, false),
        //            new AttributeInfo("active", 7, typeInfoForOid16, false),
        //            new AttributeInfo("temporary", 6, typeInfoForOid16, false),
        //            new AttributeInfo("database", 5, typeInfoForOid19, false),
        //            new AttributeInfo("datoid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("slot_type", 3, typeInfoForOid25, false),
        //            new AttributeInfo("plugin", 2, typeInfoForOid19, false),
        //            new AttributeInfo("slot_name", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12276 = new CompositeTypeInfo(12276U, "pg_stat_database", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("stats_reset", 21, typeInfoForOid1184, false),
        //            new AttributeInfo("blk_write_time", 20, typeInfoForOid701, false),
        //            new AttributeInfo("blk_read_time", 19, typeInfoForOid701, false),
        //            new AttributeInfo("checksum_last_failure", 18, typeInfoForOid1184, false),
        //            new AttributeInfo("checksum_failures", 17, typeInfoForOid20, false),
        //            new AttributeInfo("deadlocks", 16, typeInfoForOid20, false),
        //            new AttributeInfo("temp_bytes", 15, typeInfoForOid20, false),
        //            new AttributeInfo("temp_files", 14, typeInfoForOid20, false),
        //            new AttributeInfo("conflicts", 13, typeInfoForOid20, false),
        //            new AttributeInfo("tup_deleted", 12, typeInfoForOid20, false),
        //            new AttributeInfo("tup_updated", 11, typeInfoForOid20, false),
        //            new AttributeInfo("tup_inserted", 10, typeInfoForOid20, false),
        //            new AttributeInfo("tup_fetched", 9, typeInfoForOid20, false),
        //            new AttributeInfo("tup_returned", 8, typeInfoForOid20, false),
        //            new AttributeInfo("blks_hit", 7, typeInfoForOid20, false),
        //            new AttributeInfo("blks_read", 6, typeInfoForOid20, false),
        //            new AttributeInfo("xact_rollback", 5, typeInfoForOid20, false),
        //            new AttributeInfo("xact_commit", 4, typeInfoForOid20, false),
        //            new AttributeInfo("numbackends", 3, typeInfoForOid23, false),
        //            new AttributeInfo("datname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12280 = new CompositeTypeInfo(12280U, "pg_stat_database_conflicts", "pg_catalog", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("confl_deadlock", 7, typeInfoForOid20, false),
        //            new AttributeInfo("confl_bufferpin", 6, typeInfoForOid20, false),
        //            new AttributeInfo("confl_snapshot", 5, typeInfoForOid20, false),
        //            new AttributeInfo("confl_lock", 4, typeInfoForOid20, false),
        //            new AttributeInfo("confl_tablespace", 3, typeInfoForOid20, false),
        //            new AttributeInfo("datname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12283 = new CompositeTypeInfo(12283U, "pg_stat_user_functions", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("self_time", 6, typeInfoForOid701, false),
        //            new AttributeInfo("total_time", 5, typeInfoForOid701, false),
        //            new AttributeInfo("calls", 4, typeInfoForOid20, false),
        //            new AttributeInfo("funcname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("funcid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12287 = new CompositeTypeInfo(12287U, "pg_stat_xact_user_functions", "pg_catalog", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("self_time", 6, typeInfoForOid701, false),
        //            new AttributeInfo("total_time", 5, typeInfoForOid701, false),
        //            new AttributeInfo("calls", 4, typeInfoForOid20, false),
        //            new AttributeInfo("funcname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("schemaname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("funcid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12291 = new CompositeTypeInfo(12291U, "pg_stat_archiver", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("stats_reset", 7, typeInfoForOid1184, false),
        //            new AttributeInfo("last_failed_time", 6, typeInfoForOid1184, false),
        //            new AttributeInfo("last_failed_wal", 5, typeInfoForOid25, false),
        //            new AttributeInfo("failed_count", 4, typeInfoForOid20, false),
        //            new AttributeInfo("last_archived_time", 3, typeInfoForOid1184, false),
        //            new AttributeInfo("last_archived_wal", 2, typeInfoForOid25, false),
        //            new AttributeInfo("archived_count", 1, typeInfoForOid20, false)
        //        });
        //    var typeInfoForOid12294 = new CompositeTypeInfo(12294U, "pg_stat_bgwriter", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("checkpoint_write_time", 3, typeInfoForOid701, false),
        //            new AttributeInfo("stats_reset", 11, typeInfoForOid1184, false),
        //            new AttributeInfo("buffers_alloc", 10, typeInfoForOid20, false),
        //            new AttributeInfo("buffers_backend_fsync", 9, typeInfoForOid20, false),
        //            new AttributeInfo("buffers_backend", 8, typeInfoForOid20, false),
        //            new AttributeInfo("maxwritten_clean", 7, typeInfoForOid20, false),
        //            new AttributeInfo("buffers_clean", 6, typeInfoForOid20, false),
        //            new AttributeInfo("buffers_checkpoint", 5, typeInfoForOid20, false),
        //            new AttributeInfo("checkpoint_sync_time", 4, typeInfoForOid701, false),
        //            new AttributeInfo("checkpoints_req", 2, typeInfoForOid20, false),
        //            new AttributeInfo("checkpoints_timed", 1, typeInfoForOid20, false)
        //        });
        //    var typeInfoForOid12297 = new CompositeTypeInfo(12297U, "pg_stat_progress_analyze", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ext_stats_computed", 9, typeInfoForOid20, false),
        //            new AttributeInfo("current_child_table_relid", 12, typeInfoForOid26, false),
        //            new AttributeInfo("ext_stats_total", 8, typeInfoForOid20, false),
        //            new AttributeInfo("sample_blks_scanned", 7, typeInfoForOid20, false),
        //            new AttributeInfo("sample_blks_total", 6, typeInfoForOid20, false),
        //            new AttributeInfo("child_tables_done", 11, typeInfoForOid20, false),
        //            new AttributeInfo("phase", 5, typeInfoForOid25, false),
        //            new AttributeInfo("relid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("datname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false),
        //            new AttributeInfo("child_tables_total", 10, typeInfoForOid20, false)
        //        });
        //    var typeInfoForOid12301 = new CompositeTypeInfo(12301U, "pg_stat_progress_vacuum", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("heap_blks_total", 6, typeInfoForOid20, false),
        //            new AttributeInfo("num_dead_tuples", 11, typeInfoForOid20, false),
        //            new AttributeInfo("max_dead_tuples", 10, typeInfoForOid20, false),
        //            new AttributeInfo("index_vacuum_count", 9, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_vacuumed", 8, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_scanned", 7, typeInfoForOid20, false),
        //            new AttributeInfo("phase", 5, typeInfoForOid25, false),
        //            new AttributeInfo("relid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("datname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12305 = new CompositeTypeInfo(12305U, "pg_stat_progress_cluster", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("index_rebuild_count", 12, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_scanned", 11, typeInfoForOid20, false),
        //            new AttributeInfo("heap_blks_total", 10, typeInfoForOid20, false),
        //            new AttributeInfo("heap_tuples_written", 9, typeInfoForOid20, false),
        //            new AttributeInfo("heap_tuples_scanned", 8, typeInfoForOid20, false),
        //            new AttributeInfo("cluster_index_relid", 7, typeInfoForOid26, false),
        //            new AttributeInfo("phase", 6, typeInfoForOid25, false),
        //            new AttributeInfo("command", 5, typeInfoForOid25, false),
        //            new AttributeInfo("relid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("datname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("datid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12309 = new CompositeTypeInfo(12309U, "pg_stat_progress_create_index", "pg_catalog", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("datname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("partitions_done", 16, typeInfoForOid20, false),
        //            new AttributeInfo("partitions_total", 15, typeInfoForOid20, false),
        //            new AttributeInfo("tuples_done", 14, typeInfoForOid20, false),
        //            new AttributeInfo("tuples_total", 13, typeInfoForOid20, false),
        //            new AttributeInfo("blocks_done", 12, typeInfoForOid20, false),
        //            new AttributeInfo("blocks_total", 11, typeInfoForOid20, false),
        //            new AttributeInfo("current_locker_pid", 10, typeInfoForOid20, false),
        //            new AttributeInfo("lockers_done", 9, typeInfoForOid20, false),
        //            new AttributeInfo("lockers_total", 8, typeInfoForOid20, false),
        //            new AttributeInfo("phase", 7, typeInfoForOid25, false),
        //            new AttributeInfo("command", 6, typeInfoForOid25, false),
        //            new AttributeInfo("index_relid", 5, typeInfoForOid26, false),
        //            new AttributeInfo("relid", 4, typeInfoForOid26, false),
        //            new AttributeInfo("datid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("pid", 1, typeInfoForOid23, false)
        //        });
        //    var typeInfoForOid12313 = new CompositeTypeInfo(12313U, "pg_user_mappings", "pg_catalog", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("umoptions", 6, typeInfoForOid1009, false),
        //            new AttributeInfo("usename", 5, typeInfoForOid19, false),
        //            new AttributeInfo("umuser", 4, typeInfoForOid26, false),
        //            new AttributeInfo("srvname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("srvid", 2, typeInfoForOid26, false),
        //            new AttributeInfo("umid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12317 = new CompositeTypeInfo(12317U, "pg_replication_origin_status", "pg_catalog", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("local_lsn", 4, typeInfoForOid3220, false),
        //            new AttributeInfo("remote_lsn", 3, typeInfoForOid3220, false),
        //            new AttributeInfo("external_id", 2, typeInfoForOid25, false),
        //            new AttributeInfo("local_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12389 = new CompositeTypeInfo(12389U, "information_schema_catalog_name",
        //        "information_schema", -1, false,
        //        new[] {new AttributeInfo("catalog_name", 1, typeInfoForOid12387, false)});
        //    var typeInfoForOid12397 = new CompositeTypeInfo(12397U, "applicable_roles", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 3, typeInfoForOid12394, false),
        //            new AttributeInfo("role_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12401 = new CompositeTypeInfo(12401U, "administrable_role_authorizations",
        //        "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 3, typeInfoForOid12394, false),
        //            new AttributeInfo("role_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12404 = new CompositeTypeInfo(12404U, "attributes", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("ordinal_position", 5, typeInfoForOid12382, false),
        //            new AttributeInfo("is_derived_reference_attribute", 31, typeInfoForOid12394, false),
        //            new AttributeInfo("dtd_identifier", 30, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 29, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 28, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 27, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 26, typeInfoForOid12387, false),
        //            new AttributeInfo("attribute_udt_name", 25, typeInfoForOid12387, false),
        //            new AttributeInfo("attribute_udt_schema", 24, typeInfoForOid12387, false),
        //            new AttributeInfo("attribute_udt_catalog", 23, typeInfoForOid12387, false),
        //            new AttributeInfo("interval_precision", 22, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 21, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 20, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 19, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 18, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 17, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 16, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 15, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 14, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 13, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 12, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 11, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 10, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 9, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("is_nullable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("attribute_default", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("attribute_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12408 = new CompositeTypeInfo(12408U, "character_sets", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("default_collate_name", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("default_collate_schema", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("default_collate_catalog", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("form_of_use", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("character_repertoire", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12412 = new CompositeTypeInfo(12412U, "check_constraint_routine_usage",
        //        "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("specific_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12416 = new CompositeTypeInfo(12416U, "check_constraints", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("check_clause", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12420 = new CompositeTypeInfo(12420U, "collations", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("pad_attribute", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("collation_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12424 = new CompositeTypeInfo(12424U, "collation_character_set_applicability",
        //        "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("character_set_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12428 = new CompositeTypeInfo(12428U, "column_column_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("dependent_column", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("column_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12432 = new CompositeTypeInfo(12432U, "column_domain_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("column_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12436 = new CompositeTypeInfo(12436U, "column_privileges", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("column_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12440 = new CompositeTypeInfo(12440U, "column_udt_usage", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("column_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12444 = new CompositeTypeInfo(12444U, "columns", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_updatable", 44, typeInfoForOid12394, false),
        //            new AttributeInfo("generation_expression", 43, typeInfoForOid12385, false),
        //            new AttributeInfo("is_generated", 42, typeInfoForOid12385, false),
        //            new AttributeInfo("identity_cycle", 41, typeInfoForOid12394, false),
        //            new AttributeInfo("identity_minimum", 40, typeInfoForOid12385, false),
        //            new AttributeInfo("identity_maximum", 39, typeInfoForOid12385, false),
        //            new AttributeInfo("identity_increment", 38, typeInfoForOid12385, false),
        //            new AttributeInfo("identity_start", 37, typeInfoForOid12385, false),
        //            new AttributeInfo("identity_generation", 36, typeInfoForOid12385, false),
        //            new AttributeInfo("is_identity", 35, typeInfoForOid12394, false),
        //            new AttributeInfo("is_self_referencing", 34, typeInfoForOid12394, false),
        //            new AttributeInfo("dtd_identifier", 33, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 32, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 31, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 30, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 29, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 28, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 27, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 26, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_name", 25, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_schema", 24, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_catalog", 23, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_name", 22, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 21, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 20, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 19, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 18, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 17, typeInfoForOid12387, false),
        //            new AttributeInfo("interval_precision", 16, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 15, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 14, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 13, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 12, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 11, typeInfoForOid12382, false),
        //            new AttributeInfo("character_octet_length", 10, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 9, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("is_nullable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("column_default", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("ordinal_position", 5, typeInfoForOid12382, false),
        //            new AttributeInfo("column_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12448 = new CompositeTypeInfo(12448U, "constraint_column_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("constraint_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("column_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12452 = new CompositeTypeInfo(12452U, "constraint_table_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("constraint_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12456 = new CompositeTypeInfo(12456U, "domain_constraints", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("initially_deferred", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("is_deferrable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("domain_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12460 = new CompositeTypeInfo(12460U, "domain_udt_usage", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("domain_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12464 = new CompositeTypeInfo(12464U, "domains", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("dtd_identifier", 27, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 26, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 25, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 24, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 23, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 22, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 21, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 20, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_default", 19, typeInfoForOid12385, false),
        //            new AttributeInfo("interval_precision", 18, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 17, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 16, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 15, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 14, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 13, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 12, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 11, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 10, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 6, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 5, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("domain_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12468 = new CompositeTypeInfo(12468U, "enabled_roles", "information_schema", -1, false,
        //        new[] {new AttributeInfo("role_name", 1, typeInfoForOid12387, false)});
        //    var typeInfoForOid12471 = new CompositeTypeInfo(12471U, "key_column_usage", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("position_in_unique_constraint", 9, typeInfoForOid12382, false),
        //            new AttributeInfo("ordinal_position", 8, typeInfoForOid12382, false),
        //            new AttributeInfo("column_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12475 = new CompositeTypeInfo(12475U, "parameters", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("parameter_default", 32, typeInfoForOid12385, false),
        //            new AttributeInfo("dtd_identifier", 31, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 30, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 29, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 28, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 27, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 26, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 25, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 24, typeInfoForOid12387, false),
        //            new AttributeInfo("interval_precision", 23, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 22, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 21, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 20, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 19, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 18, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 17, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 16, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 15, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 14, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 13, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 12, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 11, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 10, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("parameter_name", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("as_locator", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("is_result", 6, typeInfoForOid12394, false),
        //            new AttributeInfo("parameter_mode", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("ordinal_position", 4, typeInfoForOid12382, false),
        //            new AttributeInfo("specific_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12479 = new CompositeTypeInfo(12479U, "referential_constraints", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("delete_rule", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("update_rule", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("match_option", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("unique_constraint_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("unique_constraint_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("unique_constraint_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12483 = new CompositeTypeInfo(12483U, "role_column_grants", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("column_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12486 = new CompositeTypeInfo(12486U, "routine_privileges", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 10, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("routine_name", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_schema", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_catalog", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12490 = new CompositeTypeInfo(12490U, "role_routine_grants", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 10, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("routine_name", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_schema", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_catalog", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12493 = new CompositeTypeInfo(12493U, "routines", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("result_cast_dtd_identifier", 82, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_maximum_cardinality", 81, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_scope_name", 80, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_scope_schema", 79, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_scope_catalog", 78, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_type_udt_name", 77, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_type_udt_schema", 76, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_type_udt_catalog", 75, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_interval_precision", 74, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_interval_type", 73, typeInfoForOid12385, false),
        //            new AttributeInfo("result_cast_datetime_precision", 72, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_numeric_scale", 71, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_numeric_precision_radix", 70, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_numeric_precision", 69, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_collation_name", 68, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_collation_schema", 67, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_collation_catalog", 66, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_char_set_name", 65, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_char_set_schema", 64, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_char_set_catalog", 63, typeInfoForOid12387, false),
        //            new AttributeInfo("result_cast_char_octet_length", 62, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_char_max_length", 61, typeInfoForOid12382, false),
        //            new AttributeInfo("result_cast_as_locator", 60, typeInfoForOid12394, false),
        //            new AttributeInfo("result_cast_from_data_type", 59, typeInfoForOid12385, false),
        //            new AttributeInfo("is_udt_dependent", 58, typeInfoForOid12394, false),
        //            new AttributeInfo("new_savepoint_level", 57, typeInfoForOid12394, false),
        //            new AttributeInfo("last_altered", 56, typeInfoForOid12392, false),
        //            new AttributeInfo("created", 55, typeInfoForOid12392, false),
        //            new AttributeInfo("as_locator", 54, typeInfoForOid12394, false),
        //            new AttributeInfo("to_sql_specific_name", 53, typeInfoForOid12387, false),
        //            new AttributeInfo("to_sql_specific_schema", 52, typeInfoForOid12387, false),
        //            new AttributeInfo("to_sql_specific_catalog", 51, typeInfoForOid12387, false),
        //            new AttributeInfo("security_type", 50, typeInfoForOid12385, false),
        //            new AttributeInfo("is_implicitly_invocable", 49, typeInfoForOid12394, false),
        //            new AttributeInfo("is_user_defined_cast", 48, typeInfoForOid12394, false),
        //            new AttributeInfo("max_dynamic_result_sets", 47, typeInfoForOid12382, false),
        //            new AttributeInfo("schema_level_routine", 46, typeInfoForOid12394, false),
        //            new AttributeInfo("sql_path", 45, typeInfoForOid12385, false),
        //            new AttributeInfo("is_null_call", 44, typeInfoForOid12394, false),
        //            new AttributeInfo("sql_data_access", 43, typeInfoForOid12385, false),
        //            new AttributeInfo("is_deterministic", 42, typeInfoForOid12394, false),
        //            new AttributeInfo("parameter_style", 41, typeInfoForOid12385, false),
        //            new AttributeInfo("external_language", 40, typeInfoForOid12385, false),
        //            new AttributeInfo("external_name", 39, typeInfoForOid12385, false),
        //            new AttributeInfo("routine_definition", 38, typeInfoForOid12385, false),
        //            new AttributeInfo("routine_body", 37, typeInfoForOid12385, false),
        //            new AttributeInfo("dtd_identifier", 36, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 35, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 34, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 33, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 32, typeInfoForOid12387, false),
        //            new AttributeInfo("type_udt_name", 31, typeInfoForOid12387, false),
        //            new AttributeInfo("type_udt_schema", 30, typeInfoForOid12387, false),
        //            new AttributeInfo("type_udt_catalog", 29, typeInfoForOid12387, false),
        //            new AttributeInfo("interval_precision", 28, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 27, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 26, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 25, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 24, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 23, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 22, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 21, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 20, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 19, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 18, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 17, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 16, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 15, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 14, typeInfoForOid12385, false),
        //            new AttributeInfo("udt_name", 13, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 12, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 11, typeInfoForOid12387, false),
        //            new AttributeInfo("module_name", 10, typeInfoForOid12387, false),
        //            new AttributeInfo("module_schema", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("module_catalog", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("routine_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("routine_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12497 = new CompositeTypeInfo(12497U, "schemata", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("sql_path", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("default_character_set_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("default_character_set_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("default_character_set_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("schema_owner", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("schema_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("catalog_name", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12500 = new CompositeTypeInfo(12500U, "sequences", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("cycle_option", 12, typeInfoForOid12394, false),
        //            new AttributeInfo("increment", 11, typeInfoForOid12385, false),
        //            new AttributeInfo("maximum_value", 10, typeInfoForOid12385, false),
        //            new AttributeInfo("minimum_value", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("start_value", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("numeric_scale", 7, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 6, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 5, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("sequence_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("sequence_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("sequence_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12504 = new CompositeTypeInfo(12504U, "sql_features", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("comments", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("is_verified_by", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("is_supported", 5, typeInfoForOid12394, false),
        //            new AttributeInfo("sub_feature_name", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("sub_feature_id", 3, typeInfoForOid12385, false),
        //            new AttributeInfo("feature_name", 2, typeInfoForOid12385, false),
        //            new AttributeInfo("feature_id", 1, typeInfoForOid12385, false)
        //        });
        //    var typeInfoForOid12506 = new CompositeTypeInfo(12506U, "pg_toast_12503", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12509 = new CompositeTypeInfo(12509U, "sql_implementation_info", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("comments", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("character_value", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("integer_value", 3, typeInfoForOid12382, false),
        //            new AttributeInfo("implementation_info_name", 2, typeInfoForOid12385, false),
        //            new AttributeInfo("implementation_info_id", 1, typeInfoForOid12385, false)
        //        });
        //    var typeInfoForOid12511 = new CompositeTypeInfo(12511U, "pg_toast_12508", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12514 = new CompositeTypeInfo(12514U, "sql_parts", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("comments", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("is_verified_by", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("is_supported", 3, typeInfoForOid12394, false),
        //            new AttributeInfo("feature_name", 2, typeInfoForOid12385, false),
        //            new AttributeInfo("feature_id", 1, typeInfoForOid12385, false)
        //        });
        //    var typeInfoForOid12516 = new CompositeTypeInfo(12516U, "pg_toast_12513", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12519 = new CompositeTypeInfo(12519U, "sql_sizing", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("comments", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("supported_value", 3, typeInfoForOid12382, false),
        //            new AttributeInfo("sizing_name", 2, typeInfoForOid12385, false),
        //            new AttributeInfo("sizing_id", 1, typeInfoForOid12382, false)
        //        });
        //    var typeInfoForOid12521 = new CompositeTypeInfo(12521U, "pg_toast_12518", "pg_toast", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("tableoid", -6, typeInfoForOid26, true),
        //            new AttributeInfo("cmax", -5, typeInfoForOid29, true),
        //            new AttributeInfo("xmax", -4, typeInfoForOid28, true),
        //            new AttributeInfo("cmin", -3, typeInfoForOid29, true),
        //            new AttributeInfo("xmin", -2, typeInfoForOid28, true),
        //            new AttributeInfo("ctid", -1, typeInfoForOid27, true),
        //            new AttributeInfo("chunk_data", 3, typeInfoForOid17, false),
        //            new AttributeInfo("chunk_seq", 2, typeInfoForOid23, false),
        //            new AttributeInfo("chunk_id", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12524 = new CompositeTypeInfo(12524U, "table_constraints", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("enforced", 10, typeInfoForOid12394, false),
        //            new AttributeInfo("initially_deferred", 9, typeInfoForOid12394, false),
        //            new AttributeInfo("is_deferrable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("constraint_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("constraint_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12528 = new CompositeTypeInfo(12528U, "table_privileges", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("with_hierarchy", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("is_grantable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("table_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12532 = new CompositeTypeInfo(12532U, "role_table_grants", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("with_hierarchy", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("is_grantable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("table_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12535 = new CompositeTypeInfo(12535U, "tables", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("commit_action", 12, typeInfoForOid12385, false),
        //            new AttributeInfo("is_typed", 11, typeInfoForOid12394, false),
        //            new AttributeInfo("is_insertable_into", 10, typeInfoForOid12394, false),
        //            new AttributeInfo("user_defined_type_name", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("user_defined_type_schema", 8, typeInfoForOid12387, false),
        //            new AttributeInfo("user_defined_type_catalog", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("reference_generation", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("self_referencing_column_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_type", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12539 = new CompositeTypeInfo(12539U, "transforms", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("transform_type", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("group_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12543 = new CompositeTypeInfo(12543U, "triggered_update_columns", "information_schema",
        //        -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("event_object_column", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("event_object_table", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("event_object_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("event_object_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("trigger_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("trigger_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("trigger_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12547 = new CompositeTypeInfo(12547U, "triggers", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("created", 17, typeInfoForOid12392, false),
        //            new AttributeInfo("action_reference_new_row", 16, typeInfoForOid12387, false),
        //            new AttributeInfo("action_reference_old_row", 15, typeInfoForOid12387, false),
        //            new AttributeInfo("action_reference_new_table", 14, typeInfoForOid12387, false),
        //            new AttributeInfo("action_reference_old_table", 13, typeInfoForOid12387, false),
        //            new AttributeInfo("action_timing", 12, typeInfoForOid12385, false),
        //            new AttributeInfo("action_orientation", 11, typeInfoForOid12385, false),
        //            new AttributeInfo("action_statement", 10, typeInfoForOid12385, false),
        //            new AttributeInfo("action_condition", 9, typeInfoForOid12385, false),
        //            new AttributeInfo("action_order", 8, typeInfoForOid12382, false),
        //            new AttributeInfo("event_object_table", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("event_object_schema", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("event_object_catalog", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("event_manipulation", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("trigger_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("trigger_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("trigger_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12551 = new CompositeTypeInfo(12551U, "udt_privileges", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("udt_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12555 = new CompositeTypeInfo(12555U, "role_udt_grants", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("udt_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12558 = new CompositeTypeInfo(12558U, "usage_privileges", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("object_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("object_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("object_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("object_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12562 = new CompositeTypeInfo(12562U, "role_usage_grants", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("is_grantable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("privilege_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("object_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("object_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("object_schema", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("object_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("grantee", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("grantor", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12565 = new CompositeTypeInfo(12565U, "user_defined_types", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("ref_dtd_identifier", 29, typeInfoForOid12387, false),
        //            new AttributeInfo("source_dtd_identifier", 28, typeInfoForOid12387, false),
        //            new AttributeInfo("interval_precision", 27, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 26, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 25, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 24, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 23, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 22, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 21, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 20, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 19, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 18, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 17, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 16, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 15, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 14, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 13, typeInfoForOid12385, false),
        //            new AttributeInfo("reference_type", 12, typeInfoForOid12385, false),
        //            new AttributeInfo("ordering_routine_name", 11, typeInfoForOid12387, false),
        //            new AttributeInfo("ordering_routine_schema", 10, typeInfoForOid12387, false),
        //            new AttributeInfo("ordering_routine_catalog", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("ordering_category", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("ordering_form", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("is_final", 6, typeInfoForOid12394, false),
        //            new AttributeInfo("is_instantiable", 5, typeInfoForOid12394, false),
        //            new AttributeInfo("user_defined_type_category", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("user_defined_type_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("user_defined_type_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("user_defined_type_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12569 = new CompositeTypeInfo(12569U, "view_column_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("column_name", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("view_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("view_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("view_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12573 = new CompositeTypeInfo(12573U, "view_routine_usage", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("specific_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("specific_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12577 = new CompositeTypeInfo(12577U, "view_table_usage", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("table_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("view_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("view_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("view_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12581 = new CompositeTypeInfo(12581U, "views", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("is_trigger_insertable_into", 10, typeInfoForOid12394, false),
        //            new AttributeInfo("is_trigger_deletable", 9, typeInfoForOid12394, false),
        //            new AttributeInfo("is_trigger_updatable", 8, typeInfoForOid12394, false),
        //            new AttributeInfo("is_insertable_into", 7, typeInfoForOid12394, false),
        //            new AttributeInfo("is_updatable", 6, typeInfoForOid12394, false),
        //            new AttributeInfo("check_option", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("view_definition", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12585 = new CompositeTypeInfo(12585U, "data_type_privileges", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("dtd_identifier", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("object_type", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("object_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("object_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("object_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12589 = new CompositeTypeInfo(12589U, "element_types", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("dtd_identifier", 29, typeInfoForOid12387, false),
        //            new AttributeInfo("maximum_cardinality", 28, typeInfoForOid12382, false),
        //            new AttributeInfo("scope_name", 27, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_schema", 26, typeInfoForOid12387, false),
        //            new AttributeInfo("scope_catalog", 25, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_name", 24, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_schema", 23, typeInfoForOid12387, false),
        //            new AttributeInfo("udt_catalog", 22, typeInfoForOid12387, false),
        //            new AttributeInfo("domain_default", 21, typeInfoForOid12385, false),
        //            new AttributeInfo("interval_precision", 20, typeInfoForOid12382, false),
        //            new AttributeInfo("interval_type", 19, typeInfoForOid12385, false),
        //            new AttributeInfo("datetime_precision", 18, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_scale", 17, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision_radix", 16, typeInfoForOid12382, false),
        //            new AttributeInfo("numeric_precision", 15, typeInfoForOid12382, false),
        //            new AttributeInfo("collation_name", 14, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_schema", 13, typeInfoForOid12387, false),
        //            new AttributeInfo("collation_catalog", 12, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_name", 11, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_schema", 10, typeInfoForOid12387, false),
        //            new AttributeInfo("character_set_catalog", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("character_octet_length", 8, typeInfoForOid12382, false),
        //            new AttributeInfo("character_maximum_length", 7, typeInfoForOid12382, false),
        //            new AttributeInfo("data_type", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("collection_type_identifier", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("object_type", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("object_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("object_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("object_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12593 = new CompositeTypeInfo(12593U, "_pg_foreign_table_columns", "information_schema",
        //        -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("attfdwoptions", 4, typeInfoForOid1009, false),
        //            new AttributeInfo("attname", 3, typeInfoForOid19, false),
        //            new AttributeInfo("relname", 2, typeInfoForOid19, false),
        //            new AttributeInfo("nspname", 1, typeInfoForOid19, false)
        //        });
        //    var typeInfoForOid12597 = new CompositeTypeInfo(12597U, "column_options", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("option_value", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("option_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("column_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12600 = new CompositeTypeInfo(12600U, "_pg_foreign_data_wrappers", "information_schema",
        //        -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("foreign_data_wrapper_language", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("authorization_identifier", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("fdwoptions", 3, typeInfoForOid1009, false),
        //            new AttributeInfo("fdwowner", 2, typeInfoForOid26, false),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12603 = new CompositeTypeInfo(12603U, "foreign_data_wrapper_options",
        //        "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("option_value", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("option_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12606 = new CompositeTypeInfo(12606U, "foreign_data_wrappers", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("foreign_data_wrapper_language", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("library_name", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("authorization_identifier", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12609 = new CompositeTypeInfo(12609U, "_pg_foreign_servers", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("authorization_identifier", 9, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_version", 8, typeInfoForOid12385, false),
        //            new AttributeInfo("foreign_server_type", 7, typeInfoForOid12385, false),
        //            new AttributeInfo("foreign_data_wrapper_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_catalog", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("srvoptions", 2, typeInfoForOid1009, false),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12613 = new CompositeTypeInfo(12613U, "foreign_server_options", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("option_value", 4, typeInfoForOid12385, false),
        //            new AttributeInfo("option_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12616 = new CompositeTypeInfo(12616U, "foreign_servers", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("authorization_identifier", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_version", 6, typeInfoForOid12385, false),
        //            new AttributeInfo("foreign_server_type", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("foreign_data_wrapper_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_data_wrapper_catalog", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12619 = new CompositeTypeInfo(12619U, "_pg_foreign_tables", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("authorization_identifier", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("ftoptions", 4, typeInfoForOid1009, false),
        //            new AttributeInfo("foreign_table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12623 = new CompositeTypeInfo(12623U, "foreign_table_options", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("option_value", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("option_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12626 = new CompositeTypeInfo(12626U, "foreign_tables", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("foreign_server_name", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_schema", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_table_catalog", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12629 = new CompositeTypeInfo(12629U, "_pg_user_mappings", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("srvowner", 7, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 6, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 5, typeInfoForOid12387, false),
        //            new AttributeInfo("authorization_identifier", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("umuser", 3, typeInfoForOid26, false),
        //            new AttributeInfo("umoptions", 2, typeInfoForOid1009, false),
        //            new AttributeInfo("oid", 1, typeInfoForOid26, false)
        //        });
        //    var typeInfoForOid12633 = new CompositeTypeInfo(12633U, "user_mapping_options", "information_schema", -1,
        //        false,
        //        new[]
        //        {
        //            new AttributeInfo("option_value", 5, typeInfoForOid12385, false),
        //            new AttributeInfo("option_name", 4, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("authorization_identifier", 1, typeInfoForOid12387, false)
        //        });
        //    var typeInfoForOid12637 = new CompositeTypeInfo(12637U, "user_mappings", "information_schema", -1, false,
        //        new[]
        //        {
        //            new AttributeInfo("foreign_server_name", 3, typeInfoForOid12387, false),
        //            new AttributeInfo("foreign_server_catalog", 2, typeInfoForOid12387, false),
        //            new AttributeInfo("authorization_identifier", 1, typeInfoForOid12387, false)
        //        });


        //    TypeInfoCache = new Dictionary<uint, TypeInfo>
        //    {
        //        {16U, typeInfoForOid16},
        //        {17U, typeInfoForOid17},
        //        {18U, typeInfoForOid18},
        //        {19U, typeInfoForOid19},
        //        {20U, typeInfoForOid20},
        //        {21U, typeInfoForOid21},
        //        {22U, typeInfoForOid22},
        //        {23U, typeInfoForOid23},
        //        {24U, typeInfoForOid24},
        //        {25U, typeInfoForOid25},
        //        {26U, typeInfoForOid26},
        //        {27U, typeInfoForOid27},
        //        {28U, typeInfoForOid28},
        //        {29U, typeInfoForOid29},
        //        {30U, typeInfoForOid30},
        //        {114U, typeInfoForOid114},
        //        {142U, typeInfoForOid142},
        //        {143U, typeInfoForOid143},
        //        {194U, typeInfoForOid194},
        //        {199U, typeInfoForOid199},
        //        {600U, typeInfoForOid600},
        //        {601U, typeInfoForOid601},
        //        {602U, typeInfoForOid602},
        //        {603U, typeInfoForOid603},
        //        {604U, typeInfoForOid604},
        //        {628U, typeInfoForOid628},
        //        {629U, typeInfoForOid629},
        //        {650U, typeInfoForOid650},
        //        {651U, typeInfoForOid651},
        //        {700U, typeInfoForOid700},
        //        {701U, typeInfoForOid701},
        //        {718U, typeInfoForOid718},
        //        {719U, typeInfoForOid719},
        //        {774U, typeInfoForOid774},
        //        {775U, typeInfoForOid775},
        //        {790U, typeInfoForOid790},
        //        {791U, typeInfoForOid791},
        //        {829U, typeInfoForOid829},
        //        {869U, typeInfoForOid869},
        //        {1000U, typeInfoForOid1000},
        //        {1001U, typeInfoForOid1001},
        //        {1002U, typeInfoForOid1002},
        //        {1003U, typeInfoForOid1003},
        //        {1005U, typeInfoForOid1005},
        //        {1006U, typeInfoForOid1006},
        //        {1007U, typeInfoForOid1007},
        //        {1008U, typeInfoForOid1008},
        //        {1009U, typeInfoForOid1009},
        //        {1010U, typeInfoForOid1010},
        //        {1011U, typeInfoForOid1011},
        //        {1012U, typeInfoForOid1012},
        //        {1013U, typeInfoForOid1013},
        //        {1014U, typeInfoForOid1014},
        //        {1015U, typeInfoForOid1015},
        //        {1016U, typeInfoForOid1016},
        //        {1017U, typeInfoForOid1017},
        //        {1018U, typeInfoForOid1018},
        //        {1019U, typeInfoForOid1019},
        //        {1020U, typeInfoForOid1020},
        //        {1021U, typeInfoForOid1021},
        //        {1022U, typeInfoForOid1022},
        //        {1027U, typeInfoForOid1027},
        //        {1028U, typeInfoForOid1028},
        //        {1033U, typeInfoForOid1033},
        //        {1034U, typeInfoForOid1034},
        //        {1040U, typeInfoForOid1040},
        //        {1041U, typeInfoForOid1041},
        //        {1042U, typeInfoForOid1042},
        //        {1043U, typeInfoForOid1043},
        //        {1082U, typeInfoForOid1082},
        //        {1083U, typeInfoForOid1083},
        //        {1114U, typeInfoForOid1114},
        //        {1115U, typeInfoForOid1115},
        //        {1182U, typeInfoForOid1182},
        //        {1183U, typeInfoForOid1183},
        //        {1184U, typeInfoForOid1184},
        //        {1185U, typeInfoForOid1185},
        //        {1186U, typeInfoForOid1186},
        //        {1187U, typeInfoForOid1187},
        //        {1231U, typeInfoForOid1231},
        //        {1263U, typeInfoForOid1263},
        //        {1266U, typeInfoForOid1266},
        //        {1270U, typeInfoForOid1270},
        //        {1560U, typeInfoForOid1560},
        //        {1561U, typeInfoForOid1561},
        //        {1562U, typeInfoForOid1562},
        //        {1563U, typeInfoForOid1563},
        //        {1700U, typeInfoForOid1700},
        //        {1790U, typeInfoForOid1790},
        //        {2201U, typeInfoForOid2201},
        //        {2202U, typeInfoForOid2202},
        //        {2203U, typeInfoForOid2203},
        //        {2204U, typeInfoForOid2204},
        //        {2205U, typeInfoForOid2205},
        //        {2206U, typeInfoForOid2206},
        //        {2207U, typeInfoForOid2207},
        //        {2208U, typeInfoForOid2208},
        //        {2209U, typeInfoForOid2209},
        //        {2210U, typeInfoForOid2210},
        //        {2211U, typeInfoForOid2211},
        //        {2949U, typeInfoForOid2949},
        //        {2950U, typeInfoForOid2950},
        //        {2951U, typeInfoForOid2951},
        //        {2970U, typeInfoForOid2970},
        //        {3220U, typeInfoForOid3220},
        //        {3221U, typeInfoForOid3221},
        //        {3361U, typeInfoForOid3361},
        //        {3402U, typeInfoForOid3402},
        //        {3614U, typeInfoForOid3614},
        //        {3615U, typeInfoForOid3615},
        //        {3642U, typeInfoForOid3642},
        //        {3643U, typeInfoForOid3643},
        //        {3644U, typeInfoForOid3644},
        //        {3645U, typeInfoForOid3645},
        //        {3734U, typeInfoForOid3734},
        //        {3735U, typeInfoForOid3735},
        //        {3769U, typeInfoForOid3769},
        //        {3770U, typeInfoForOid3770},
        //        {3802U, typeInfoForOid3802},
        //        {3807U, typeInfoForOid3807},
        //        {3905U, typeInfoForOid3905},
        //        {3907U, typeInfoForOid3907},
        //        {3909U, typeInfoForOid3909},
        //        {3911U, typeInfoForOid3911},
        //        {3913U, typeInfoForOid3913},
        //        {3927U, typeInfoForOid3927},
        //        {4072U, typeInfoForOid4072},
        //        {4073U, typeInfoForOid4073},
        //        {4089U, typeInfoForOid4089},
        //        {4090U, typeInfoForOid4090},
        //        {4096U, typeInfoForOid4096},
        //        {4097U, typeInfoForOid4097},
        //        {5017U, typeInfoForOid5017},
        //        {12381U, typeInfoForOid12381},
        //        {12384U, typeInfoForOid12384},
        //        {12386U, typeInfoForOid12386},
        //        {12391U, typeInfoForOid12391},
        //        {12393U, typeInfoForOid12393},
        //        {32U, typeInfoForOid32},
        //        {269U, typeInfoForOid269},
        //        {325U, typeInfoForOid325},
        //        {705U, typeInfoForOid705},
        //        {2249U, typeInfoForOid2249},
        //        {2275U, typeInfoForOid2275},
        //        {2276U, typeInfoForOid2276},
        //        {2277U, typeInfoForOid2277},
        //        {2278U, typeInfoForOid2278},
        //        {2279U, typeInfoForOid2279},
        //        {2280U, typeInfoForOid2280},
        //        {2281U, typeInfoForOid2281},
        //        {2282U, typeInfoForOid2282},
        //        {2283U, typeInfoForOid2283},
        //        {2287U, typeInfoForOid2287},
        //        {2776U, typeInfoForOid2776},
        //        {3115U, typeInfoForOid3115},
        //        {3310U, typeInfoForOid3310},
        //        {3500U, typeInfoForOid3500},
        //        {3831U, typeInfoForOid3831},
        //        {3838U, typeInfoForOid3838},
        //        {12382U, typeInfoForOid12382},
        //        {12385U, typeInfoForOid12385},
        //        {12387U, typeInfoForOid12387},
        //        {12392U, typeInfoForOid12392},
        //        {12394U, typeInfoForOid12394},
        //        {3904U, typeInfoForOid3904},
        //        {3906U, typeInfoForOid3906},
        //        {3908U, typeInfoForOid3908},
        //        {3910U, typeInfoForOid3910},
        //        {3912U, typeInfoForOid3912},
        //        {3926U, typeInfoForOid3926},
        //        {71U, typeInfoForOid71},
        //        {75U, typeInfoForOid75},
        //        {81U, typeInfoForOid81},
        //        {83U, typeInfoForOid83},
        //        {1248U, typeInfoForOid1248},
        //        {2842U, typeInfoForOid2842},
        //        {2843U, typeInfoForOid2843},
        //        {4066U, typeInfoForOid4066},
        //        {6101U, typeInfoForOid6101},
        //        {12000U, typeInfoForOid12000},
        //        {12001U, typeInfoForOid12001},
        //        {12002U, typeInfoForOid12002},
        //        {12003U, typeInfoForOid12003},
        //        {12004U, typeInfoForOid12004},
        //        {12005U, typeInfoForOid12005},
        //        {12006U, typeInfoForOid12006},
        //        {12007U, typeInfoForOid12007},
        //        {12008U, typeInfoForOid12008},
        //        {12009U, typeInfoForOid12009},
        //        {12010U, typeInfoForOid12010},
        //        {12011U, typeInfoForOid12011},
        //        {12012U, typeInfoForOid12012},
        //        {12013U, typeInfoForOid12013},
        //        {12014U, typeInfoForOid12014},
        //        {12015U, typeInfoForOid12015},
        //        {12016U, typeInfoForOid12016},
        //        {12017U, typeInfoForOid12017},
        //        {12018U, typeInfoForOid12018},
        //        {12019U, typeInfoForOid12019},
        //        {12020U, typeInfoForOid12020},
        //        {12021U, typeInfoForOid12021},
        //        {12022U, typeInfoForOid12022},
        //        {12023U, typeInfoForOid12023},
        //        {12024U, typeInfoForOid12024},
        //        {12025U, typeInfoForOid12025},
        //        {12026U, typeInfoForOid12026},
        //        {12027U, typeInfoForOid12027},
        //        {12028U, typeInfoForOid12028},
        //        {12029U, typeInfoForOid12029},
        //        {12030U, typeInfoForOid12030},
        //        {12031U, typeInfoForOid12031},
        //        {12032U, typeInfoForOid12032},
        //        {12033U, typeInfoForOid12033},
        //        {12034U, typeInfoForOid12034},
        //        {12035U, typeInfoForOid12035},
        //        {12036U, typeInfoForOid12036},
        //        {12037U, typeInfoForOid12037},
        //        {12038U, typeInfoForOid12038},
        //        {12039U, typeInfoForOid12039},
        //        {12040U, typeInfoForOid12040},
        //        {12041U, typeInfoForOid12041},
        //        {12042U, typeInfoForOid12042},
        //        {12043U, typeInfoForOid12043},
        //        {12044U, typeInfoForOid12044},
        //        {12045U, typeInfoForOid12045},
        //        {12046U, typeInfoForOid12046},
        //        {12047U, typeInfoForOid12047},
        //        {12048U, typeInfoForOid12048},
        //        {12049U, typeInfoForOid12049},
        //        {12050U, typeInfoForOid12050},
        //        {12051U, typeInfoForOid12051},
        //        {12052U, typeInfoForOid12052},
        //        {12053U, typeInfoForOid12053},
        //        {12054U, typeInfoForOid12054},
        //        {12055U, typeInfoForOid12055},
        //        {12056U, typeInfoForOid12056},
        //        {12057U, typeInfoForOid12057},
        //        {12058U, typeInfoForOid12058},
        //        {12059U, typeInfoForOid12059},
        //        {12060U, typeInfoForOid12060},
        //        {12061U, typeInfoForOid12061},
        //        {12062U, typeInfoForOid12062},
        //        {12063U, typeInfoForOid12063},
        //        {12064U, typeInfoForOid12064},
        //        {12065U, typeInfoForOid12065},
        //        {12066U, typeInfoForOid12066},
        //        {12067U, typeInfoForOid12067},
        //        {12068U, typeInfoForOid12068},
        //        {12069U, typeInfoForOid12069},
        //        {12070U, typeInfoForOid12070},
        //        {12071U, typeInfoForOid12071},
        //        {12072U, typeInfoForOid12072},
        //        {12073U, typeInfoForOid12073},
        //        {12074U, typeInfoForOid12074},
        //        {12075U, typeInfoForOid12075},
        //        {12076U, typeInfoForOid12076},
        //        {12077U, typeInfoForOid12077},
        //        {12078U, typeInfoForOid12078},
        //        {12079U, typeInfoForOid12079},
        //        {12080U, typeInfoForOid12080},
        //        {12081U, typeInfoForOid12081},
        //        {12082U, typeInfoForOid12082},
        //        {12083U, typeInfoForOid12083},
        //        {12084U, typeInfoForOid12084},
        //        {12085U, typeInfoForOid12085},
        //        {12086U, typeInfoForOid12086},
        //        {12087U, typeInfoForOid12087},
        //        {12088U, typeInfoForOid12088},
        //        {12090U, typeInfoForOid12090},
        //        {12094U, typeInfoForOid12094},
        //        {12098U, typeInfoForOid12098},
        //        {12101U, typeInfoForOid12101},
        //        {12104U, typeInfoForOid12104},
        //        {12108U, typeInfoForOid12108},
        //        {12112U, typeInfoForOid12112},
        //        {12116U, typeInfoForOid12116},
        //        {12120U, typeInfoForOid12120},
        //        {12124U, typeInfoForOid12124},
        //        {12128U, typeInfoForOid12128},
        //        {12132U, typeInfoForOid12132},
        //        {12136U, typeInfoForOid12136},
        //        {12140U, typeInfoForOid12140},
        //        {12144U, typeInfoForOid12144},
        //        {12147U, typeInfoForOid12147},
        //        {12150U, typeInfoForOid12150},
        //        {12153U, typeInfoForOid12153},
        //        {12156U, typeInfoForOid12156},
        //        {12160U, typeInfoForOid12160},
        //        {12163U, typeInfoForOid12163},
        //        {12167U, typeInfoForOid12167},
        //        {12172U, typeInfoForOid12172},
        //        {12175U, typeInfoForOid12175},
        //        {12178U, typeInfoForOid12178},
        //        {12181U, typeInfoForOid12181},
        //        {12184U, typeInfoForOid12184},
        //        {12187U, typeInfoForOid12187},
        //        {12190U, typeInfoForOid12190},
        //        {12194U, typeInfoForOid12194},
        //        {12198U, typeInfoForOid12198},
        //        {12202U, typeInfoForOid12202},
        //        {12205U, typeInfoForOid12205},
        //        {12209U, typeInfoForOid12209},
        //        {12212U, typeInfoForOid12212},
        //        {12216U, typeInfoForOid12216},
        //        {12219U, typeInfoForOid12219},
        //        {12222U, typeInfoForOid12222},
        //        {12226U, typeInfoForOid12226},
        //        {12229U, typeInfoForOid12229},
        //        {12232U, typeInfoForOid12232},
        //        {12236U, typeInfoForOid12236},
        //        {12239U, typeInfoForOid12239},
        //        {12242U, typeInfoForOid12242},
        //        {12246U, typeInfoForOid12246},
        //        {12249U, typeInfoForOid12249},
        //        {12252U, typeInfoForOid12252},
        //        {12256U, typeInfoForOid12256},
        //        {12260U, typeInfoForOid12260},
        //        {12263U, typeInfoForOid12263},
        //        {12266U, typeInfoForOid12266},
        //        {12269U, typeInfoForOid12269},
        //        {12272U, typeInfoForOid12272},
        //        {12276U, typeInfoForOid12276},
        //        {12280U, typeInfoForOid12280},
        //        {12283U, typeInfoForOid12283},
        //        {12287U, typeInfoForOid12287},
        //        {12291U, typeInfoForOid12291},
        //        {12294U, typeInfoForOid12294},
        //        {12297U, typeInfoForOid12297},
        //        {12301U, typeInfoForOid12301},
        //        {12305U, typeInfoForOid12305},
        //        {12309U, typeInfoForOid12309},
        //        {12313U, typeInfoForOid12313},
        //        {12317U, typeInfoForOid12317},
        //        {12389U, typeInfoForOid12389},
        //        {12397U, typeInfoForOid12397},
        //        {12401U, typeInfoForOid12401},
        //        {12404U, typeInfoForOid12404},
        //        {12408U, typeInfoForOid12408},
        //        {12412U, typeInfoForOid12412},
        //        {12416U, typeInfoForOid12416},
        //        {12420U, typeInfoForOid12420},
        //        {12424U, typeInfoForOid12424},
        //        {12428U, typeInfoForOid12428},
        //        {12432U, typeInfoForOid12432},
        //        {12436U, typeInfoForOid12436},
        //        {12440U, typeInfoForOid12440},
        //        {12444U, typeInfoForOid12444},
        //        {12448U, typeInfoForOid12448},
        //        {12452U, typeInfoForOid12452},
        //        {12456U, typeInfoForOid12456},
        //        {12460U, typeInfoForOid12460},
        //        {12464U, typeInfoForOid12464},
        //        {12468U, typeInfoForOid12468},
        //        {12471U, typeInfoForOid12471},
        //        {12475U, typeInfoForOid12475},
        //        {12479U, typeInfoForOid12479},
        //        {12483U, typeInfoForOid12483},
        //        {12486U, typeInfoForOid12486},
        //        {12490U, typeInfoForOid12490},
        //        {12493U, typeInfoForOid12493},
        //        {12497U, typeInfoForOid12497},
        //        {12500U, typeInfoForOid12500},
        //        {12504U, typeInfoForOid12504},
        //        {12506U, typeInfoForOid12506},
        //        {12509U, typeInfoForOid12509},
        //        {12511U, typeInfoForOid12511},
        //        {12514U, typeInfoForOid12514},
        //        {12516U, typeInfoForOid12516},
        //        {12519U, typeInfoForOid12519},
        //        {12521U, typeInfoForOid12521},
        //        {12524U, typeInfoForOid12524},
        //        {12528U, typeInfoForOid12528},
        //        {12532U, typeInfoForOid12532},
        //        {12535U, typeInfoForOid12535},
        //        {12539U, typeInfoForOid12539},
        //        {12543U, typeInfoForOid12543},
        //        {12547U, typeInfoForOid12547},
        //        {12551U, typeInfoForOid12551},
        //        {12555U, typeInfoForOid12555},
        //        {12558U, typeInfoForOid12558},
        //        {12562U, typeInfoForOid12562},
        //        {12565U, typeInfoForOid12565},
        //        {12569U, typeInfoForOid12569},
        //        {12573U, typeInfoForOid12573},
        //        {12577U, typeInfoForOid12577},
        //        {12581U, typeInfoForOid12581},
        //        {12585U, typeInfoForOid12585},
        //        {12589U, typeInfoForOid12589},
        //        {12593U, typeInfoForOid12593},
        //        {12597U, typeInfoForOid12597},
        //        {12600U, typeInfoForOid12600},
        //        {12603U, typeInfoForOid12603},
        //        {12606U, typeInfoForOid12606},
        //        {12609U, typeInfoForOid12609},
        //        {12613U, typeInfoForOid12613},
        //        {12616U, typeInfoForOid12616},
        //        {12619U, typeInfoForOid12619},
        //        {12623U, typeInfoForOid12623},
        //        {12626U, typeInfoForOid12626},
        //        {12629U, typeInfoForOid12629},
        //        {12633U, typeInfoForOid12633},
        //        {12637U, typeInfoForOid12637}
        //    };
        //}
    }
}
