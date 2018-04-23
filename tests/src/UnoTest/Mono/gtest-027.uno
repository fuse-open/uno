namespace Mono.gtest_027
{
    // A generic type declaration may have a non-generic base type.
    
    class TheBase
    {
        public void BaseFunc ()
        { }
    }
    
    class Stack<S> : TheBase
    {
        public void Hello (S s)
        { }        
    }
    
    class Test<T> : Stack<T>
    {
        public void Foo (T t)
        { }
    }
    
    class X
    {
        Test<int> test;
    
        void Test ()
        {
            test.Foo (4);
            test.Hello (3);
            test.BaseFunc ();
        }
    
        [Uno.Testing.Test] public static void gtest_027() { Main(); }
        public static void Main()
        { }
    }
}
