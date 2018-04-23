namespace Mono.test_partial_32
{
    namespace A
    {
        using X;
    
        partial class C
        {
            private class N : N1
            {
            }
    
            [Uno.Testing.Test] public static void test_partial_32() { Main(); }
        public static void Main()
            {            
            }
        }
    }
    
    namespace A
    {
        using X;
    
        partial class C : C1
        {
        }
    }
    
    
    namespace X
    {
        public class C1
        {
            public class N1
            {
    
            }
        }
    }
}
