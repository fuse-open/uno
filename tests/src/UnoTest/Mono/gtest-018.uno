namespace Mono.gtest_018
{
    public class Stack
    {
        public Stack ()
        { }
    
        public void Hello<T> (T t)
        { }
    }
    
    public class X
    {
        public static void Foo (Stack stack)
        {
            stack.Hello<string> ("Hello World");
        }
    
        [Uno.Testing.Test] public static void gtest_018() { Main(); }
        public static void Main()
        {
            Stack stack = new Stack ();
            Foo (stack);
        }
    }
}
