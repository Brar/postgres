using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateClrConfig
{
    internal sealed class RuntimeIdentifier
    {
        public RuntimeIdentifier(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public RuntimeIdentifier[]? CompatibleIdentifiers { get; set; }

        public override string ToString()
        {
            return $"{Name}: [{(CompatibleIdentifiers == null ? "null" : string.Join(", ", CompatibleIdentifiers.Select(e => e.ToString())))}]";
        }
    }
}
