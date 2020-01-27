using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FunctionCallInfo
    {
        public IntPtr ExecuteDelegatePtr;
        public int NumberOfArguments;
        public IntPtr ArgumentValues;
    }
}
