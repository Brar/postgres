using System;
using System.Text;

namespace PlClr
{
    public static class Marshal
    {
        public static ulong SizeOf<T>()
            => (ulong)System.Runtime.InteropServices.Marshal.SizeOf<T>();

        public static string? ToStringPFree(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            var result = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr)
                : System.Runtime.InteropServices.Marshal.PtrToStringUTF8(ptr);

            ServerMemory.PFree(ptr);

            return result;
        }

        public static IntPtr ToPtrPalloc(string? str)
            => StringToPtrPalloc(str, false);

        public static IntPtr ToPtrPalloc(int? value)
        {
            if (!value.HasValue)
                return IntPtr.Zero;

            var ptr = ServerMemory.Palloc(SizeOf<int>());
            System.Runtime.InteropServices.Marshal.WriteInt32(ptr, value.Value);
            return ptr;
        }

        public static IntPtr ToPtrPalloc(uint? value)
        {
            if (!value.HasValue)
                return IntPtr.Zero;

            var ptr = ServerMemory.Palloc(SizeOf<uint>());
            System.Runtime.InteropServices.Marshal.WriteInt32(ptr, unchecked((int)value.Value));
            return ptr;
        }

        public static IntPtr StructureToPtrPalloc<T>(T value) where T : struct
        {
            var ptr = ServerMemory.Palloc(SizeOf<T>());
            System.Runtime.InteropServices.Marshal.StructureToPtr(value, ptr, false);
            return ptr;
        }

        internal static unsafe IntPtr StringToPtrPalloc(string? str, bool toClrString)
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
                    Buffer.MemoryCopy((void*)unsafeString, (void*)returnValue, (ulong)resultLength, (ulong)resultLength);
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
