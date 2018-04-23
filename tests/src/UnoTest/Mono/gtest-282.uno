namespace Mono.gtest_282
{
    class Foo : C<Foo.Bar> {
        public class Bar {}
    }
    class C<T> {}
    
    class Test {
        static Foo f = new Foo ();
        [Uno.Testing.Test] public static void gtest_282() { Main(); }
        public static void Main() { Console.WriteLine (f.GetType ().BaseType); }
    }
}
