namespace Mono.test_933
{
    using Uno;
    
    class X
    {
        static int Foo (params X[] p)
        {
            return 1;
        }
    
        static int Foo (object p)
        {
            return 0;
        }
    
        [Uno.Testing.Test] public static void test_933() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Foo ((X[]) null) != 1)
                return 1;
    
            return 0;
        }
    }
}
