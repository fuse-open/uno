namespace Mono.gtest_067
{
    class Test
    {
        public delegate int Foo<T> (T t, T u);
    
        public void Hello<U> (Foo<U> foo, U u)
        { }
    }
    
    class X
    {
        static int Add (int a, int b)
        {
            return a + b;
        }
    
        [Uno.Testing.Test] public static void gtest_067() { Main(); }
        public static void Main()
        {
            Test test = new Test ();
            test.Hello<int> (new Test.Foo<int> (Add), 5);
        }
    }
}
