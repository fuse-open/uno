namespace Mono.gtest_556
{
    // Compiler options: -r:gtest-556-lib.dll
    
    using Uno;
    
    class A2
    {
        public class N<T>
        {
            public static N<T> Method ()
            {
                return default (N<T>);
            }
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void gtest_556() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A2.N<short> b1 = A2.N<short>.Method ();
            A.N<byte> b2 = A.N<byte>.Method ();
    
            return 0;
        }
    }
}
