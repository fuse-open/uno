namespace Mono.test_503
{
    // Compiler options: -warnaserror
    
    class Foo {
        [Uno.Testing.Test] public static void test_503() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            for (;;) {
                try {
                    break;
                } catch {
                    continue;
                }
            }
            return 0;
        }
    }
}
