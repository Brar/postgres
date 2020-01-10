using System;
using System.Collections.Immutable;

namespace PlClr
{
    internal class FunctionCompileInfo
    {
        public uint FunctionOid { get; }
        public string FunctionName { get; }
        public string FunctionBody { get; }
        public uint ReturnValueType { get; }
        public bool ReturnsSet { get; }
        public int NumberOfArguments { get; }
        public ImmutableArray<uint> ArgumentOids { get; }
        public ImmutableArray<string>? ArgumentNames { get; }
        public ImmutableArray<byte>? ArgumentModes { get; }

        public FunctionCompileInfo(uint functionOid, string functionName, string functionBody, uint returnValueType, bool returnsSet) : this(functionOid,
            functionName, functionBody, returnValueType, returnsSet, 0, Array.Empty<uint>(), null, Array.Empty<byte>())
        {
        }

        public FunctionCompileInfo(uint functionOid, string functionName, string functionBody, uint returnValueType, bool returnsSet, int numberOfArguments,
            uint[] argumentOids, string[]? argumentNames, byte[]? argumentModes)
        {
            FunctionOid = functionOid;
            FunctionName = functionName;
            FunctionBody = functionBody;
            ReturnValueType = returnValueType;
            ReturnsSet = returnsSet;
            NumberOfArguments = numberOfArguments;
            ArgumentOids = argumentOids.ToImmutableArray();
            ArgumentNames = argumentNames?.ToImmutableArray();
            ArgumentModes = argumentModes?.ToImmutableArray();
        }
    }
}
