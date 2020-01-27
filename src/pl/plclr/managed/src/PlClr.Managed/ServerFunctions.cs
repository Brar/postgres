using System;
using System.Runtime.InteropServices;

namespace PlClr
{
    public delegate IntPtr ReferenceTypeConversionDelegate(IntPtr argPtr);
    public delegate IntPtr GetAttributeByNumDelegate(IntPtr argPtr, short attNo, out bool isNull);

    public static class ServerFunctions
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

        private static ReferenceTypeConversionDelegate _getTextDelegate = null!;
        private static ReferenceTypeConversionDelegate _setTextDelegate = null!;
        private static ReferenceTypeConversionDelegate _deToastDatumDelegate = null!;
        private static GetAttributeByNumDelegate _getAttributeByNumDelegate = null!;
        private static GetTypeInfoDelegate _getTypeInfoDelegate = null!;

        internal static void Initialize(
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

        public static bool DatumGetBool(IntPtr datum) => datum != IntPtr.Zero;
        public static IntPtr BoolGetDatum(bool value) => value ? One : IntPtr.Zero;

        public static byte DatumGetByte(IntPtr datum) => (byte) datum;
        public static IntPtr ByteGetDatum(byte value) => (IntPtr)value;

        public static short DatumGetInt16(IntPtr datum) => (short) datum;
        public static IntPtr Int16GetDatum(short value) => (IntPtr)value;

        public static int DatumGetInt32(IntPtr datum) => (int) datum;
        public static IntPtr Int32GetDatum(int value) => (IntPtr)value;

        public static long DatumGetInt64(IntPtr datum) => (long) datum;
        public static IntPtr Int64GetDatum(long value) => (IntPtr)value;

        public static uint DatumGetUInt32(IntPtr datum) => (uint) datum;
        public static IntPtr UInt32GetDatum(uint value) => (IntPtr)value;

        public static float DatumGetSingle(IntPtr datum) => (float) datum;
        public static IntPtr SingleGetDatum(float value) => (IntPtr)value;

        public static double DatumGetDouble(IntPtr datum) => (double) datum;
        public static IntPtr DoubleGetDatum(double value) => (IntPtr)value;

        public static string DatumGetText(IntPtr datum) => Marshal.ToStringPFree(_getTextDelegate!(datum))!;
        public static IntPtr TextGetDatum(string value) => _setTextDelegate!(Marshal.ToPtrPalloc(value));

        public static IntPtr DeToastDatum(IntPtr datum) => _deToastDatumDelegate!(datum);

        public static IntPtr GetAttributeByNum(IntPtr heapTupleHeader, short attNo, out bool isNull) => _getAttributeByNumDelegate!(heapTupleHeader, attNo, out isNull);

        public static TypeInfo GetTypeInfo(uint oid)
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
                            ServerTypes.GetTypeInfo(attributeInfo.atttypid),
                            attributeInfo.attnotnull
                        );
                    }

                    return retVal;
                }
            }
        }
    }
}
