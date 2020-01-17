using System;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PlClrManagedInterface
    {
        public IntPtr CompileFunctionPtr;
        public IntPtr ExecuteFunctionPtr;
    }
}
