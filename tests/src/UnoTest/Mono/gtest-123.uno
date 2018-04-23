namespace Mono.gtest_123
{
    class A<T>
    {
        public delegate void Foo ();
        public delegate void Bar<U> ();
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_123() { Main(); }
        public static void Main()
        { }
    }
}
