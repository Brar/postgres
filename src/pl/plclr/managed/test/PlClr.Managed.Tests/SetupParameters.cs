using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class SetupParameters
    {
        public bool PallocFunctionPtrIsNull { get; set; }
        public bool Palloc0FunctionPtrIsNull { get; set; }
        public bool RePallocFunctionPtrIsNull { get; set; }
        public bool PFreeFunctionPtrIsNull { get; set; }
        public bool ELogFunctionPtrIsNull { get; set; }
    }
}
