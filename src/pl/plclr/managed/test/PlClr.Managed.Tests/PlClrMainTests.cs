using System;
using System.Diagnostics;
using Xunit;

namespace PlClr.Managed.Tests
{
    public class PlClrMainTests : TestBase
    {
        [Fact]
        public void SetupSuccess()
        {
            var p = new SetupParameters();

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.NotEqual(IntPtr.Zero, res);
                return res;
            });


            var totalAllocatedBytes = (ulong)(2 * System.Runtime.InteropServices.Marshal.SizeOf<IntPtr>());

            Assert.Equal(totalAllocatedBytes, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(totalAllocatedBytes, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Empty(result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNullPtr()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(IntPtr.Zero, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Argument arg must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupZeroLength()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, 0);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Argument argLength is 0 but is expected to be greater than or equal to 40{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNoPallocPtr()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Field PallocFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNoPalloc0Ptr()
        {
            var p = new SetupParameters
            {
                Palloc0FunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Field Palloc0FunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNoRePallocPtr()
        {
            var p = new SetupParameters
            {
                RePallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Field RePallocFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNoPFreePtr()
        {
            var p = new SetupParameters
            {
                PFreeFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Field PFreeFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void SetupNoELogPtr()
        {
            var p = new SetupParameters
            {
                ELogFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.Equal(IntPtr.Zero, res);
                return res;
            });

            Assert.Equal(0UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(0UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Equal($"Field ELogFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}", result.StdErr);
            Assert.Empty(result.ElogMessages);
        }

        [Fact]
        public void CompileSimpleSuccess()
        {
            Debug.WriteLine($"Running test {nameof(CompileSimpleSuccess)}...");
            var p = new CompileParameters(123U, "myFunc", "/* 😇 */");

            var result = Compile(p, (ptr, len) =>
            {
                var res = PlClrMain.Compile(ptr, len);
                Assert.NotEqual(IntPtr.Zero, res);
                return res;
            });

            Debug.WriteLine($"Starting assertions for test {nameof(CompileSimpleSuccess)}...");
            Assert.Equal(56UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(56UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Empty(result.StdErr);
            Assert.Empty(result.ElogMessages);
            Debug.WriteLine($"Finished test {nameof(CompileSimpleSuccess)}.");
        }

        [Fact]
        public void CompileInt32InArgsWithoutNameSuccess()
        {
            Debug.WriteLine($"Running test {nameof(CompileInt32InArgsWithoutNameSuccess)}...");
            var p = new CompileParameters(234U, "add", "if (arg1 == null || arg2 == null)\n\treturn null;\n\nreturn arg1 + arg2;", new []{ new CompileArgumentInfo(HardcodedOid.Int32), new CompileArgumentInfo(HardcodedOid.Int32) }, HardcodedOid.Int32);

            var result = Compile(p, (ptr, len) =>
            {
                var res = PlClrMain.Compile(ptr, len);
                Assert.NotEqual(IntPtr.Zero, res);
                return res;
            });

            Debug.WriteLine($"Starting assertions for test {nameof(CompileInt32InArgsWithoutNameSuccess)}...");
            Assert.Equal(178UL, result.TotalBytesPalloc);
            Assert.Equal(0UL, result.TotalBytesPalloc0);
            Assert.Equal(0UL, result.TotalBytesRepalloc);
            Assert.Equal(0UL, result.TotalBytesRepallocFree);
            Assert.Equal(178UL, result.TotalBytesPfree);
            Assert.Empty(result.StdOut);
            Assert.Empty(result.StdErr);
            Assert.Empty(result.ElogMessages);
            Debug.WriteLine($"Finished test {nameof(CompileInt32InArgsWithoutNameSuccess)}.");
        }
    }
}