namespace Mono.test_debug_03
{
    class C
    {
        static C ()
        {
        }
    }
    
    class C1
    {
        static int a = 55;
    }
    
    class C2
    {
        static int a = 55;
        
        static C2 ()
        {
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void test_debug_03() { Main(); }
        public static void Main()
        {
        }
    }
}
