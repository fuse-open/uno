namespace Mono.test_648
{
    using Uno;
    
    namespace ParamMismatch
    {
        public class TestCase
        {
            [Uno.Testing.Test] public static void test_648() { Main(); }
        public static void Main()
            {
            }
            
            public TestCase()
            {
            }
    
            public event EventHandler Culprit
            {
                add
                {
                    // even when this contained something, compiling would fail
                }
    
                remove
                {
                    // even when this contained something, compiling would fail
                }
            }
            ~TestCase()
            {
            }
        }
    }
}
