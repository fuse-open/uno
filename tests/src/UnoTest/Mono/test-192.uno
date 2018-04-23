namespace Mono.test_192
{
    //
    // Tests that we validate the unchecked state during constatn resolution
    //
    class X {
        [Uno.Testing.Test] public static void test_192() { Main(); }
        public static void Main()
        {
            unchecked {
                const int val = (int)0x800B0109;
            }
        }
    }
}
