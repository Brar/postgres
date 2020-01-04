using NUnit.Framework;
using System.Runtime.InteropServices;

namespace PlClr.Managed.Tests
{
    public class PlClrMainTests : TestBase
    {
        [Test]
        public void CompileSimple()
        {
            FunctionCompileInfo(1, "test", "return 1;", (fci, length) =>
            {
                PlClrMain.CompileFunction(fci, length);
            });
        }
    }
}