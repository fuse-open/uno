namespace Mono.support_test_debug_04
{
    partial class C
    {
    }
    
    partial class C1
    {
        int b = 55;
    }
    
    partial class C2
    {
        int b = 55;
        
        C2 ()
        {
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void support_test_debug_04() { Main(); }
        public static void Main()
        {
        }
    }
}
