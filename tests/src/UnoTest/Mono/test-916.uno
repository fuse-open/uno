namespace Mono.test_916
{
    public class Foo
    {
        public static class Nested
        {
            static int bar ()
            {
                return 0;
            }
            
            [Uno.Testing.Test] public static void test_916() { Main(); }
        public static void Main()
            {
                var i = bar ();
                {
                    bool bar = false;
                }
            } 
        }
    }
}
