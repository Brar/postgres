using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class CompileResult : SetupResult
    {
        public CompileResult(IEnumerable<(SeverityLevel, string?)> elogMessages, ulong totalBytesPalloc, ulong totalBytesPalloc0, ulong totalBytesRepalloc, ulong totalBytesRepallocFree, ulong totalBytesPfree, string stdOut, string stdErr) : base(elogMessages, totalBytesPalloc, totalBytesPalloc0, totalBytesRepalloc, totalBytesRepallocFree, totalBytesPfree, stdOut, stdErr)
        {
        }
    }
}
