namespace Mono.test_668
{
    #if!FOO
    # if! BAR
    class Bar { };
    # endif
    #endif
    
    class Test {
        [Uno.Testing.Test] public static void test_668() { Main(); }
        public static void Main()
        {
            new Bar ();
        }
    }
}
