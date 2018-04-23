namespace Mono.test_620
{
    //
    // fixed
    //
    class X {
    
        static void A (ref int a)
        {
            a++;
        }
    
        // Int&
        static void B (ref int a)
        {
            // Int&&
            A (ref a);
        }
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_620() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int a = 10;
    
            B (ref a);
    
            if (a == 11)
                return 0;
            else
                return 1;
        }
    }
}
