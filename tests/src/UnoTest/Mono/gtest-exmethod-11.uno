namespace Mono.gtest_exmethod_11
{
    using Uno;
    
    public class Test
    {
        [Uno.Testing.Test] public static void gtest_exmethod_11() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args)
        {
            if (1.OneEleven ())
                return 0;
    
            return 1;
        }
    }
    
    public static class Lol
    {
        public static bool OneEleven (this object o)
        {
            return true;
        }
    }
}
