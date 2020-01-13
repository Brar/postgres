using System;
using System.Runtime.InteropServices;

namespace PlClr
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NullableDatum
    {
        public IntPtr Value;
        public bool IsNull;
    }
}
