namespace Mono.gtest_013
{
    // Compiler options: -r:gtest-013-lib.dll
    
    public class X
    {
        Stack<int> stack;
    
        void Test ()
        {
            stack.Hello (3);
        }
    
        [Uno.Testing.Test] public static void gtest_013() { Main(); }
        public static void Main()
        { }
    }
}
