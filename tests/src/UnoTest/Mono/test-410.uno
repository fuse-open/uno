namespace Mono.test_410
{
    // Compiler options: -r:test-410-lib.dll
    
    using Uno;
    using Q;
    
    public class B {
        [Uno.Testing.Test] public static void test_410() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            return (A.ToString() == "Hello world!") ? 0 : 1;
        }
    }
}
