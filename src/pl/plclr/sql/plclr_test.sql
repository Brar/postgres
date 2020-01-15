-- first test some basic functionality
CREATE EXTENSION IF NOT EXISTS plclr;

-- simple function to get the module loaded
CREATE FUNCTION plclr_regress_simple() RETURNS void AS '/* 😇 */' LANGUAGE plclr;

select plclr_regress_simple();

-- simple function adding two integer values
CREATE FUNCTION plclr_regress_add_integer(integer, integer) RETURNS int AS 'return arg1 + arg2;' LANGUAGE plclr;

select plclr_regress_add_integer(21, 21);