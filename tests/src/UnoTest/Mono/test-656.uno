namespace Mono.test_656
{
    // Compiler options: -r:test-656-lib.dll;
    
    // Trailing semicolon is part of the test
    
    class Goo
    {
        [Uno.Testing.Test] public static void test_656() { Main(); }
        public static void Main()
        {
            string s = new Foo () ["foo"];
        }
    }
}
