namespace Mono.test_debug_08
{
    // Compiler options: -optimize
    
    class C
    {
        [Uno.Testing.Test] public static void test_debug_08() { Main(); }
        public static void Main()
        {
            return;
        }
        
        void Foo ()
        {
        }
        
        int Foo2 ()
        {
            return 7;
        }
        
        int Foo3 ()
        {
            {
                {
                    return 2;
                }
            }
        }
    }
}
