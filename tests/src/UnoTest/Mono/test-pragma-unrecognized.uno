namespace Mono.test_pragma_unrecognized
{
    // Compiler options: -warn:4
    // This test should print only: warning CS1633: Unrecognized #pragma directive
    
    #pragma xxx some unrecognized text
    
    public class C
    {
      [Uno.Testing.Test] public static void test_pragma_unrecognized() { Main(); }
        public static void Main() {}
    }
}
