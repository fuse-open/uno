namespace Mono.gtest_implicitarray_02
{
    class MyString
    {
        public static implicit operator string (MyString s)
        {
            return "ggtt";
        }
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void gtest_implicitarray_02() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var v = new [] { new MyString (), "a" };
            if (v [0] != "ggtt")
                return 1;
            return 0;
        }
    }
}
