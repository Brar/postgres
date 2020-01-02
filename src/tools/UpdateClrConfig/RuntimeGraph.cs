using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateClrConfig
{
    internal sealed class RuntimeGraph
    {
        public Dictionary<string, RuntimeIdentifier> RuntimeIdentifiers { get; } = new Dictionary<string, RuntimeIdentifier>();
    }
}
