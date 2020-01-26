using System;
using System.Runtime.InteropServices;
using static PlClr.Globals;

namespace PlClr
{
    public delegate IntPtr ReferenceTypeConversionDelegate(IntPtr argPtr);
    public delegate IntPtr GetAttributeByNumDelegate(IntPtr argPtr, short attNo, out bool isNull);

    public class ServerFunctions
    {
        #region private structs for marshalling

        [StructLayout(LayoutKind.Sequential)]
        private struct AttributeInfoPrivate
        {
            public IntPtr attname;
            public uint atttypid;
            public short attnum;
            [MarshalAs(UnmanagedType.U1)]
            public bool attnotnull; // bool
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ClassInfoPrivate
        {
            public IntPtr relattributes;
            public uint oid;
            public short relnatts;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeInfoPrivate
        {
            public IntPtr typname;
            public IntPtr nspname;
            public IntPtr typclass;
            public uint oid;
            public uint typarray;
            public uint typelem;
            public uint typbasetype;
            public int typndims;
            public short typlen;
            public byte typtype;
            [MarshalAs(UnmanagedType.U1)]
            public bool typnotnull; // bool
            [MarshalAs(UnmanagedType.U1)]
            public bool typbyval; // bool
        }

        #endregion

        private readonly ReferenceTypeConversionDelegate _getTextDelegate;
        private readonly ReferenceTypeConversionDelegate _setTextDelegate;
        private readonly ReferenceTypeConversionDelegate _deToastDatumDelegate;
        private readonly GetAttributeByNumDelegate _getAttributeByNumDelegate;
        private readonly GetTypeInfoDelegate _getTypeInfoDelegate;

        internal ServerFunctions(
            ReferenceTypeConversionDelegate getTextDelegate,
            ReferenceTypeConversionDelegate setTextDelegate,
            ReferenceTypeConversionDelegate deToastDatumDelegate,
            GetAttributeByNumDelegate getAttributeByNumDelegate,
            GetTypeInfoDelegate getTypeInfoDelegate
        )
        {
            _getTextDelegate = getTextDelegate;
            _setTextDelegate = setTextDelegate;
            _deToastDatumDelegate = deToastDatumDelegate;
            _getAttributeByNumDelegate = getAttributeByNumDelegate;
            _getTypeInfoDelegate = getTypeInfoDelegate;
        }

        private static readonly IntPtr One = new IntPtr(1);

        public bool GetBool(IntPtr datum) => datum != IntPtr.Zero;
        public IntPtr GetDatum(bool value) => value ? One : IntPtr.Zero;

        public byte GetByte(IntPtr datum) => (byte) datum;
        public IntPtr GetDatum(byte value) => (IntPtr)value;

        public short GetInt16(IntPtr datum) => (short) datum;
        public IntPtr GetDatum(short value) => (IntPtr)value;

        public int GetInt32(IntPtr datum) => (int) datum;
        public IntPtr GetDatum(int value) => (IntPtr)value;

        public long GetInt64(IntPtr datum) => (long) datum;
        public IntPtr GetDatum(long value) => (IntPtr)value;

        public string GetText(IntPtr datum) => Marshal.ToStringPFree(_getTextDelegate!(datum))!;
        public IntPtr TextGetDatum(string value) => _setTextDelegate!(Marshal.ToPtrPalloc(value));

        public IntPtr DeToastDatum(IntPtr datum) => _deToastDatumDelegate!(datum);

        public IntPtr GetAttributeByNum(IntPtr heapTupleHeader, short attNo, out bool isNull) => _getAttributeByNumDelegate!(heapTupleHeader, attNo, out isNull);

        public TypeInfo GetTypeInfo(uint oid)
        {
            var tiPtr = _getTypeInfoDelegate!(oid);
            var typeInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<TypeInfoPrivate>(tiPtr);

            var typeName = Marshal.ToStringPFree(typeInfo.typname)!;
            var namespaceName = Marshal.ToStringPFree(typeInfo.nspname)!;
            switch (typeInfo.typtype)
            {
                case (byte)'b':
                case (byte)'p':
                    return new TypeInfo(typeInfo.oid, typeName, namespaceName, typeInfo.typlen, typeInfo.typbyval);
                case (byte)'c':
                    return GetCompositeTypeInfo(in typeInfo, typeName, namespaceName);
                case (byte)'d':
                    throw ServerLog.EReport(SeverityLevel.Error, "Domain types are currently not supported by PL/CLR.",
                        errorDataType: oid)!;
                case (byte)'e':
                    throw ServerLog.EReport(SeverityLevel.Error, "Enum types are currently not supported by PL/CLR.",
                        errorDataType: oid)!;
                default:
                    throw ServerLog.EReport(SeverityLevel.Error, $"Unknown kind of type {typeInfo.typtype} in type {typeInfo.typname}.",
                        errorDataType: oid)!;
            }

            static CompositeTypeInfo GetCompositeTypeInfo(in TypeInfoPrivate typeInfo, string typeName, string namespaceName)
            {
                var classInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<ClassInfoPrivate>(typeInfo.typclass);

                return new CompositeTypeInfo(typeInfo.oid, typeName, namespaceName, typeInfo.typlen, typeInfo.typbyval, GetAttributeInfos(classInfo));

                static unsafe AttributeInfo[] GetAttributeInfos(ClassInfoPrivate classInfoPrivate)
                {
                    var attributeInfos = new AttributeInfoPrivate[classInfoPrivate.relnatts];
                    new ReadOnlySpan<AttributeInfoPrivate>((void*) classInfoPrivate.relattributes, classInfoPrivate.relnatts).CopyTo(
                        attributeInfos);

                    var retVal = new AttributeInfo[classInfoPrivate.relnatts];
                    for (var i = 0; i < classInfoPrivate.relnatts; i++)
                    {
                        var attributeInfo = attributeInfos[i];
                        var attributeName = Marshal.ToStringPFree(attributeInfo.attname)!;
                        retVal[i] = new AttributeInfo(
                            attributeName,
                            attributeInfo.attnum,
                            BackendFunctions.GetTypeInfo(attributeInfo.atttypid),
                            attributeInfo.attnotnull
                        );
                    }

                    return retVal;
                }
            }
        }
    }
}
