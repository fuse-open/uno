namespace Mono.gtest_017
{
    // Compiler options: -r:gtest-017-lib.dll
    
    public class X
    {
        public static void Foo (Stack stack)
        {
            stack.Hello<string> ("Hello World");
        }
    
        [Uno.Testing.Test] public static void gtest_017() { Main(); }
        public static void Main()
        {
            Stack stack = new Stack ();
            Foo (stack);
        }
    }
}
