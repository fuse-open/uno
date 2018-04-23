namespace Mono.gtest_089
{
    using Uno;
    
    class Test<T>
    {
        public void Foo (T t, out int a)
        {
            a = 5;
        }
    
        public void Hello (T t)
        {
            int a;
    
            Foo (t, out a);
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_089() { Main(); }
        public static void Main()
        { }
    }
}
