using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class CompileParameters
    {
        public CompileParameters(uint functionOid, string? functionName, string? functionBody)
        {
            FunctionOid = functionOid;
            FunctionName = functionName;
            FunctionBody = functionBody;
        }

        public uint FunctionOid { get; }
        public string? FunctionName { get; }
        public string? FunctionBody { get; }
    }
}
