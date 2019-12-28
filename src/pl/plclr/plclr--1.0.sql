/* src/pl/plclr/plclr--1.0.sql */

CREATE FUNCTION plclr_call_handler() RETURNS language_handler
  LANGUAGE c AS 'MODULE_PATHNAME';

CREATE LANGUAGE plclr
  HANDLER plclr_call_handler;

COMMENT ON LANGUAGE plclr IS 'PL/CLR procedural language';
