namespace Mono.test_235
{
    //
    // Compilation test: bug #47234
    //
    public class T {
    
        static void Foo (T t, T tt)
        {
        }
    
        static void Foo (params object[] theParams)
        {
        }
    
        [Uno.Testing.Test] public static void test_235() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo (new T (), null);
                    return 0;
        }
    }
}
