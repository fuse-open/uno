namespace Mono.test_396
{
    // Compiler options: -r:test-396-lib.dll
    
    public class MainClass
    {
        [Uno.Testing.Test] public static void test_396() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A a = new A ();
            B b = new B ();
            bool r = (a == b);
    
                    return 0;
        }
    }
}
