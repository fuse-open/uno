namespace Mono.test_875
{
    // Compiler options: -r:test-875-lib.dll -r:test-875-2-lib.dll
    
    using N;
    
    public class Test: Lib
    {
        [Uno.Testing.Test] public static void test_875() { Main(); }
        public static void Main()
        {
            new Test ();
        }
    }
}
