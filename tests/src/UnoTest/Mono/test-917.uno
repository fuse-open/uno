namespace Mono.test_917
{
    public class Foo
    {
        public static class Nested
        {
            class bar
            {
                public static int value;
            }
            
            [Uno.Testing.Test] public static void test_917() { Main(); }
        public static void Main()
            {
                {
                    bool bar = false;
                }
                
                var i = bar.value;
            } 
        }
    }
}
