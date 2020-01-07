-- first test some basic functionality
CREATE EXTENSION IF NOT EXISTS plclr;

-- simple function to get the module loaded
CREATE FUNCTION plclr_regress_simple() RETURNS void AS '/* 😇 */' LANGUAGE plclr;

select plclr_regress_simple();

-- simple add function
CREATE FUNCTION plclr_add(int, int) RETURNS int AS 'return arg1 + arg2;' LANGUAGE plclr;

select plclr_add(21, 21);