﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("PlClr.Managed.Tests")]

namespace PlClr
{
    public delegate IntPtr PAllocDelegate(ulong size);
    public delegate IntPtr RePAllocDelegate(IntPtr ptr, ulong size);
    public delegate void PFreeDelegate(IntPtr ptr);
    public delegate void ELogDelegate(int level, IntPtr message);
    public delegate void EReportDelegate(int level, IntPtr errorMessageInternal, IntPtr errorCode, IntPtr errorDetailInternal, IntPtr errorDetailLog, IntPtr errorHint, IntPtr errorPosition, IntPtr errorDataType);
    public delegate IntPtr PlClrMainDelegate(IntPtr args, int sizeBytes);
    public delegate IntPtr FunctionCallDelegate(NullableDatum[] values);
    public delegate IntPtr GetTypeInfoDelegate(uint value);

    public static class PlClrMain
    {
        #region Private structs for marshalling

        // ReSharper disable FieldCanBeMadeReadOnly.Local

        [StructLayout(LayoutKind.Sequential)]
        private struct PlClrUnmanagedInterface
        {
            /* palloc functions */
            public IntPtr PAllocFunctionPtr;
            public IntPtr PAlloc0FunctionPtr;
            public IntPtr RePAllocFunctionPtr;
            public IntPtr PFreeFunctionPtr;

            /* logging functions */
            public IntPtr ELogFunctionPtr;
            public IntPtr EReportFunctionPtr;

            /* type I/O */
            public IntPtr GetTypeInfoFunctionPtr;
            public IntPtr GetTextFunctionPtr;
            public IntPtr SetTextFunctionPtr;
            public IntPtr DeToastDatumFunctionPtr;
            public IntPtr GetAttributeByNumFunctionPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PlClrManagedInterface
        {
            public IntPtr CompileFunctionPtr;
            public IntPtr ExecuteFunctionPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileInfoPrivate
        {
            public uint FunctionOid;
            public IntPtr FunctionNamePtr;
            public IntPtr FunctionBodyPtr;
            public uint ReturnValueType;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ReturnsSet;
            [MarshalAs(UnmanagedType.Bool)]
            public bool IsStrict;
            public int NumberOfArguments;
            public IntPtr ArgumentTypes;
            public IntPtr ArgumentNames;
            public IntPtr ArgumentModes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileResult
        {
            public IntPtr ExecuteDelegatePtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCallInfoPrivate
        {
            public IntPtr ExecuteDelegatePtr;
            public int NumberOfArguments;
            public IntPtr ArgumentValues;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionExecuteResult
        {
            
        }

        // ReSharper restore FieldCanBeMadeReadOnly.Local

        #endregion

        private static readonly PlClrMainDelegate CompileDelegate = Compile;
        private static readonly PlClrMainDelegate ExecuteDelegate = Execute;
        private static readonly Dictionary<uint, FunctionCallDelegate> executeDelegates = new Dictionary<uint, FunctionCallDelegate>();

        /// <summary>
        /// This is the initial setup Method.
        /// It initializes all delegates that are used for communication
        /// between the clr an the backend.
        /// </summary>
        /// <param name="arg">Pointer to a <see cref="PlClrUnmanagedInterface"/> struct</param>
        /// <param name="argLength">The size of the passed <see cref="PlClrUnmanagedInterface"/> struct</param>
        /// <returns></returns>
        public static IntPtr Setup(IntPtr arg, int argLength)
        {
            var ptr = IntPtr.Zero;
            try
            {
                if (arg == IntPtr.Zero)
                {
                    Console.Error.WriteLine(
                        $"Argument {nameof(arg)} must not be NULL");
                    return IntPtr.Zero;
                }

                if (argLength < System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>())
                {
                    Console.Error.WriteLine(
                        $"Argument {nameof(argLength)} is {argLength} but is expected to be greater than or equal to {System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>()}");
                    return IntPtr.Zero;
                }

                var unmanagedInterface = System.Runtime.InteropServices.Marshal.PtrToStructure<PlClrUnmanagedInterface>(arg);

                if (unmanagedInterface.PAllocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.PAllocFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.PAlloc0FunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.PAlloc0FunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.RePAllocFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.RePAllocFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.PFreeFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.PFreeFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.ELogFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.ELogFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.GetTextFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.GetTextFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.SetTextFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.SetTextFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }
                if (unmanagedInterface.GetTypeInfoFunctionPtr == IntPtr.Zero)
                {
                    Console.Error.WriteLine($"Field {nameof(PlClrUnmanagedInterface.GetTypeInfoFunctionPtr)} in struct {nameof(PlClrUnmanagedInterface)} must not be NULL");
                    return IntPtr.Zero;
                }

                var palloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(unmanagedInterface.PAllocFunctionPtr);
                var palloc0 = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(unmanagedInterface.PAlloc0FunctionPtr);
                var repalloc = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<RePAllocDelegate>(unmanagedInterface.RePAllocFunctionPtr);
                var pfree = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PFreeDelegate>(unmanagedInterface.PFreeFunctionPtr);
                var elog = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ELogDelegate>(unmanagedInterface.ELogFunctionPtr);
                var ereport = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<EReportDelegate>(unmanagedInterface.EReportFunctionPtr);
                var getText = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ReferenceTypeConversionDelegate>(unmanagedInterface.GetTextFunctionPtr);
                var setText = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ReferenceTypeConversionDelegate>(unmanagedInterface.SetTextFunctionPtr);
                var getTypeInfo = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<GetTypeInfoDelegate>(unmanagedInterface.GetTypeInfoFunctionPtr);
                var deToastDatum = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ReferenceTypeConversionDelegate>(unmanagedInterface.DeToastDatumFunctionPtr);
                var getAttributeByNum = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<GetAttributeByNumDelegate>(unmanagedInterface.GetAttributeByNumFunctionPtr);

                ServerMemory.Initialize(palloc, palloc0, repalloc, pfree);
                ServerLog.Initialize(elog, ereport);
                ServerFunctions.Initialize(getText, setText, deToastDatum, getAttributeByNum, getTypeInfo);

                PlClrManagedInterface managedInterface;
                managedInterface.CompileFunctionPtr =
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(CompileDelegate);
                managedInterface.ExecuteFunctionPtr =
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(ExecuteDelegate);

                var size = System.Runtime.InteropServices.Marshal.SizeOf<PlClrManagedInterface>();

                // We shall not palloc here because our managed interface needs to live as long as the process lives.
                ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                System.Runtime.InteropServices.Marshal.StructureToPtr(managedInterface, ptr, false);
                
                return ptr;
            }
            catch (Exception e)
            {
                if (ptr != IntPtr.Zero) System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                Console.Error.WriteLine($"An unexpected exception occured during PL/CLR setup: {e}");
                return IntPtr.Zero;
            }
        }

        public static IntPtr Compile(IntPtr arg, int argLength)
        {
            try
            {
                if (arg == IntPtr.Zero)
                {
                    ServerLog.ELog(SeverityLevel.Error, $"Argument {nameof(arg)} must not be NULL");
                    return IntPtr.Zero;
                }

                if (argLength < System.Runtime.InteropServices.Marshal.SizeOf(typeof(FunctionCompileInfoPrivate)))
                {
                    ServerLog.ELog(SeverityLevel.Error,
                        $"Argument {nameof(argLength)} is {argLength} but is expected to be greater than or equal to {System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>()}");
                    return IntPtr.Zero;
                }

                var compileInfo = GetFunctionCompileInfo(arg);

                var executeDelegate = CSharpCompiler.Compile(compileInfo);
                if (executeDelegate == null)
                {
                    return IntPtr.Zero;
                }

                // Keep a reference to the delegate so that it doesn't get garbage collected
                executeDelegates[compileInfo.FunctionOid] = executeDelegate;

                FunctionCompileResult result;
                result.ExecuteDelegatePtr =
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(executeDelegate);
                var ret = ServerMemory.Palloc((ulong)System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileResult>());
                System.Runtime.InteropServices.Marshal.StructureToPtr(result, ret, false);
                return ret;

                static unsafe FunctionCompileInfo GetFunctionCompileInfo(IntPtr arg)
                {
                    var ci = System.Runtime.InteropServices.Marshal.PtrToStructure<FunctionCompileInfoPrivate>(arg);

                    var functionName = Marshal.ToStringPFree(ci.FunctionNamePtr);
                    if (functionName == null)
                    {
                        ServerLog.ELog(SeverityLevel.Error, "The Function name must not be NULL");
                        // unreachable as Elog >= Error will tear down the process.
                        throw new Exception("Unreachable");
                    }

                    var functionBody = Marshal.ToStringPFree(ci.FunctionBodyPtr);
                    if (functionBody == null)
                    {
                        ServerLog.ELog(SeverityLevel.Error, "The Function body must not be NULL");
                        // unreachable as Elog >= Error will tear down the process.
                        throw new Exception("Unreachable");
                    }

                    var nArgs = ci.NumberOfArguments;

                    if (nArgs == 0)
                        return new FunctionCompileInfo(ci.FunctionOid, functionName, functionBody, ci.ReturnValueType, ci.ReturnsSet, ci.IsStrict);

                    if (ci.ArgumentTypes == IntPtr.Zero)
                    {
                        ServerLog.ELog(SeverityLevel.Error, $"The Function has {nArgs} Arguments but the array of argument types is empty");
                        // unreachable as Elog >= Error will tear down the process.
                        throw new Exception("Unreachable");
                    }

                    var argTypes = new uint[nArgs];
                    new ReadOnlySpan<uint>((void*)ci.ArgumentTypes, nArgs).CopyTo(new Span<uint>(argTypes, 0, nArgs));
                    ServerMemory.PFree(ci.ArgumentTypes);

                    string[]? argNames = null;
                    if (ci.ArgumentNames != IntPtr.Zero)
                    {
                        var argNamePtrs = new IntPtr[nArgs];
                        new ReadOnlySpan<IntPtr>((void*)ci.ArgumentNames, nArgs).CopyTo(new Span<IntPtr>(argNamePtrs, 0, nArgs));
                        argNames = new string[nArgs];
                        for (var i = 0; i < nArgs; i++)
                        {
                            // Missing argument names are an empty string not null so we don't expect null here
                            argNames[i] = Marshal.ToStringPFree(argNamePtrs[i])!;
                        }
                        ServerMemory.PFree(ci.ArgumentNames);
                    }

                    byte[]? argModes = null;
                    if (ci.ArgumentModes != IntPtr.Zero)
                    {
                        argModes = new byte[nArgs];
                        new ReadOnlySpan<byte>((void*)ci.ArgumentModes, nArgs).CopyTo(new Span<byte>(argModes, 0, nArgs));
                        ServerMemory.PFree(ci.ArgumentModes);
                    }

                    return new FunctionCompileInfo(ci.FunctionOid, functionName, functionBody, ci.ReturnValueType, ci.ReturnsSet, ci.IsStrict, nArgs, argTypes, argNames,
                        argModes);
                }
            }
            catch (Exception e)
            {
                ServerLog.ELog(SeverityLevel.Error, $"An unexpected exception occured during PL/CLR compilation: {e}");
                return IntPtr.Zero;
            }
        }

        public static unsafe IntPtr Execute(IntPtr arg, int argLength)
        {
            try
            {
                var ci = System.Runtime.InteropServices.Marshal.PtrToStructure<FunctionCallInfoPrivate>(arg);
                var nArgs = ci.NumberOfArguments;
                var callDelegate =
                    System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<FunctionCallDelegate>(
                        ci.ExecuteDelegatePtr);

                var values = new NullableDatum[nArgs];
                if (nArgs > 0)
                    new ReadOnlySpan<NullableDatum>((void*) ci.ArgumentValues, nArgs).CopyTo(new Span<NullableDatum>(values, 0, nArgs));

                return callDelegate(values);
            }
            catch (Exception e)
            {
                ServerLog.ELog(SeverityLevel.Error, $"An unexpected exception occured during PL/CLR execution: {e}");
                return IntPtr.Zero;
            }
        }
    }
}
