namespace Mono.test_249
{
    struct Foo
    {
        Bar a;
        Bar b;
    }
    
    struct Bar
    {
        public readonly int Test;
    }
    
    class X
    {
        [Uno.Testing.Test] public static void test_249() { Main(); }
        public static void Main()
        { }
    }
}
