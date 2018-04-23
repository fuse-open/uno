namespace Mono.test_static_using_06
{
    // Compiler options: -langversion:6
    
    using Uno;
    using static A.B.X;
    using static A.C.X;
    
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
            public static int Test<T> (T o)
            {
                if (typeof (T) != typeof (object))
                    return -1;
    
                return 2;
            }
        }
    }
    
    namespace C
    {
        class M
        {
            [Uno.Testing.Test] public static void test_static_using_06() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                if (Test<object> ("") != 2)
                    return 1;
    
                return 0;
            }
        }
    }
}
