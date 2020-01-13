using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class CompileParameters
    {
        public CompileParameters(uint functionOid, string? functionName, string? functionBody, IEnumerable<CompileArgumentInfo>? compileArgumentInfos = null, HardcodedOid returnValueType = HardcodedOid.Void, bool returnsSet = false)
        {
            FunctionOid = functionOid;
            FunctionName = functionName;
            FunctionBody = functionBody;
            ReturnValueType = returnValueType;
            ReturnsSet = returnsSet;
            CompileArgumentInfos = (compileArgumentInfos ?? Enumerable.Empty<CompileArgumentInfo>()).ToImmutableArray() ;
        }

        public uint FunctionOid { get; }
        public string? FunctionName { get; }
        public string? FunctionBody { get; }
        public ImmutableArray<CompileArgumentInfo> CompileArgumentInfos { get; }
        public HardcodedOid ReturnValueType { get; }
        public bool ReturnsSet { get; }
    }
}
