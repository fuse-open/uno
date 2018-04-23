namespace Mono.gtest_307
{
    partial class Foo<T> {}
    partial class Foo<T> {
        public delegate int F ();
    }
    
    class Bar {
        static int g () { return 0; }
        [Uno.Testing.Test] public static void gtest_307() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo<int>.F f = g;
            return f ();
        }
    }
}
