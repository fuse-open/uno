namespace Mono.gtest_175
{
    using Uno;
    
    public class Foo
    {
    }
    
    public class X
    {
        public static Foo Test (Foo foo, Foo bar)
        {
            return foo ?? bar;
        }
    
        [Uno.Testing.Test] public static void gtest_175() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo[] array = new Foo [1];
    
            Foo foo = new Foo ();
            Foo bar = Test (array [0], foo);
    
            if (bar == null)
                return 1;
    
            return 0;
        }
    }
}
