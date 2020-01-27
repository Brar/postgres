using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FunctionCompileResult
    {
        public IntPtr ExecuteDelegatePtr;
    }
}
