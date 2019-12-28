/* src/pl/plclr/plclr--unpackaged--1.0.sql */

ALTER EXTENSION plclr ADD LANGUAGE plclr;
-- ALTER ADD LANGUAGE doesn't pick up the support functions, so we have to.
ALTER EXTENSION plclr ADD FUNCTION plclr_call_handler();
--ALTER EXTENSION plclr ADD FUNCTION plclr_inline_handler(internal);
--ALTER EXTENSION plclr ADD FUNCTION plclr_validator(oid);
