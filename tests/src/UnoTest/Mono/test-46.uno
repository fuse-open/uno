namespace Mono.test_46
{
    //
    // This test probes the various explicit unboxing casts
    //
    using Uno;
    
    class X {
        static int cast_int (object o) { return (int) o; }
        static uint cast_uint (object o) { return (uint) o; }
        static short cast_short (object o) { return (short) o; }
        static char cast_char (object o) { return (char) o; }
        static ushort cast_ushort (object o) { return (ushort) o; }
        static byte cast_byte (object o) { return (byte) o; }
        static sbyte cast_sbyte (object o) { return (sbyte) o; }
        static long cast_long (object o) { return (long) o; }
        static ulong cast_ulong (object o) { return (ulong) o; }
        static float cast_float (object o) { return (float) o; }
        static double cast_double (object o) { return (double) o; }
        static bool cast_bool (object o) { return (bool) o; }
    
        [Uno.Testing.Test] public static void test_46() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (cast_int ((object) -1) != -1)
                return 1;
            if (cast_int ((object) 1) != 1)
                return 2;
            if (cast_int ((object) Int.MaxValue) != Int.MaxValue)
                return 1;
            if (cast_int ((object) Int.MinValue) != Int.MinValue)
                return 2;
            if (cast_int ((object) 0) != 0)
                return 3;
    
            if (cast_uint ((object) (uint)0) != 0)
                return 4;
            if (cast_uint ((object) (uint) 1) != 1)
                return 5;
            if (cast_uint ((object) (uint) UInt.MaxValue) != UInt.MaxValue)
                return 6;
            if (cast_uint ((object) (uint) UInt.MinValue) != UInt.MinValue)
                return 7;
    
            if (cast_ushort ((object) (ushort) 1) != 1)
                return 8;
            if (cast_ushort ((object) (ushort) UShort.MaxValue) != UShort.MaxValue)
                return 9;
            if (cast_ushort ((object) (ushort) UShort.MinValue) != UShort.MinValue)
                return 10;
            if (cast_ushort ((object) (ushort) 0) != 0)
                return 11;
    
            if (cast_short ((object) (short)-1) != -1)
                return 12;
            if (cast_short ((object) (short) 1) != 1)
                return 13;
            if (cast_short ((object) (short) Short.MaxValue) != Short.MaxValue)
                return 14;
            if (cast_short ((object) (short) Short.MinValue) != Short.MinValue)
                return 15;
            if (cast_short ((object) (short) 0) != 0)
                return 16;
    
            if (cast_byte ((object) (byte)1) != 1)
                return 17;
            if (cast_byte ((object) (byte) Byte.MaxValue) != Byte.MaxValue)
                return 18;
            if (cast_byte ((object) (byte) Byte.MinValue) != Byte.MinValue)
                return 19;
            if (cast_byte ((object) (byte) 0) != 0)
                return 20;
    
            if (cast_sbyte ((object) (sbyte) -1) != -1)
                return 21;
            if (cast_sbyte ((object) (sbyte) 1) != 1)
                return 22;
            if (cast_sbyte ((object) (sbyte) SByte.MaxValue) != SByte.MaxValue)
                return 23;
            if (cast_sbyte ((object) (sbyte)SByte.MinValue) != SByte.MinValue)
                return 24;
            if (cast_sbyte ((object) (sbyte) 0) != 0)
                return 25;
            
    
            if (cast_long ((object) (long) -1) != -1)
                return 26;
            if (cast_long ((object) (long) 1) != 1)
                return 27;
            if (cast_long ((object) (long) Long.MaxValue) != Long.MaxValue)
                return 28;
            if (cast_long ((object) (long) Long.MinValue) != Long.MinValue)
                return 29;
            if (cast_long ((object) (long) 0) != 0)
                return 30;
    
            if (cast_ulong ((object) (ulong) 0) != 0)
                return 31;
            if (cast_ulong ((object) (ulong) 1) != 1)
                return 32;
            if (cast_ulong ((object) (ulong) ULong.MaxValue) != ULong.MaxValue)
                return 33;
            if (cast_ulong ((object) (ulong) ULong.MinValue) != ULong.MinValue)
                return 34;
    
            if (cast_double ((object) (double) -1) != -1)
                return 35;
            if (cast_double ((object) (double) 1) != 1)
                return 36;
            if (cast_double ((object) (double) Double.MaxValue) != Double.MaxValue)
                return 37;
            if (cast_double ((object) (double) Double.MinValue) != Double.MinValue)
                return 38;
            if (cast_double ((object) (double) 0) != 0)
                return 39;
    
            if (cast_float ((object) (float) -1) != -1)
                return 40;
            if (cast_float ((object) (float) 1) != 1)
                return 41;
            if (cast_float ((object) (float)Float.MaxValue) != Float.MaxValue)
                return 42;
            if (cast_float ((object) (float) Float.MinValue) != Float.MinValue)
                return 43;
            if (cast_float ((object) (float) 0) != 0)
                return 44;
    
            return 0;
        }
    }
}
