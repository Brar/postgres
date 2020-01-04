using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    public abstract class TestBase
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileInfoPrivate
        {
            public uint FunctionOid;
            public IntPtr FunctionNamePtr;
            public IntPtr FunctionBodyPtr;
        }

        protected static void FunctionCompileInfo(uint oid, string functionName, string functionBody, Action<IntPtr, int> functionCompileInfo)
        {
            FunctionCompileInfoPrivate fci;
            fci.FunctionOid = oid;
            fci.FunctionNamePtr = StringToPointer(functionName);
            fci.FunctionBodyPtr = StringToPointer(functionBody);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf<FunctionCompileInfoPrivate>());
            Marshal.StructureToPtr(fci, ptr, false);

            try
            {
                functionCompileInfo(ptr, Marshal.SizeOf<FunctionCompileInfoPrivate>());
            }
            finally
            {
                Marshal.FreeCoTaskMem(fci.FunctionNamePtr);
                Marshal.FreeCoTaskMem(fci.FunctionBodyPtr);
                Marshal.FreeCoTaskMem(ptr);
            }

            static IntPtr StringToPointer(string value)
                => Environment.OSVersion.Platform == PlatformID.Win32NT ? Marshal.StringToCoTaskMemUni(value) : Marshal.StringToCoTaskMemUTF8(value);
        }
    }
}
