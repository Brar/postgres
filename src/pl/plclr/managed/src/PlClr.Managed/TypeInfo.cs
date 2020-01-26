using System.Collections.Generic;
using System.Collections.Immutable;

namespace PlClr
{
    public class TypeInfo
    {
        public TypeInfo(uint oid, string name, string ns, short length, bool isPassedByValue)
        {
            Oid = oid;
            Name = name;
            Namespace = ns;
            Length = length;
            IsPassedByValue = isPassedByValue;
        }

        public uint Oid { get; }
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
        public CompositeTypeInfo(uint oid, string name, string ns, short length, bool isPassedByValue, IEnumerable<AttributeInfo> attributes) : base(oid, name, ns, length, isPassedByValue)
        {
            Attributes = attributes.ToImmutableArray();
        }

        public ImmutableArray<AttributeInfo> Attributes { get; }
    }
}