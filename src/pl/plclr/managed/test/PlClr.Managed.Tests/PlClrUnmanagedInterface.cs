using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PlClrUnmanagedInterface
    {
        /* palloc functions */
        public IntPtr PAllocFunctionPtr;
        public IntPtr PAlloc0FunctionPtr;
        public IntPtr RePAllocFunctionPtr;
        public IntPtr PFreeFunctionPtr;

        /* logging functions */
        public IntPtr ELogFunctionPtr;
        public IntPtr EReportFunctionPtr;

        /* type I/O */
        public IntPtr GetTypeInfoFunctionPtr;
        public IntPtr GetTextFunctionPtr;
        public IntPtr SetTextFunctionPtr;
        public IntPtr DeToastDatumFunctionPtr;
        public IntPtr GetAttributeByNumFunctionPtr;
    }
}
