namespace Mono.test_633
{
    namespace A.X
    {
    }
    
    namespace B.X
    {
    }
    
    namespace Test
    {
        using A.X;
        using B.X;
        
        class C
        {
            [Uno.Testing.Test] public static void test_633() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
