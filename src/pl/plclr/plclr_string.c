#include "postgres.h"

#include <access/xact.h>
#include "mb/pg_wchar.h"
#include "plclr.h"

#if defined(WIN32)

int utf16le_to_utf8(LPWSTR utf16_string, char* utf8_string_buffer, int utf8_string_buffer_size)
{
    return WideCharToMultiByte(CP_UTF8, 0, utf16_string, -1, utf8_string_buffer, utf8_string_buffer_size, NULL, NULL);
}

int utf16le_get_required_utf8_buffer_size(LPWSTR utf16_string)
{
    return WideCharToMultiByte(CP_UTF8, 0, utf16_string, -1, NULL, 0, NULL, NULL);
}

int utf8_to_utf16le(char* utf8_string, LPWSTR utf16_string_buffer, int utf16_string_buffer_size)
{
    return MultiByteToWideChar(CP_UTF8, 0, utf8_string, -1, utf16_string_buffer, utf16_string_buffer_size);
}

int utf8_get_required_utf16le_buffer_size(char* utf8_string)
{
    return MultiByteToWideChar(CP_UTF8, 0, utf8_string, -1, NULL, 0);
}
#endif

clr_char*
server_encoding_to_clr_char(const char *input)
{
	int			db_encoding = GetDatabaseEncoding();
	size_t		input_length = strlen(input);
	size_t		output_length = input_length;
	clr_char*	output;

#ifdef WIN32
	unsigned	codepage;
	codepage = pg_enc2name_tbl[db_encoding].codepage;

	if (codepage != 0)
	{
		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
		output_length = MultiByteToWideChar(codepage, 0, input, input_length, output, input_length);
	}
	else if (db_encoding != PG_UTF8)
#else
	if (db_encoding != PG_UTF8)
#endif
	{
		char	   *utf8;

		/*
		 * XXX pg_do_encoding_conversion() requires a transaction.  In the
		 * absence of one, hope for the input to be valid UTF8.
		 */
		if (IsTransactionState())
		{
			utf8 = (char *) pg_do_encoding_conversion((unsigned char *) input,
													  input_length,
													  db_encoding,
													  PG_UTF8);
			if (utf8 != input)
				input_length = strlen(utf8);
		}
		else
			utf8 = (char *) input;

		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
#ifdef WIN32
		output_length = MultiByteToWideChar(CP_UTF8, 0, utf8, input_length, output, input_length);
#else
		memcpy(output, input, input_length);
#endif

		if (utf8 != input)
			pfree(utf8);
	}
	/* If our input is already UTF-8 we have to and copy it to a new
	 * palloc'd output anyways as that's what our caller expects.
	 */
	else
	{
		output = (clr_char *) palloc(sizeof(clr_char) * (input_length + 1));
		memcpy(output, input, input_length);
	}

	output[output_length] = (clr_char) 0;

	if (output_length == 0 && input_length > 0)
	{
		pfree(output);
		return NULL;			/* error */
	}

	return output;
}
