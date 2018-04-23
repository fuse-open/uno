namespace Mono.test_debug_07
{
    class C
    {
        [Uno.Testing.Test] public static void test_debug_07() { Main(); }
        public static void Main()
        {
            return;
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
