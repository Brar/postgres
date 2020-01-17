using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class TestHelper : IDisposable
    {
        private static readonly Random Random = new Random();
        private bool _disposed;
        private readonly Log _log;
        private readonly MemoryContext _memoryContext;
        private readonly List<IntPtr> _unmanagedMemoryPointers = new List<IntPtr>();
        private readonly uint _functionOidDefault = (uint)Random.Next(100000000, 999999999);
        private readonly string _functionNameDefault = "TestFunction";

        public TestHelper(ulong memoryContextSize = 8192UL)
        {
            _memoryContext = new MemoryContext(memoryContextSize);
            _log = new Log(MemoryContext.PFree);
        }

        public Log Log
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TestHelper));

                return _log;
            }
        }

        public MemoryContext MemoryContext
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TestHelper));

                return _memoryContext;
            }
        }

        public uint FunctionOidDefault
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TestHelper));

                return _functionOidDefault;
            }
        }

        public string FunctionNameDefault
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TestHelper));

                return _functionNameDefault;
            }
        }

        public (IntPtr unmanagedInterfacePointer, int unmanagedInterfaceSize) GetUnmanagedInterface()
        {
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = Log.GetELogFunctionPointer();
            i.PFreeFunctionPtr = MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = MemoryContext.GetPAlloc0FunctionPointer();
            i.PAllocFunctionPtr = MemoryContext.GetPAllocFunctionPointer();
            i.RePAllocFunctionPtr = MemoryContext.GetRePAllocFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(unmanagedInterfaceSize);
            _unmanagedMemoryPointers.Add(unmanagedInterfacePointer);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);
            return (unmanagedInterfacePointer, unmanagedInterfaceSize);
        }

        public (IntPtr functionCompileInfoPointer, int functionCompileInfoSize) GetFunctionCompileInfo(uint? oid = null, string? name = null, Argument[]? arguments = null, string? body = null, TypeOid returnValueType = TypeOid.Void, bool returnsSet = false)
        {
            var (unmanagedInterfacePointer, unmanagedInterfaceSize) = GetUnmanagedInterface();
            var managedInterfacePointer = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);
            MemoryContext.PFree(managedInterfacePointer);

            FunctionCompileInfo functionCompileInfo;
            functionCompileInfo.FunctionOid = oid ?? FunctionOidDefault;
            functionCompileInfo.FunctionNamePtr = Marshal.StringToPtrPalloc(name ?? FunctionNameDefault, true);
            functionCompileInfo.FunctionBodyPtr = Marshal.StringToPtrPalloc(body ?? string.Empty, true);
            if (arguments == null || arguments.Length == 0)
            {
                functionCompileInfo.NumberOfArguments = 0;
                functionCompileInfo.ArgumentTypes = IntPtr.Zero;
                functionCompileInfo.ArgumentNames = IntPtr.Zero;
                functionCompileInfo.ArgumentModes = IntPtr.Zero;
            }
            else
            {
                functionCompileInfo.NumberOfArguments = arguments.Length;
                (functionCompileInfo.ArgumentTypes, functionCompileInfo.ArgumentNames, functionCompileInfo.ArgumentModes) =
                    GetArgumentPointers(arguments);
            }
            functionCompileInfo.ReturnValueType = (uint)returnValueType;
            functionCompileInfo.ReturnsSet = returnsSet;

            var functionCompileInfoSize = System.Runtime.InteropServices.Marshal.SizeOf<FunctionCompileInfo>();
            var functionCompileInfoPointer = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(functionCompileInfoSize);
            _unmanagedMemoryPointers.Add(functionCompileInfoPointer);
            System.Runtime.InteropServices.Marshal.StructureToPtr(functionCompileInfo, functionCompileInfoPointer, false);

            return (functionCompileInfoPointer, functionCompileInfoSize);

            unsafe (IntPtr argumentTypesPtr, IntPtr argumentNamesPtr, IntPtr argumentModesPtr) GetArgumentPointers(Argument[] argumentInfos)
            {
                var nArgs = argumentInfos.Length;

                var argumentTypesPtr =
                    MemoryContext.PAlloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<uint>() * nArgs));
                new ReadOnlySpan<uint>(argumentInfos.Select(a=> (uint)a.Oid).ToArray()).CopyTo(new Span<uint>((void*)argumentTypesPtr, nArgs));

                var argumentNamesPtr = IntPtr.Zero;
                if (argumentInfos.Any(a => a.Name != null))
                {
                    argumentNamesPtr =
                        MemoryContext.PAlloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<IntPtr>() * nArgs));
                    new ReadOnlySpan<IntPtr>(argumentInfos.Select(a =>
                            Marshal.StringToPtrPalloc(a.Name, Environment.OSVersion.Platform == PlatformID.Win32NT))
                        .ToArray()).CopyTo(new Span<IntPtr>((void*) argumentNamesPtr, nArgs));
                }

                var argumentModesPtr = IntPtr.Zero;
                if (argumentInfos.Any(a => a.Mode != ArgumentMode.In))
                {
                    argumentModesPtr =
                        MemoryContext.PAlloc((ulong) (System.Runtime.InteropServices.Marshal.SizeOf<byte>() * nArgs));
                    new ReadOnlySpan<byte>(argumentInfos.Select(a=> (byte)a.Mode).ToArray()).CopyTo(new Span<byte>((void*)argumentModesPtr, nArgs));
                }

                return (argumentTypesPtr, argumentNamesPtr, argumentModesPtr);
            }
        }

        public IntPtr AllocCoTaskMem(int size)
        {
            var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(size);
            _unmanagedMemoryPointers.Add(ptr);
            return ptr;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _unmanagedMemoryPointers.ForEach(System.Runtime.InteropServices.Marshal.FreeCoTaskMem);
            MemoryContext.Dispose();
            Log.Dispose();
            _disposed = true;
        }
    }
}
