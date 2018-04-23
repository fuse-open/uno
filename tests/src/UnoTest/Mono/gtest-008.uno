namespace Mono.gtest_008
{
    interface I
    {
        void Hello ();
    }
    
    class Stack<T>
        where T : I, new ()
    {
    }
    
    class Test
    {
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_008() { Main(); }
        public static void Main()
        {
        }
    }
}
