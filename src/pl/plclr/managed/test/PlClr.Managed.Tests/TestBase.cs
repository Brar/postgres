using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace PlClr.Managed.Tests
{
    public abstract class TestBase
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileInfoPrivate
        {
            public uint FunctionOid;
            public IntPtr FunctionNamePtr;
            public IntPtr FunctionBodyPtr;
            public uint ReturnValueType;
            public bool ReturnsSet;
            public int NumberOfArguments;
            public IntPtr ArgumentTypes;
            public IntPtr ArgumentNames;
            public IntPtr ArgumentModes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ClrSetupInfoPrivate
        {
            public IntPtr PallocFunctionPtr;
            public IntPtr Palloc0FunctionPtr;
            public IntPtr RePallocFunctionPtr;
            public IntPtr PFreeFunctionPtr;
            public IntPtr ELogFunctionPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HostSetupInfoPrivate
        {
            public IntPtr CompileFunctionPtr;
            public IntPtr ExecuteFunctionPtr;
        }

        
        [StructLayout(LayoutKind.Sequential)]
        private struct FunctionCompileResultPrivate
        {
            public IntPtr ExecuteDelegatePtr;
        }

        private sealed class MemoryManager : IDisposable
        {
            private readonly TextWriter _outBackup;
            private readonly TextWriter _errorBackup;
            private readonly Dictionary<IntPtr, ulong> _pallocPtrs;
            private readonly Dictionary<IntPtr, ulong> _palloc0Ptrs;
            private readonly List<(SeverityLevel, string?)> _elogMessages;
            private readonly TextWriter _consoleOut;
            private readonly TextWriter _consoleError;

            private MemoryManager()
            {
                _outBackup = Console.Out;
                _errorBackup = Console.Error;
                _pallocPtrs = new Dictionary<IntPtr, ulong>();
                _palloc0Ptrs = new Dictionary<IntPtr, ulong>();
                _elogMessages = new List<(SeverityLevel, string?)>();
                _consoleOut = new StringWriter();
                _consoleError = new StringWriter();
                Console.SetOut(_consoleOut);
                Console.SetError(_consoleError);
            }

            public static MemoryManager SetUp() => new MemoryManager();

            public string ConsoleOut => _consoleOut.ToString()!;
            public string ConsoleError => _consoleError.ToString()!;
            public ulong TotalBytesPalloc { get; private set; }
            public ulong TotalBytesPalloc0 { get; private set; }
            public ulong TotalBytesRepalloc { get; private set; }
            public ulong TotalBytesRepallocFree { get; private set; }
            public ulong TotalBytesPfree { get; private set; }
            public IEnumerable<(SeverityLevel, string?)> ELogMessages => _elogMessages;

            public IntPtr Palloc(ulong size)
            {
                TotalBytesPalloc += size;
                var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int) size);
                Debug.WriteLine($"Palloc of {size} bytes at {ptr.ToInt64():x}");
                _pallocPtrs.Add(ptr, size);
                return ptr;
            }

            public IntPtr Palloc0(ulong size)
            {
                TotalBytesPalloc0 += size;
                var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int) size);
                Debug.WriteLine($"Palloc0 of {size} bytes at {ptr.ToInt64():x}");
                _palloc0Ptrs.Add(ptr, size);
                return ptr;
            }

            public IntPtr Repalloc(IntPtr ptr, ulong size)
            {
                TotalBytesRepalloc += size;
                var newPtr = System.Runtime.InteropServices.Marshal.ReAllocCoTaskMem(ptr, (int) size);
                if (_pallocPtrs.ContainsKey(ptr))
                {
                    Debug.WriteLine($"Repalloc from {_pallocPtrs[ptr]} bytes at {ptr.ToInt64():x} to {size} bytes at {newPtr.ToInt64():x} (originally allocated via Palloc)");
                    TotalBytesRepallocFree += _pallocPtrs[ptr];
                    _pallocPtrs.Remove(ptr);
                    _pallocPtrs.Add(newPtr, size);
                }
                else if (_palloc0Ptrs.ContainsKey(ptr))
                {
                    Debug.WriteLine($"Repalloc from {_palloc0Ptrs[ptr]} bytes at {ptr.ToInt64():x} to {size} bytes at {newPtr.ToInt64():x} (originally allocated via Palloc)");
                    TotalBytesRepallocFree += _palloc0Ptrs[ptr];
                    _palloc0Ptrs.Remove(ptr);
                    _palloc0Ptrs.Add(newPtr, size);
                }

                return newPtr;
            }

            public void Pfree(IntPtr ptr)
            {
                if (ptr == IntPtr.Zero) return;

                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                if (_pallocPtrs.ContainsKey(ptr))
                {
                    Debug.WriteLine($"Pfree of {_pallocPtrs[ptr]} bytes at {ptr.ToInt64():x} (allocated via Palloc)");
                    TotalBytesPfree += _pallocPtrs[ptr];
                    _pallocPtrs.Remove(ptr);
                }
                else if (_palloc0Ptrs.ContainsKey(ptr))
                {
                    Debug.WriteLine($"Pfree of {_palloc0Ptrs[ptr]} bytes at {ptr.ToInt64():x} (allocated via Palloc0)");
                    TotalBytesPfree += _pallocPtrs[ptr];
                    _palloc0Ptrs.Remove(ptr);
                }
            }

            public void Elog(int level, IntPtr message)
            {
                var msg = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(message);
                _elogMessages.Add(((SeverityLevel) level, msg));

                // We pfree the string here since we've allocated it via palloc
                // in PlCLR.Managed.
                // Otherwise our statistics would get messed up.
                Pfree(message);
            }

            public void Dispose()
            {
                foreach (var p in _pallocPtrs.Keys)
                {
                    Debug.WriteLine($"Cleaning up {_pallocPtrs[p]} bytes from palloc.");
                    _pallocPtrs.Remove(p);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(p);
                }
                foreach (var p in _palloc0Ptrs.Keys)
                {
                    Debug.WriteLine($"Cleaning up {_palloc0Ptrs[p]} bytes from palloc0.");
                    _palloc0Ptrs.Remove(p);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(p);
                }
                Console.SetOut(_outBackup);
                Console.SetError(_errorBackup);
                _consoleOut.Dispose();
                _consoleError.Dispose();
            }
        }

        protected SetupResult Setup(SetupParameters setupParameters, Func<IntPtr, int, IntPtr> setupFunc)
            => Setup(setupParameters, setupFunc, null, null).setupResult!;

        protected CompileResult Compile(CompileParameters compileParameters, Func<IntPtr, int, IntPtr> compileFunc)
            => Setup(new SetupParameters(), PlClrMain.Setup, compileParameters, compileFunc).compileResult!;

        private (SetupResult? setupResult, CompileResult? compileResult) Setup(SetupParameters setupParameters, Func<IntPtr, int, IntPtr> setupInfoFunc, CompileParameters? compileParameters, Func<IntPtr, int, IntPtr>? compileFunc)
        {
            using var memory = MemoryManager.SetUp();
            var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal
                .SizeOf<ClrSetupInfoPrivate>());

            try
            {
                ClrSetupInfoPrivate setupInfo;
                setupInfo.PallocFunctionPtr = setupParameters.PallocFunctionPtrIsNull ? IntPtr.Zero : 
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PAllocDelegate>(memory.Palloc);
                setupInfo.Palloc0FunctionPtr = setupParameters.Palloc0FunctionPtrIsNull ? IntPtr.Zero : 
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PAllocDelegate>(memory.Palloc0);
                setupInfo.RePallocFunctionPtr = setupParameters.RePallocFunctionPtrIsNull ? IntPtr.Zero : 
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<RePAllocDelegate>(memory.Repalloc);
                setupInfo.PFreeFunctionPtr = setupParameters.PFreeFunctionPtrIsNull ? IntPtr.Zero : 
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PFreeDelegate>(memory.Pfree);
                setupInfo.ELogFunctionPtr = setupParameters.ELogFunctionPtrIsNull ? IntPtr.Zero : 
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ELogDelegate>(memory.Elog);
                System.Runtime.InteropServices.Marshal.StructureToPtr(setupInfo, ptr, false);

                var setupResultPtr = setupInfoFunc(ptr, System.Runtime.InteropServices.Marshal.SizeOf<ClrSetupInfoPrivate>());

                if (setupResultPtr == IntPtr.Zero)
                    return CreateResult(false);

                var hostSetupInfo =
                    System.Runtime.InteropServices.Marshal.PtrToStructure<HostSetupInfoPrivate>(setupResultPtr);
                memory.Pfree(setupResultPtr);

                Assert.NotEqual(IntPtr.Zero, hostSetupInfo.CompileFunctionPtr);
                var compile = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PlClrMainDelegate>(
                    hostSetupInfo.CompileFunctionPtr);
                Assert.Equal(PlClrMain.Compile, compile);

                Assert.NotEqual(IntPtr.Zero, hostSetupInfo.ExecuteFunctionPtr);
                var execute = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PlClrMainDelegate>(
                    hostSetupInfo.ExecuteFunctionPtr);
                Assert.Equal(PlClrMain.Execute, execute);

                // If we reach this point, Setup has completed successfully and we can compile
                // At this point could use our delegate d() or PlClrMain.Compile() but the unmanaged
                // code obviously has to use the delegate.
                // For the Setup tests we're done here but for the Compile tests, everything until now
                // has happened silently in the background and this is the place where we call our compileFunc
                if (compileFunc == null || compileParameters == null)
                    return CreateResult(false);

                FunctionCompileInfoPrivate fci;
                fci.FunctionOid = compileParameters.FunctionOid;

                // We have to palloc ClrStrings (UTF-16LE on Windows, UTF-8 elsewhere) here as this is what
                // the managed part of PlClr expects whenever it receives a string from the unmanaged world
                fci.FunctionNamePtr = Marshal.StringToPtrPalloc(compileParameters.FunctionName, true);
                fci.FunctionBodyPtr = Marshal.StringToPtrPalloc(compileParameters.FunctionBody, true);
                fci.NumberOfArguments = compileParameters.CompileArgumentInfos.Length;

                if (fci.NumberOfArguments > 0)
                {
                    (fci.ArgumentTypes, fci.ArgumentNames, fci.ArgumentModes) =
                        GetArgumentPointers(compileParameters.CompileArgumentInfos);
                }
                else
                {
                    fci.ArgumentTypes = IntPtr.Zero;
                    fci.ArgumentNames = IntPtr.Zero;
                    fci.ArgumentModes = IntPtr.Zero;
                }

                fci.ReturnValueType = (uint)compileParameters.ReturnValueType;
                fci.ReturnsSet = compileParameters.ReturnsSet;

                var fciPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
                try
                {
                    System.Runtime.InteropServices.Marshal.StructureToPtr(fci, fciPtr, false);

                    var compileResultPtr = compileFunc(fciPtr, System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
                    if (compileResultPtr == IntPtr.Zero)
                        return CreateResult(true);

                    var compileResult =
                        System.Runtime.InteropServices.Marshal.PtrToStructure<FunctionCompileResultPrivate>(
                            compileResultPtr);
                    memory.Pfree(compileResultPtr);

                    Assert.NotEqual(IntPtr.Zero, compileResult.ExecuteDelegatePtr);
                    var executeDelegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<FunctionCallDelegate>(compileResult.ExecuteDelegatePtr);

                    // ToDo: At the moment Compile doesn't return anything useful but this has to change next

                    return CreateResult(true);

                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fciPtr);
                }

                (SetupResult? setupResult, CompileResult? compileResult) CreateResult(bool createCompileResult)
                    => createCompileResult
                        ? ((SetupResult?) null,
                            new CompileResult(memory.ELogMessages, memory.TotalBytesPalloc, memory.TotalBytesPalloc0,
                                memory.TotalBytesRepalloc, memory.TotalBytesRepallocFree, memory.TotalBytesPfree,
                                memory.ConsoleOut, memory.ConsoleError))
                        : (
                            new SetupResult(memory.ELogMessages, memory.TotalBytesPalloc, memory.TotalBytesPalloc0,
                                memory.TotalBytesRepalloc, memory.TotalBytesRepallocFree, memory.TotalBytesPfree,
                                memory.ConsoleOut, memory.ConsoleError), (CompileResult?) null);

                unsafe (IntPtr argumentTypesPtr, IntPtr argumentNamesPtr, IntPtr argumentModesPtr) GetArgumentPointers(IReadOnlyList<CompileArgumentInfo> argumentInfos)
                {
                    var nArgs = argumentInfos.Count;

                    var argumentTypesPtr =
                        memory.Palloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<uint>() * nArgs));
                    new ReadOnlySpan<uint>(argumentInfos.Select(a=> (uint)a.Oid).ToArray()).CopyTo(new Span<uint>((void*)argumentTypesPtr, nArgs));

                    var argumentNamesPtr = IntPtr.Zero;
                    if (argumentInfos.Any(a => a.Name != null))
                    {
                        argumentNamesPtr =
                            memory.Palloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<IntPtr>() * nArgs));
                        new ReadOnlySpan<IntPtr>(argumentInfos.Select(a =>
                                Marshal.StringToPtrPalloc(a.Name, Environment.OSVersion.Platform == PlatformID.Win32NT))
                            .ToArray()).CopyTo(new Span<IntPtr>((void*) argumentNamesPtr, nArgs));
                    }

                    var argumentModesPtr = IntPtr.Zero;
                    if (argumentInfos.Any(a => a.Mode != ArgumentMode.In))
                    {
                        argumentModesPtr =
                            memory.Palloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<byte>() * nArgs));
                        new ReadOnlySpan<byte>(argumentInfos.Select(a=> (byte)a.Mode).ToArray()).CopyTo(new Span<byte>((void*)argumentModesPtr, nArgs));
                    }

                    return (argumentTypesPtr, argumentNamesPtr, argumentModesPtr);
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                memory.Dispose();
            }
        }

    }
}
