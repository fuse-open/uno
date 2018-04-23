namespace Mono.gtest_015
{
    // Very simple example of a generic method.
    
    class Stack<S>
    {
        public static void Hello<T,U> (S s, T t, U u)
        {
            U v = u;
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_015() { Main(); }
        public static void Main()
        {
        }
    }
}
