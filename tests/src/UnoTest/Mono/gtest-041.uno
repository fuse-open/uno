namespace Mono.gtest_041
{
    // We may use type parameters as `params' type.
    
    class Stack<T>
    {
        public void Hello (int a, params T[] args)
        { }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_041() { Main(); }
        public static void Main()
        {
            Stack<string> stack = new Stack<string> ();
            stack.Hello (1, "Hello", "World");
        }
    }
}
