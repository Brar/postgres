using System;
using System.IO;
using NUnit.Framework;

namespace PlClr.Managed.Tests
{
    public class PlClrMainTests : TestBase
    {
        [Test]
        public void SetupTest()
        {
            var result = Setup(PlClrMain.Setup);

            Assert.That(
                result.TotalBytesPalloc + result.TotalBytesPalloc0 + result.TotalBytesRepalloc -
                result.TotalBytesPfree - result.TotalBytesRepallocFree, Is.Zero);
            Assert.That(result.StdOut, Is.Empty);
            Assert.That(result.StdErr, Is.Empty);
        }
    }
}