namespace Mono.test_98
{
    class X {
        int a;
        Y x;
        
        void b ()
        {
            if (x.a == 1)
                return;
        }
    }
    
    class Y : X {
    
        [Uno.Testing.Test] public static void test_98() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
