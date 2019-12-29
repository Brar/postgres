using System;
using System.Runtime.InteropServices;

namespace PlClrManaged
{
    public static class Lib
    {
        private static int s_CallCount = 1;

        private static string MarshalToString(IntPtr arg, string argName)
        {
            if (arg == IntPtr.Zero)
            {
                throw new ArgumentNullException(argName);
            }

#if PLATFORM_WINDOWS
            string? result = Marshal.PtrToStringUni(arg);
#else
            string? result = Marshal.PtrToStringUTF8(arg);
#endif

            if (result == null)
            {
                throw new ArgumentNullException(argName);
            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LibArgs
        {
            public IntPtr Message;
            public int Number;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FunctionCompileInfo
        {
            public uint FunctionOid;
            public IntPtr FunctionName;
            public IntPtr FunctionBody;
        }

        public static int CompileAndExecute(IntPtr arg, int argLength)
        {
            if (argLength < Marshal.SizeOf(typeof(FunctionCompileInfo)))
            {
                return -1;
            }

            var compileInfo = Marshal.PtrToStructure<FunctionCompileInfo>(arg);
            var functionName = MarshalToString(compileInfo.FunctionName, nameof(compileInfo.FunctionName));
            var functionBody = MarshalToString(compileInfo.FunctionBody, nameof(compileInfo.FunctionBody));

            Console.WriteLine($"Compiling function {functionName} (Oid: {compileInfo.FunctionOid}) ...");
            Console.WriteLine($"Body: \"{functionBody}\"");
            Console.WriteLine($"done.");
            return 0;
        }

        public static int Hello(IntPtr arg, int argLength)
        {
            if (argLength < Marshal.SizeOf(typeof(LibArgs)))
            {
                return -1;
            }

            var libArgs = Marshal.PtrToStructure<LibArgs>(arg);
            var message = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Marshal.PtrToStringUni(libArgs.Message)
                : Marshal.PtrToStringUTF8(libArgs.Message);

            Console.WriteLine($"Hello, world! from {nameof(Lib)} [count: {s_CallCount++}]");
            Console.WriteLine($"-- message: {message}");
            Console.WriteLine($"-- number: {libArgs.Number}");
            return 0;
        }
    }
}
