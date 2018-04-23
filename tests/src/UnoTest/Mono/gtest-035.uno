namespace Mono.gtest_035
{
    // May use a constructed type as constraint.
    
    class Test<T>
    { }
    
    class Foo<T>
        where T : Test<T>
    { }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_035() { Main(); }
        public static void Main()
        {
        }
    }
}
