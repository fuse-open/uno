namespace Mono.gtest_011
{
    class Stack<S>
    {
        public void Hello (S s)
        { }
    }
    
    class X
    {
        Stack<int> stack;
    
        void Test ()
        {
            stack.Hello (3);
        }
    
        [Uno.Testing.Test] public static void gtest_011() { Main(); }
        public static void Main()
        { }
    }
}
