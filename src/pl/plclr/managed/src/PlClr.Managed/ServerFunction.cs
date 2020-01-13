using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static class ServerFunction
    {
        private static readonly IntPtr One = new IntPtr(1);

        public static bool GetBool(IntPtr datum) => datum != IntPtr.Zero;
        public static IntPtr GetDatum(bool value) => value ? One : IntPtr.Zero;

        public static byte GetByte(IntPtr datum) => (byte) datum;
        public static IntPtr GetDatum(byte value) => (IntPtr)value;

        public static short GetInt16(IntPtr datum) => (short) datum;
        public static IntPtr GetDatum(short value) => (IntPtr)value;

        public static int GetInt32(IntPtr datum) => (int) datum;
        public static IntPtr GetDatum(int value) => (IntPtr)value;

        public static long GetInt64(IntPtr datum) => (long) datum;
        public static IntPtr GetDatum(long value) => (IntPtr)value;
    }
}
