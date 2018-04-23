namespace Mono.gtest_059
{
    namespace N {
        interface A <T> {
        }
    }
    class X <T> : N.A <T> {
    }
    class Foo {
        [Uno.Testing.Test] public static void gtest_059() { Main(); }
        public static void Main() {}
    }
}
