namespace Mono.gtest_exmethod_39
{
    using Uno;
    using Extra;
    
    namespace Extra
    {
        static class S
        {
            public static int Prefix (this string s, string prefix)
            {
                return 1;
            }
        }
    }
    
    static class SimpleTest
    {
        public static int Prefix (this string s, string prefix, bool bold)
        {
            return 0;
        }
    }
    
    public class M
    {
        [Uno.Testing.Test] public static void gtest_exmethod_39() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var res = "foo".Prefix ("1");
            if (res != 1)
                return 1;
            
            return 0;
        }
    }
}
