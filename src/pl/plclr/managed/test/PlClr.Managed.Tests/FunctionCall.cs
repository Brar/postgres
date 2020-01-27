
using System;

namespace PlClr.Managed.Tests
{
	public class FunctionCall
	{
        private readonly PAllocDelegate _pAlloc;

        public FunctionCall(PAllocDelegate pAllocDelegate)
        {
            _pAlloc = pAllocDelegate;
        }

        IntPtr DeToastDatum(IntPtr ptr) => ptr;

        IntPtr GetAttributeByNum(IntPtr argPtr, short attNo, out bool isNull)
        {
            isNull = true;
            return IntPtr.Zero;
        }

        public IntPtr GetDeToastDatumFunctionPointer() => System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ReferenceTypeConversionDelegate>(DeToastDatum);
        public IntPtr GetGetAttributeByNumFunctionPtr() => System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<GetAttributeByNumDelegate>(GetAttributeByNum);
    }
}