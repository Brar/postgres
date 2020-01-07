using System;
using System.Collections.Immutable;

namespace PlClr
{
    internal class FunctionCompileInfo
    {
        public uint FunctionOid { get; }
        public string FunctionName { get; }
        public string FunctionBody { get; }
        public int NumberOfArguments { get; }
        public ImmutableArray<uint> ArgumentOids { get; }
        public ImmutableArray<string>? ArgumentNames { get; }
        public ImmutableArray<byte>? ArgumentModes { get; }

        public FunctionCompileInfo(uint functionOid, string functionName, string functionBody) : this(functionOid,
            functionName, functionBody, 0, Array.Empty<uint>(), null, Array.Empty<byte>())
        {
        }

        public FunctionCompileInfo(uint functionOid, string functionName, string functionBody, int numberOfArguments,
            uint[] argumentOids, string[]? argumentNames, byte[]? argumentModes)
        {
            FunctionOid = functionOid;
            FunctionName = functionName;
            FunctionBody = functionBody;
            NumberOfArguments = numberOfArguments;
            ArgumentOids = argumentOids.ToImmutableArray();
            ArgumentNames = argumentNames?.ToImmutableArray();
            ArgumentModes = argumentModes?.ToImmutableArray();
        }
    }
}
