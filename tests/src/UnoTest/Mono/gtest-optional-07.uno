namespace Mono.gtest_optional_07
{
    public class Tests
    {
        public static void foo (Foo f = Foo.None)
        {
        }
    
        [Uno.Testing.Test] public static void gtest_optional_07() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            foo ();
            return 0;
        }
    }
    
    public enum Foo
    {
        None = 0
    }
}
