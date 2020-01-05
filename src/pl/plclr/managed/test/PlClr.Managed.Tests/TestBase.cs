using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

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

        private delegate IntPtr PallocDelegate(ulong size);
        private delegate IntPtr RepallocDelegate(IntPtr ptr, ulong size);
        private delegate void PfreeDelegate(IntPtr ptr);
        private delegate void ElogDelegate(int level, IntPtr message);

        protected static SetupResult Setup(Func<IntPtr, int, IntPtr> setupInfoAction)
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
            setupInfo.PallocFunctionPtr =
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PallocDelegate>(Palloc);
            setupInfo.Palloc0FunctionPtr =
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PallocDelegate>(Palloc0);
            setupInfo.RePallocFunctionPtr =
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<RepallocDelegate>(Repalloc);
            setupInfo.PFreeFunctionPtr =
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<PfreeDelegate>(Pfree);
            setupInfo.ELogFunctionPtr =
                System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ElogDelegate>(Elog);

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

                setupInfoAction(ptr, System.Runtime.InteropServices.Marshal.SizeOf<ClrSetupInfoPrivate>());

                Assert.That(pallocPtrs, Is.Empty);
                Assert.That(palloc0Ptrs, Is.Empty);

                return new SetupResult(elogMessages, totalBytesPalloc, totalBytesPalloc0, totalBytesRepalloc,
                    totalBytesRepallocFree, totalBytesPfree, consoleOut.ToString(), consoleError.ToString());
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

        protected static void FunctionCompileInfo(uint oid, string functionName, string functionBody, Action<IntPtr, int> functionCompileInfo)
        {
            FunctionCompileInfoPrivate fci;
            fci.FunctionOid = oid;
            fci.FunctionNamePtr = StringToPointer(functionName);
            fci.FunctionBodyPtr = StringToPointer(functionBody);

            var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
            System.Runtime.InteropServices.Marshal.StructureToPtr(fci, ptr, false);

            try
            {
                functionCompileInfo(ptr, System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfoPrivate>());
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fci.FunctionNamePtr);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fci.FunctionBodyPtr);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
            }

            static IntPtr StringToPointer(string value)
                => Environment.OSVersion.Platform == PlatformID.Win32NT ? System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(value) : System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(value);
        }
    }
}
