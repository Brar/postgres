using System.Collections.Generic;
using System.Collections.Immutable;

namespace PlClr
{
    public class TypeInfo
    {
        public TypeInfo(uint oid, uint xMin, ushort itemPointerBlockIdHigh, ushort itemPointerBlockIdLow, ushort itemPointerOffsetNumber, string name, string ns, short length, bool isPassedByValue)
        {
            Oid = oid;
            XMin = xMin;
            ItemPointerBlockIdHigh = itemPointerBlockIdHigh;
            ItemPointerBlockIdLow = itemPointerBlockIdLow;
            ItemPointerOffsetNumber = itemPointerOffsetNumber;
            Name = name;
            Namespace = ns;
            Length = length;
            IsPassedByValue = isPassedByValue;
        }

        public uint Oid { get; }
        public uint XMin { get; }
        public ushort ItemPointerBlockIdHigh { get; }
        public ushort ItemPointerBlockIdLow { get; }
        public ushort ItemPointerOffsetNumber { get; }
        public string Name { get; }
        public string Namespace { get; }
        public short Length { get; }
        public bool IsPassedByValue { get; }
    }

    public class AttributeInfo
    {
        public AttributeInfo(string name, short rowNumber, TypeInfo type, bool notNull)
        {
            Name = name;
            Type = type;
            NotNull = notNull;
            RowNumber = rowNumber;
        }

        public string Name { get; }
        public short RowNumber { get; }
        public TypeInfo Type { get; }
        public bool NotNull { get; }
    }

    public class CompositeTypeInfo : TypeInfo
    {
        public CompositeTypeInfo(uint oid, uint xMin, ushort itemPointerBlockIdHigh, ushort itemPointerBlockIdLow, ushort itemPointerOffsetNumber, string name, string ns, short length, bool isPassedByValue, IEnumerable<AttributeInfo> attributes)
            : base(oid, xMin, itemPointerBlockIdHigh, itemPointerBlockIdLow, itemPointerOffsetNumber, name, ns, length, isPassedByValue)
        {
            Attributes = attributes.ToImmutableArray();
        }

        public ImmutableArray<AttributeInfo> Attributes { get; }
    }
}