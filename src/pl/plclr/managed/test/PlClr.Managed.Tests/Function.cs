using System;

namespace PlClr.Managed.Tests
{
	public class Function
	{
		private readonly PAllocDelegate _pAlloc;
		public Function(PAllocDelegate pAllocDelegate)
		{
			_pAlloc = pAllocDelegate;
		}

        IntPtr GetText(IntPtr argPtr)
        {
            return IntPtr.Zero;
        }

        IntPtr SetText(IntPtr argPtr)
        {
            return IntPtr.Zero;
        }

        public IntPtr GetGetTextFunctionPointer()
        {
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ReferenceTypeConversionDelegate>(GetText);
        }

        public IntPtr GetSetTextFunctionPointer()
        {
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ReferenceTypeConversionDelegate>(SetText);
        }
	}
}