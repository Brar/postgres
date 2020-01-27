using System;
using System.Collections.Generic;

namespace PlClr
{
    public static partial class ServerTypes
    {
        private static readonly Dictionary<uint, string> ValueAccessMethodCache = new Dictionary<uint, string>();
        private static readonly Dictionary<uint, Type> TypeCache = new Dictionary<uint, Type>();
        private static readonly Dictionary<uint, TypeAccessInfo> TypeAccessInfoCache = new Dictionary<uint, TypeAccessInfo>();

        internal static TypeInfo GetTypeInfo(uint oid)
            => oid switch
            {
                _ => LookupOrCreateTypeInfo(oid)
            };

        private static TypeInfo LookupOrCreateTypeInfo(uint oid)
        {
            if (TypeInfoCache.TryGetValue(oid, out var value))
                return value;

            var typeInfo = ServerFunctions.GetTypeInfo(oid);
            TypeInfoCache[oid] = typeInfo;
            return typeInfo;
        }

        internal static TypeAccessInfo GeTypeAccessInfo(uint oid)
            => oid switch
            {
                16 => BoolTypeAccessInfo,
                20 => Int64TypeAccessInfo,
                21 => Int16TypeAccessInfo,
                23 => Int32TypeAccessInfo,
                25 => TextTypeAccessInfo,
                26 => OidTypeAccessInfo,
                700 => FloatTypeAccessInfo,
                701 => DoubleTypeAccessInfo,
                2278 => VoidTypeAccessInfo,
                _ => LookupOrCreateTypeAccessInfo(oid)
            };

        private static TypeAccessInfo LookupOrCreateTypeAccessInfo(uint oid)
        {
            if (TypeAccessInfoCache.TryGetValue(oid, out var value))
                return value;

            var typeInfo = GetTypeInfo(oid);
            var typeAccessInfo = CSharpCompiler.CreateTypeAccessInfo(typeInfo);
            TypeAccessInfoCache[oid] = typeAccessInfo;
            return typeAccessInfo;
        }
    }
}
