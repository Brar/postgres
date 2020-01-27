using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static partial class ServerTypes
    {
        // void is the only type where we have to cheat about nullability
        private static readonly TypeAccessInfo VoidTypeAccessInfo = new TypeAccessInfo(typeof(void), null!, null!, null!, null!);
        private static readonly TypeAccessInfo BoolTypeAccessInfo = new TypeAccessInfo(typeof(bool), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetBool), typeof(ServerFunctions), nameof(ServerFunctions.BoolGetDatum));
        private static readonly TypeAccessInfo Int64TypeAccessInfo = new TypeAccessInfo(typeof(long), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetInt64), typeof(ServerFunctions), nameof(ServerFunctions.Int64GetDatum));
        private static readonly TypeAccessInfo Int16TypeAccessInfo = new TypeAccessInfo(typeof(short), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetInt16), typeof(ServerFunctions), nameof(ServerFunctions.Int16GetDatum));
        private static readonly TypeAccessInfo Int32TypeAccessInfo = new TypeAccessInfo(typeof(int), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetInt32), typeof(ServerFunctions), nameof(ServerFunctions.Int32GetDatum));
        private static readonly TypeAccessInfo TextTypeAccessInfo = new TypeAccessInfo(typeof(string), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetText), typeof(ServerFunctions), nameof(ServerFunctions.TextGetDatum));
        private static readonly TypeAccessInfo OidTypeAccessInfo = new TypeAccessInfo(typeof(uint), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetUInt32), typeof(ServerFunctions), nameof(ServerFunctions.UInt32GetDatum));
        private static readonly TypeAccessInfo FloatTypeAccessInfo = new TypeAccessInfo(typeof(float), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetSingle), typeof(ServerFunctions), nameof(ServerFunctions.SingleGetDatum));
        private static readonly TypeAccessInfo DoubleTypeAccessInfo = new TypeAccessInfo(typeof(double), typeof(ServerFunctions), nameof(ServerFunctions.DatumGetDouble), typeof(ServerFunctions), nameof(ServerFunctions.DoubleGetDatum));
    }
}
