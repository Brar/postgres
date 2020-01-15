#include "postgres.h"

#include "access/htup_details.h"
#include "catalog/pg_proc.h"
#include "commands/event_trigger.h"
#include "commands/trigger.h"
#include "funcapi.h"
#include "utils/builtins.h"
#include "utils/syscache.h"

#include "plclr_comp.h"
#include "plclr_managed.h"
#include "plclr_string.h"

typedef struct PlClrFunctionCompileResult
{
    void* ExecuteDelegatePtr;
} PlClrFunctionCompileResult, *PlClrFunctionCompileResultPtr;


/* A context appropriate for short-term allocs during compilation */
MemoryContext plclr_compile_tmp_cxt;

PlClr_function *
plclr_compile_function(FunctionCallInfo fcinfo, HeapTuple procTup, PlClr_function* function, bool forValidator)
{
	Form_pg_proc procStruct = (Form_pg_proc) GETSTRUCT(procTup);
	bool		is_dml_trigger = CALLED_AS_TRIGGER(fcinfo);
	bool		is_event_trigger = CALLED_AS_EVENT_TRIGGER(fcinfo);
	Datum		prosrcdatum;
	bool		isnull;
	PlClrFunctionCompileInfo compileInfo;
	//HeapTuple	typeTup;
	//Form_pg_type typeStruct;
	//PLpgSQL_variable *var;
	//PLpgSQL_rec *rec;
	//int			i;
	//ErrorContextCallback plerrcontext;
	//int			parse_rc;
	//Oid			rettypeid;
	int			numargs;
	//int			num_in_args = 0;
	//int			num_out_args = 0;
	Oid		   *argtypes;
	char	  **argnames;
	char	   *argmodes;
	//int		   *in_arg_varnos = NULL;
	//PLpgSQL_variable **out_arg_variables;
	MemoryContext func_cxt;
	PlClrFunctionCompileResultPtr compileResult;

	/*
	 * Create the new function struct, if not done already.  The function
	 * structs are never thrown away, so keep them in TopMemoryContext.
	 */
	if (function == NULL)
	{
		function = (PlClr_function *)
			MemoryContextAllocZero(TopMemoryContext, sizeof(PlClr_function));
	}
	else
	{
		/* re-using a previously existing struct, so clear it out */
		memset(function, 0, sizeof(PlClr_function));
	}

	/*
	 * All the permanent output of compilation (e.g. parse tree) is kept in a
	 * per-function memory context, so it can be reclaimed easily.
	 */
	func_cxt = AllocSetContextCreate(TopMemoryContext,
									 "PL/pgSQL function",
									 ALLOCSET_DEFAULT_SIZES);
	plclr_compile_tmp_cxt = MemoryContextSwitchTo(func_cxt);

//	function->fn_signature = format_procedure(fcinfo->flinfo->fn_oid);
	MemoryContextSetIdentifier(func_cxt, function->fn_signature);
	function->fn_oid = fcinfo->flinfo->fn_oid;
	function->fn_xmin = HeapTupleHeaderGetRawXmin(procTup->t_data);
	function->fn_tid = procTup->t_self;
	function->fn_input_collation = fcinfo->fncollation;
	function->fn_cxt = func_cxt;
	function->out_param_varno = -1; /* set up for no OUT param */

	if (is_dml_trigger)
		function->fn_is_trigger = PLCLR_DML_TRIGGER;
	else if (is_event_trigger)
		function->fn_is_trigger = PLCLR_EVENT_TRIGGER;
	else
		function->fn_is_trigger = PLCLR_NOT_TRIGGER;

	function->fn_prokind = procStruct->prokind;
	switch (function->fn_is_trigger)
	{
		case PLCLR_NOT_TRIGGER:

			/*
			 * Fetch info about the procedure's parameters. Allocations aren't
			 * needed permanently, so make them in tmp cxt.
			 *
			 * We also need to resolve any polymorphic input or output
			 * argument types.  In validation mode we won't be able to, so we
			 * arbitrarily assume we are dealing with integers.
			 */
			MemoryContextSwitchTo(plclr_compile_tmp_cxt);

			compileInfo.FunctionOid = fcinfo->flinfo->fn_oid;

			procStruct = (Form_pg_proc) GETSTRUCT(procTup);
		    compileInfo.ReturnValueType = procStruct->prorettype;
		    compileInfo.ReturnsSet = procStruct->proretset;
		    compileInfo.FunctionName = server_encoding_to_clr_char(NameStr(procStruct->proname));

		    prosrcdatum = SysCacheGetAttr(PROCOID, procTup, Anum_pg_proc_prosrc, &isnull);
		    if (isnull)
		        elog(ERROR, "null prosrc");
		    compileInfo.FunctionBody = server_encoding_to_clr_char(TextDatumGetCString(prosrcdatum));

		    numargs = get_func_arg_info(procTup, &argtypes, &argnames, &argmodes);

			//plpgsql_resolve_polymorphic_argtypes(numargs, argtypes, argmodes,
			//									 fcinfo->flinfo->fn_expr,
			//									 forValidator,
			//									 plpgsql_error_funcname);

			//in_arg_varnos = (int *) palloc(numargs * sizeof(int));
			//out_arg_variables = (PLpgSQL_variable **) palloc(numargs * sizeof(PLpgSQL_variable *));

		    if(numargs > 0)
		    {
		        compileInfo.NumberOfArguments = numargs;
		        compileInfo.ArgumentTypes = argtypes;

		        if (argnames == NULL)
		        {
			        compileInfo.ArgumentNames = NULL;
		        }
		        else
		        {
		            clr_char** unicode_argnames = palloc(numargs * sizeof(clr_char*));
		            for (int i = numargs - 1; i >= 0; i--)
		            {
			            unicode_argnames[i] = server_encoding_to_clr_char(argnames[i]);
		            }
		            compileInfo.ArgumentNames = unicode_argnames;
		        }

		        if (argmodes == NULL)
		        {
			        compileInfo.ArgumentModes = NULL;
		        }
		    	else
				{
					compileInfo.ArgumentModes = argmodes;
				}
		    }
			else
			{
		        compileInfo.NumberOfArguments = 0;
		        compileInfo.ArgumentTypes = NULL;
		        compileInfo.ArgumentNames = NULL;
		        compileInfo.ArgumentModes = NULL;
			}

			compileResult = (PlClrFunctionCompileResultPtr)plclrManagedInterface->CompilePtr(&compileInfo, sizeof(PlClrFunctionCompileInfo));
			function->action = compileResult->ExecuteDelegatePtr;

			MemoryContextSwitchTo(func_cxt);
			break;

		case PLCLR_DML_TRIGGER:
			break;

		case PLCLR_EVENT_TRIGGER:
			break;

		default:
			elog(ERROR, "unrecognized function typecode: %d",
				 (int) function->fn_is_trigger);
			break;
	}

	return function;
}