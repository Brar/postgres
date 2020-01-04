-- first test some basic functionality
CREATE EXTENSION IF NOT EXISTS plclr;

-- simple function to get the module loaded
CREATE FUNCTION plclr_regress_simple() RETURNS int AS '/* 😇 */return 1;' LANGUAGE plclr;

select plclr_regress_simple();