using System;
using System.Text;

namespace PlClr
{
    public static class Marshal
    {

        public static string? PtrToStringPFree(IntPtr ptr, string argName)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(argName);

            var result = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr)
                : System.Runtime.InteropServices.Marshal.PtrToStringUTF8(ptr);

            ServerMemory.PFree(ptr);

            return result;
        }

        public static IntPtr StringToPtrPalloc(string? str)
            => StringToPtrPalloc(str, false);

        public static unsafe IntPtr StringToPtrPalloc(string? str, bool toClrString)
        {
            if (str == null)
                return IntPtr.Zero;

            var toUFT16 = toClrString && Environment.OSVersion.Platform == PlatformID.Win32NT;

            var resultLength = toUFT16
                ? 2 * (str.Length + 1)
                : 1 + Encoding.UTF8.GetMaxByteCount(str.Length);

            if (str.Length > resultLength)
                throw new OverflowException(nameof(str));

            var returnValue = ServerMemory.Palloc((ulong)resultLength);

            if (toUFT16)
            {
                fixed (char* unsafeString = str)
                {
                    Buffer.MemoryCopy((void*)unsafeString, (void*)returnValue, (ulong)resultLength, (ulong)resultLength - 2UL);
                }
            }
            else
            {
                fixed (char* firstChar = str)
                {
                    var unsafeBuffer = (byte*)returnValue;
                    var bytesWritten = Encoding.UTF8.GetBytes(firstChar, str.Length, unsafeBuffer, resultLength);
                    unsafeBuffer[bytesWritten] = 0;
                }
            }
            return returnValue;                
        }

    }
}
