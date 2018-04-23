namespace Mono.test_static_using_01
{
    // Compiler options: -langversion:6
    
    using static A.B.X;
    
    namespace A.B
    {
        static class X
        {
            public static int Test ()
            {
                return 5;
            }
        }
    }
    
    namespace C
    {
        class M
        {
            [Uno.Testing.Test] public static void test_static_using_01() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                if (Test () != 5)
                    return 1;
    
                return 0;
            }
        }
    }
}
