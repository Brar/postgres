using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
        }

        protected static SetupResult Setup(SetupParameters setupParameters, Func<IntPtr, int, IntPtr> setupFunc)
            => Setup(setupParameters, setupFunc, null, null).setupResult!;

        protected static CompileResult Compile(CompileParameters compileParameters, Func<IntPtr, int, IntPtr> compileFunc)
            => Setup(new SetupParameters(), PlClrMain.Setup, compileParameters, compileFunc).compileResult!;

        private static (SetupResult? setupResult, CompileResult? compileResult) Setup(SetupParameters setupParameters, Func<IntPtr, int, IntPtr> setupInfoFunc, CompileParameters? compileParameters, Func<IntPtr, int, IntPtr>? compileFunc)
        {
            var pallocPtrs = new Dictionary<IntPtr, ulong>();
            var palloc0Ptrs = new Dictionary<IntPtr, ulong>();
            var elogMessages = new List<(SeverityLevel, string?)>();
            var totalBytesPalloc = 0UL;
            var totalBytesPalloc0 = 0UL;
            var totalBytesRepalloc = 0UL;
            var totalBytesRepallocFree = 0UL;
            var totalBytesPfree = 0UL;

            IntPtr Palloc(ulong size)
            {
                totalBytesPalloc += size;
                var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int) size);
                pallocPtrs.Add(ptr, size);
                return ptr;
            }

            IntPtr Palloc0(ulong size)
            {
                totalBytesPalloc0 += size;
                var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int) size);
                palloc0Ptrs.Add(ptr, size);
                return ptr;
            }

            IntPtr Repalloc(IntPtr ptr, ulong size)
            {
                totalBytesRepalloc += size;
                var oldPtr = ptr;
                var newPtr = System.Runtime.InteropServices.Marshal.ReAllocCoTaskMem(ptr, (int) size);
                if (pallocPtrs.ContainsKey(oldPtr))
                {
                    totalBytesRepallocFree += pallocPtrs[oldPtr];
                    pallocPtrs.Remove(oldPtr);
                    pallocPtrs.Add(newPtr, size);
                }

                if (palloc0Ptrs.ContainsKey(oldPtr))
                {
                    palloc0Ptrs.Remove(oldPtr);
                    palloc0Ptrs.Add(newPtr, size);
                }

                return newPtr;
            }

            void Pfree(IntPtr ptr)
            {
                if (ptr == IntPtr.Zero) return;

                var oldPtr = ptr;
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                if (pallocPtrs.ContainsKey(oldPtr))
                {
                    totalBytesPfree += pallocPtrs[oldPtr];
                    pallocPtrs.Remove(oldPtr);
                }
                else if (palloc0Ptrs.ContainsKey(oldPtr))
                {
                    totalBytesPfree += pallocPtrs[oldPtr];
                    palloc0Ptrs.Remove(oldPtr);
                }
            }

            void Elog(int level, IntPtr message)
            {
                var msg = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(message);
                elogMessages.Add(((SeverityLevel) level, msg));

                // We pfree the string here since we've allocated it via palloc
                // in PlCLR.Managed.
                // Otherwise our statistics would get messed up.
                Pfree(message);
            }

            ClrSetupInfoPrivate setupInfo;
            setupInfo.PallocFunctionPtr = setupParameters.PallocFunctionPtrIsNull ? IntPtr.Zero : 
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PAllocDelegate>(Palloc);
            setupInfo.Palloc0FunctionPtr = setupParameters.Palloc0FunctionPtrIsNull ? IntPtr.Zero : 
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PAllocDelegate>(Palloc0);
            setupInfo.RePallocFunctionPtr = setupParameters.RePallocFunctionPtrIsNull ? IntPtr.Zero : 
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<RePAllocDelegate>(Repalloc);
            setupInfo.PFreeFunctionPtr = setupParameters.PFreeFunctionPtrIsNull ? IntPtr.Zero : 
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PFreeDelegate>(Pfree);
            setupInfo.ELogFunctionPtr = setupParameters.ELogFunctionPtrIsNull ? IntPtr.Zero : 
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ELogDelegate>(Elog);

            var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal
                .SizeOf<ClrSetupInfoPrivate>());
            System.Runtime.InteropServices.Marshal.StructureToPtr(setupInfo, ptr, false);

            var outBackup = Console.Out;
            var errorBackup = Console.Error;
            try
            {
                using var consoleOut = new StringWriter();
                using var consoleError = new StringWriter();
                Console.SetOut(consoleOut);
                Console.SetError(consoleError);

                var setupResultPtr = setupInfoFunc(ptr, System.Runtime.InteropServices.Marshal.SizeOf<ClrSetupInfoPrivate>());

                if (setupResultPtr == IntPtr.Zero)
                    return CreateResult(false);

                var hostSetupInfo =
                    System.Runtime.InteropServices.Marshal.PtrToStructure<HostSetupInfoPrivate>(setupResultPtr);
                Pfree(setupResultPtr);

                Assert.That(hostSetupInfo.CompileFunctionPtr, Is.Not.EqualTo(IntPtr.Zero));

                var d = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<CompileDelegate>(
                    hostSetupInfo.CompileFunctionPtr);

                Assert.That(d, Is.EqualTo((CompileDelegate)PlClrMain.Compile));

                Assert.That(pallocPtrs, Is.Empty);
                Assert.That(palloc0Ptrs, Is.Empty);

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

                var fciPtr = ServerMemory.Palloc((ulong)System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
                System.Runtime.InteropServices.Marshal.StructureToPtr(fci, fciPtr, false);

                var compileResultPtr = compileFunc(fciPtr, System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
                if (compileResultPtr == IntPtr.Zero)
                    return CreateResult(true);

                // ToDo: At the moment Compile doesn't return anything useful but this has to change next

                return CreateResult(true);

                (SetupResult? setupResult, CompileResult? compileResult) CreateResult(bool createCompileResult)
                    => createCompileResult ? ((SetupResult?)null, new CompileResult()) : (new SetupResult(elogMessages, totalBytesPalloc, totalBytesPalloc0, totalBytesRepalloc,
                        totalBytesRepallocFree, totalBytesPfree, consoleOut.ToString(), consoleError.ToString()), (CompileResult?)null);
            }
            finally
            {
                foreach (var p in pallocPtrs.Keys)
                {
                    pallocPtrs.Remove(ptr);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                }
                foreach (var p in palloc0Ptrs.Keys)
                {
                    palloc0Ptrs.Remove(ptr);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                }
                Console.SetOut(outBackup);
                Console.SetError(errorBackup);
            }
        }

    }
}
