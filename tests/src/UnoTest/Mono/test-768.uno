namespace Mono.test_768
{
    namespace A.N
    {
        class Wrong
        {
        }
    }
    
    namespace N
    {
        class C
        {
            public static string value;
        }
    }
    
    namespace X
    {
        using A;
        
        public class TestClass
        {
            [Uno.Testing.Test] public static void test_768() { Main(); }
        public static void Main()
            {
                string s = N.C.value;
            }
        }
    }
}
