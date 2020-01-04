namespace PlClr
{
    internal class FunctionCompileInfo
    {
        public FunctionCompileInfo(uint functionOid, string functionName, string functionBody)
        {
            FunctionOid = functionOid;
            FunctionName = functionName;
            FunctionBody = functionBody;
        }

        public uint FunctionOid { get; }
        public string FunctionName { get; }
        public string FunctionBody { get; }
    }
}
