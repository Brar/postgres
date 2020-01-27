#ifndef PLCLR_STRING_H
#define PLCLR_STRING_H
#include "plclr.h"

#if defined(WIN32)

    int utf16le_to_utf8(LPWSTR utf16_string, char* utf8_string_buffer, int utf8_string_buffer_size);
    int utf16le_get_required_utf8_buffer_size(LPWSTR utf16_string);

    int utf8_to_utf16le(char* utf8_string, LPWSTR utf16_string_buffer, int utf16_string_buffer_size);
    int utf8_get_required_utf16le_buffer_size(char* utf8_string);
#endif

clr_char* server_encoding_to_clr_char(const char* input, int length);
char* clr_char_to_server_encoding(clr_char* input);

#endif // PLCLR_STRING_H
