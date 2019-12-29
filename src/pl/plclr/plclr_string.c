#if defined(WIN32)

#include "postgres.h"
#include "plclr_string.h"

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
