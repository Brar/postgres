using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class SetupResult
    {
        public SetupResult(IEnumerable<(SeverityLevel, string?)> elogMessages, ulong totalBytesPalloc, ulong totalBytesPalloc0, ulong totalBytesRepalloc, ulong totalBytesRepallocFree, ulong totalBytesPfree, string stdOut, string stdErr)
        {
            TotalBytesPalloc = totalBytesPalloc;
            TotalBytesPalloc0 = totalBytesPalloc0;
            TotalBytesRepalloc = totalBytesRepalloc;
            TotalBytesRepallocFree = totalBytesRepallocFree;
            TotalBytesPfree = totalBytesPfree;
            StdOut = stdOut;
            StdErr = stdErr;
            ElogMessages = elogMessages.ToImmutableArray();
        }

        public ImmutableArray<(SeverityLevel, string?)> ElogMessages { get; }
        public ulong TotalBytesPalloc { get; }
        public ulong TotalBytesPalloc0 { get; }
        public ulong TotalBytesRepalloc { get; }
        public ulong TotalBytesRepallocFree { get; }
        public ulong TotalBytesPfree { get; }
        public string StdOut { get; }
        public string StdErr { get; }

    }
}
