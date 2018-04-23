namespace Mono.gtest_034
{
    class Foo<T>
    { }
    
    class Stack<T>
    { }
    
    //
    // We may use a constructed type `Stack<T>' instead of
    // just a type parameter.
    //
    
    class Bar<T> : Foo<Stack<T>>
    { }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_034() { Main(); }
        public static void Main()
        { }
    }
}
