using System;
using System.Runtime.InteropServices;

namespace PlClr
{
    internal static class MarshalHelper
    {
        public static string PtrToString(IntPtr arg, string argName)
        {
            if (arg == IntPtr.Zero)
            {
                throw new ArgumentNullException(argName);
            }

            var result = Environment.OSVersion.Platform == PlatformID.Win32NT ? Marshal.PtrToStringUni(arg) : Marshal.PtrToStringUTF8(arg);

            if (result == null)
            {
                throw new ArgumentNullException(argName);
            }
            return result;
        }

    }
}
