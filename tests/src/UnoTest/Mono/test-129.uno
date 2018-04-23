namespace Mono.test_129
{
    //
    // Check unary minus.
    //
    using Uno;
    
    class X {
    
        [Uno.Testing.Test] public static void test_129() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            short a = -32768;
            int b = -2147483648;
            long c = -9223372036854775808;
            sbyte d = -128;
            
            object o = -(2147483648);
            if (o.GetType () != typeof (long))
                return 1;
    
            o = -(uint)2147483648;
            Console.WriteLine (o.GetType ());
            if (o.GetType () != typeof (long))
                return 2;
    
            uint ui = (1);
            byte b1 = (int)(0x30);
            byte b2 = (int)0x30;
            
            return 0;
        }
    }
}
