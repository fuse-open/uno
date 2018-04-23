namespace Mono.test_738
{
    // Compiler options: -r:test-738-lib.dll
    
    using Uno;
    
    namespace TestNamespace
    {
        public class ResumableInputStream
        {
            public ResumableInputStream()
            {
                stream.Dispose();
            }
    
            private NonClosingStream stream;
            
            [Uno.Testing.Test] public static void test_738() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
