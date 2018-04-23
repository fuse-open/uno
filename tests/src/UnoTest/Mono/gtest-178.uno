namespace Mono.gtest_178
{
    public interface Foo
    {
        T Test<T> ()
            where T : class;
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_178() { Main(); }
        public static void Main()
        { }
    }
}
