namespace Mono.test_911
{
    // Compiler options: -r:test-911-lib.dll
    
    class N
    {
        public static void Foo ()
        {
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void test_911() { Main(); }
        public static void Main()
        {
            N.Foo ();
        }
    }
}
