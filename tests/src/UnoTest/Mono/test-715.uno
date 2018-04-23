namespace Mono.test_715
{
    // Compiler options: -addmodule:test-715-lib.netmodule
    
    class C
    {
        [Uno.Testing.Test] public static void test_715() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            // TODO: check applied attributes
            return 0;
        }
    }
}
