namespace Mono.test_466
{
    // Compiler options: -r:test-466-lib.dll
    
    namespace A.X
    {
        using A.B;
        
        class Test
        {
            [Uno.Testing.Test] public static void test_466() { Main(); }
        public static void Main()
            {
                C c = new C ();
                c.Foo ();
            }
        }
    }
}
