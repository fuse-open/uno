namespace Mono.test_static_using_02
{
    // Compiler options: -langversion:6
    
    using Uno;
    
    namespace A.B
    {
        static class X
        {
            public static int Test (object o)
            {
                return 1;
            }
        }
    }
    
    namespace A.C
    {
        static class X
        {
            public static int Test (int o)
            {
                return 2;
            }
        }
    }
    
    namespace C
    {
        using static A.B.X;
        using static A.C.X;
    
        class M
        {
            [Uno.Testing.Test] public static void test_static_using_02() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                if (Test (3) != 2)
                    return 1;
    
                return 0;
            }
        }
    }
}
