namespace Mono.test_650
{
    // Compiler options: -debug
    
    // Symbol writer test
    
    #line 1 "@@file_does_not_exist@@"
    
    #line 3 "test-650.cs" // self-reference
    
    class App
    {
        [Uno.Testing.Test] public static void test_650() { Main(); }
        public static void Main()
        {
        }
    }
}
