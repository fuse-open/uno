namespace Mono.gtest_058
{
    class Foo {
        [Uno.Testing.Test] public static void gtest_058() { Main(); }
        public static void Main() {}
    }
    
    class Foo <T> {
        static Foo <T> x;
        static Foo <T> Blah { get { return x; } }
    }
}
