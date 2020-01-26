-- first test some basic functionality
--CREATE EXTENSION IF NOT EXISTS plclr;
--
---- simple function to get the module loaded
--CREATE FUNCTION plclr_regress_simple() RETURNS void AS '/* 😇 */' LANGUAGE plclr;
--
--select plclr_regress_simple();
--
---- simple function adding two integer values
--CREATE FUNCTION plclr_regress_add_integer(integer, integer) RETURNS int AS 'if (arg1 == null || arg2 == null) return null; return arg1 + arg2;' LANGUAGE plclr;
--
--select plclr_regress_add_integer(21, 21);
--select plclr_regress_add_integer(NULL, 21);
--select plclr_regress_add_integer(21, NULL);
--
---- strict function counting characters in a text
--CREATE FUNCTION plclr_regress_count_characters(text) RETURNS int AS 'return arg1.Length;' STRICT LANGUAGE plclr;
--
--select plclr_regress_count_characters('123456789');
--select plclr_regress_count_characters(NULL);
--
---- strict function concatenating two text values
--CREATE FUNCTION plclr_regress_concat_text(text, text) RETURNS text AS 'return arg1 + arg2;' STRICT LANGUAGE plclr;
--
--select plclr_regress_concat_text('The answer is 4', '2!');

CREATE TABLE emp
(
	name TEXT NOT NULL,
	salary integer NOT NULL
);

COPY emp FROM STDIN;
Bill	1200
Sam	1550
\.

CREATE FUNCTION plclr_overpaid(emp, integer) RETURNS boolean AS 'return arg1.salary > arg2;' STRICT LANGUAGE plclr;

SELECT name, plclr_overpaid(emp, 1500) AS overpaid FROM emp WHERE name = 'Bill' OR name = 'Sam';