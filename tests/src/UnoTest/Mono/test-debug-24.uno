namespace Mono.test_debug_24
{
    // Tests for special columns handling
    
    class C
    {
        void Test_1 ()
        {
            object a = new object (),  b = new object ();
        }
        
        void Test_2 ()
        {
            for (int i = 0; i <= 10; ++i) { }
        }
        
        [Uno.Testing.Test] public static void test_debug_24() { Main(); }
        public static void Main()
        {
        }
    }
}
