using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PlClr.Managed.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FunctionCompileInfo
    {
        public uint FunctionOid;
        public IntPtr FunctionNamePtr;
        public IntPtr FunctionBodyPtr;
        public uint ReturnValueType;
        public bool ReturnsSet;
        public int NumberOfArguments;
        public IntPtr ArgumentTypes;
        public IntPtr ArgumentNames;
        public IntPtr ArgumentModes;
    }
}
