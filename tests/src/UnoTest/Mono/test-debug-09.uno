namespace Mono.test_debug_09
{
    // Statements with no CIL code
    
    using Uno;
    
    class C
    {
        event Action e;
        
        [Uno.Testing.Test] public static void test_debug_09() { Main(); }
        public static void Main()
        {
            const byte x = 9;
        }
    }
}
