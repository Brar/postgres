using System;
using System.Runtime.InteropServices;

namespace PlClr
{
    public delegate IntPtr PAllocDelegate(ulong size);
    public delegate IntPtr RePAllocDelegate(IntPtr ptr, ulong size);
    public delegate void PFreeDelegate(IntPtr ptr);
    public delegate void ELogDelegate(int level, IntPtr message);
    public delegate IntPtr CompileDelegate(IntPtr ptr, int size);
    public delegate IntPtr PlClrMainDelegate(IntPtr args, int sizeBytes);

    public static class PlClrMain
    {
        #region Private structs for marshalling

        // ReSharper disable FieldCanBeMadeReadOnly.Local

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileInfoPrivate
        {
            public uint FunctionOid;
            public IntPtr FunctionNamePtr;
            public IntPtr FunctionBodyPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ClrSetupInfo
        {
            public IntPtr PallocFunctionPtr;
            public IntPtr Palloc0FunctionPtr;
            public IntPtr RePallocFunctionPtr;
            public IntPtr PFreeFunctionPtr;
            public IntPtr ELogFunctionPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HostSetupInfo
        {
            public IntPtr CompileFunctionPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileResult
        {
            
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionExecuteResult
        {
            
        }

        // ReSharper restore FieldCanBeMadeReadOnly.Local

        #endregion

        private static readonly CompileDelegate CompileDelegate = Compile;

        /// <summary>
        /// This is the initial setup Method.
        /// It initializes all delegates that are used for communication
        /// between the clr an the backend.
        /// </summary>
        /// <param name="arg">Pointer to a <see cref="ClrSetupInfo"/> struct</param>
        /// <param name="argLength">The size of the passed <see cref="ClrSetupInfo"/> struct</param>
        /// <returns></returns>
        public static IntPtr Setup(IntPtr arg, int argLength)
        {
            try
            {
                if (arg == IntPtr.Zero)
                {
                    Console.Error.WriteLine(
                        $"Argument {nameof(arg)} must not be NULL");
                    return IntPtr.Zero;
                }

                if (argLength < System.Runtime.InteropServices.Marshal.SizeOf<ClrSetupInfo>())
                {
                    Console.Error.WriteLine(
                        $"Argument {nameof(argLength)} is {argLength} but is expected to be greater than or equal to {System.Runtime.InteropServices.Marshal.SizeOf<ClrSetupInfo>()}");
                    return IntPtr.Zero;
                }

                var clrSetupInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<ClrSetupInfo>(arg);

                if (clrSetupInfo.PallocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.PallocFunctionPtr)} in struct {nameof(ClrSetupInfo)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.Palloc0FunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.Palloc0FunctionPtr)} in struct {nameof(ClrSetupInfo)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.RePallocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.RePallocFunctionPtr)} in struct {nameof(ClrSetupInfo)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.PFreeFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.PFreeFunctionPtr)} in struct {nameof(ClrSetupInfo)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.ELogFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.ELogFunctionPtr)} in struct {nameof(ClrSetupInfo)} must not be NULL");
                    return IntPtr.Zero;
                }

                var palloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(clrSetupInfo.PallocFunctionPtr);
                var palloc0 = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(clrSetupInfo.Palloc0FunctionPtr);
                var repalloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<RePAllocDelegate>(clrSetupInfo.RePallocFunctionPtr);
                var pfree = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PFreeDelegate>(clrSetupInfo.PFreeFunctionPtr);
                var elog = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ELogDelegate>(clrSetupInfo.ELogFunctionPtr);

                ServerMemory.Initialize(palloc, palloc0, repalloc, pfree);
                ServerLog.Initialize(elog);

                HostSetupInfo hostSetupInfo;
                hostSetupInfo.CompileFunctionPtr =
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(CompileDelegate);

                var size = System.Runtime.InteropServices.Marshal.SizeOf<HostSetupInfo>();
                var ptr = ServerMemory.Palloc((ulong) size);
                System.Runtime.InteropServices.Marshal.StructureToPtr(hostSetupInfo, ptr, false);
                return ptr;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"An unexpected exception occured during PL/CLR setup: {e}");
                return IntPtr.Zero;
            }
        }

        public static IntPtr Compile(IntPtr arg, int argLength)
        {
            if (argLength < System.Runtime.InteropServices.Marshal.SizeOf(typeof(FunctionCompileInfoPrivate)))
                return IntPtr.Zero;

            var compileInfo = GetFunctionCompileInfo(arg);

            var methodInfo = CSharpCompiler.Compile(compileInfo);

            return IntPtr.Zero;

            static FunctionCompileInfo GetFunctionCompileInfo(IntPtr arg)
            {
                var ci = System.Runtime.InteropServices.Marshal.PtrToStructure<FunctionCompileInfoPrivate>(arg);

                var functionName = Marshal.PtrToStringPFree(ci.FunctionNamePtr, nameof(ci.FunctionNamePtr));
                if (functionName == null)
                {
                    ServerLog.ELog(SeverityLevel.Error, "The Function name must not be NULL");
                    // unreachable as Elog >= Error will tear down th process.
                    throw new Exception("Unreachable");
                }

                var functionBody = Marshal.PtrToStringPFree(ci.FunctionBodyPtr, nameof(ci.FunctionBodyPtr));
                if (functionBody == null)
                {
                    ServerLog.ELog(SeverityLevel.Error, "The Function name must not be NULL");
                    // unreachable as Elog >= Error will tear down th process.
                    throw new Exception("Unreachable");
                }

                return new FunctionCompileInfo(ci.FunctionOid,
                    functionName,
                    functionBody);
            }
        }
    }
}
