using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class Types
    {
        private readonly PAllocDelegate _pAlloc;

        public Types(PAllocDelegate pAllocDelegate)
        {
            _pAlloc = pAllocDelegate;
        }

        IntPtr GetTypeInfo(uint value)
        {
            return IntPtr.Zero;
        }

        public IntPtr GetGetTypeInfoFunctionPtr() => System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<GetTypeInfoDelegate>(GetTypeInfo);
    }
}
