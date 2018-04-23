namespace Mono.test_95
{
    class X {
    
        double d = 0;
    
        X ()
        {
        }
    
        [Uno.Testing.Test] public static void test_95() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            X x = new X ();
    
            if (x.d != 0)
                return 1;
    
            return 0;
        }
    }
}
