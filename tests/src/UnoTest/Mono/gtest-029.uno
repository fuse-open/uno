namespace Mono.gtest_029
{
    class Stack<T>
    {
        T[] t;
    
        public Stack (int n)
        {
            t = new T [n];
        }
    
        public object Test ()
        {
            // Boxing the type parameter to an object; note that we're
            // an array !
            return t;
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_029() { Main(); }
        public static void Main()
        {
            Stack<int> stack = new Stack<int> (5);
            Console.WriteLine (stack.Test ());
        }
    }
}
