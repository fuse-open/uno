namespace Mono.gtest_260
{
    class A<T> where T : class {}
    class B<T> : A<T> where T : class {}
    class Test {
        internal static A<Test> x = new B<Test> ();
        [Uno.Testing.Test] public static void gtest_260() { Main(); }
        public static void Main() { }
    }
}
