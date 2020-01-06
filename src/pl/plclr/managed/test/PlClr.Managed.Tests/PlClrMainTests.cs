using System;
using NUnit.Framework;

namespace PlClr.Managed.Tests
{
    public class PlClrMainTests : TestBase
    {
        [Test]
        public void SetupSuccess()
        {
            var p = new SetupParameters();

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.Not.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(8));
            Assert.That(result.TotalBytesPfree, Is.EqualTo(8));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.Empty);
        }

        [Test]
        public void SetupNullPtr()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(IntPtr.Zero, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Argument arg must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void SetupZeroLength()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, 0);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Argument argLength is 0 but is expected to be greater than or equal to 40{Environment.NewLine}"));
        }

        [Test]
        public void SetupNoPallocPtr()
        {
            var p = new SetupParameters
            {
                PallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Field PallocFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void SetupNoPalloc0Ptr()
        {
            var p = new SetupParameters
            {
                Palloc0FunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Field Palloc0FunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void SetupNoRePallocPtr()
        {
            var p = new SetupParameters
            {
                RePallocFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Field RePallocFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void SetupNoPFreePtr()
        {
            var p = new SetupParameters
            {
                PFreeFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Field PFreeFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void SetupNoELogPtr()
        {
            var p = new SetupParameters
            {
                ELogFunctionPtrIsNull = true
            };

            var result = Setup(p, (ptr, len) =>
            {
                var res = PlClrMain.Setup(ptr, len);
                Assert.That(res, Is.EqualTo(IntPtr.Zero));
                return res;
            });

            Assert.That(result.TotalBytesPalloc, Is.EqualTo(0));
            Assert.That(result.TotalBytesPalloc0, Is.Zero);
            Assert.That(result.TotalBytesRepalloc, Is.Zero);
            Assert.That(result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.TotalBytesPfree, Is.EqualTo(0));
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.EqualTo($"Field ELogFunctionPtr in struct ClrSetupInfo must not be NULL{Environment.NewLine}"));
        }

        [Test]
        public void CompileSimpleSuccess()
        {
            var p = new CompileParameters(123U, "myFunc", "");

            var result = Compile(p, (ptr, len) =>
            {
                var res = PlClrMain.Compile(ptr, len);
                Assert.That(res, Is.Not.EqualTo(IntPtr.Zero));
                return res;
            });
        }
    }
}