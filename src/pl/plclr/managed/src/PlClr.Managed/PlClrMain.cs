using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PlClr
{
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

        private static readonly Func<IntPtr, int, IntPtr> CompileDelegate = CompileFunction;

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
                if (argLength < System.Runtime.InteropServices.Marshal.SizeOf(typeof(ClrSetupInfo)))
                    return IntPtr.Zero;

                var clrSetupInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<ClrSetupInfo>(arg);

                if (clrSetupInfo.PallocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.PallocFunctionPtr)} in struct {nameof(ClrSetupInfo)} is NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.Palloc0FunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.Palloc0FunctionPtr)} in struct {nameof(ClrSetupInfo)} is NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.RePallocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.RePallocFunctionPtr)} in struct {nameof(ClrSetupInfo)} is NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.PFreeFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.PFreeFunctionPtr)} in struct {nameof(ClrSetupInfo)} is NULL");
                    return IntPtr.Zero;
                }
                if (clrSetupInfo.ELogFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(clrSetupInfo.ELogFunctionPtr)} in struct {nameof(ClrSetupInfo)} is NULL");
                    return IntPtr.Zero;
                }

                var palloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<Func<ulong, IntPtr>>(clrSetupInfo.PallocFunctionPtr);
                var palloc0 = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<Func<ulong, IntPtr>>(clrSetupInfo.Palloc0FunctionPtr);
                var repalloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<Func<IntPtr, ulong, IntPtr>>(clrSetupInfo.RePallocFunctionPtr);
                var pfree = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<Action<IntPtr>>(clrSetupInfo.PFreeFunctionPtr);
                var elog = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<Action<int, IntPtr>>(clrSetupInfo.ELogFunctionPtr);

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

        public static IntPtr CompileFunction(IntPtr arg, int argLength)
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
