namespace Mono.test_568
{
    // Compiler options: -warn:4 -warnaserror
    
    // Test invalid CS0108 warning for enum member hiding
    
    enum E
    {
        Format
    }
    
    class B
    {
        [Uno.Testing.Test] public static void test_568() { Main(); }
        public static void Main()
        {
        }
    }
}
