using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PlClr
{
    public static class PlClrMain
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileInfoPrivate
        {
            public uint FunctionOid;
            public IntPtr FunctionNamePtr;
            public IntPtr FunctionBodyPtr;
        }

        public static int CompileFunction(IntPtr arg, int argLength)
        {
            if (argLength < Marshal.SizeOf(typeof(FunctionCompileInfoPrivate)))
            {
                return -1;
            }

            var compileInfo = GetFunctionCompileInfo(arg);

            var assembly = CSharpCompiler.Compile(compileInfo);
            var type = assembly.GetType($"_{compileInfo.FunctionOid}_");
            if (type == null)
                return -1;

            var result = type.InvokeMember(compileInfo.FunctionName, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);
            if (result == null)
                return -1;

            return (int) result;

            static FunctionCompileInfo GetFunctionCompileInfo(IntPtr arg)
            {
                var ci = Marshal.PtrToStructure<FunctionCompileInfoPrivate>(arg);
                return new FunctionCompileInfo(ci.FunctionOid,
                    MarshalHelper.PtrToString(ci.FunctionNamePtr, nameof(ci.FunctionNamePtr)),
                    MarshalHelper.PtrToString(ci.FunctionBodyPtr, nameof(ci.FunctionBodyPtr)));
            }
        }

        public static int ExecuteFunction(IntPtr arg, int argLength)
        {
            return -1;
        }
    }
}
