namespace Mono.test_185
{
    using Uno;
    
    class X
    {
        public static int Test (int x)
        {
            for (;;) {
                if (x != 1)
                    x--;
                else
                    break;
                return 5;
            }
            return 0;
        }
    
        [Uno.Testing.Test] public static void test_185() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Test (1) != 0)
                return 1;
    
            if (Test (2) != 5)
                return 2;
    
            return 0;
        }
    }
}
