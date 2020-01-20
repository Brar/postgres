using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PlClrUnmanagedInterface
    {
        public IntPtr PAllocFunctionPtr;
        public IntPtr PAlloc0FunctionPtr;
        public IntPtr RePAllocFunctionPtr;
        public IntPtr PFreeFunctionPtr;
        public IntPtr ELogFunctionPtr;
        public IntPtr EReportFunctionPtr;
        public IntPtr GetTextFunctionPtr;
    }
}
