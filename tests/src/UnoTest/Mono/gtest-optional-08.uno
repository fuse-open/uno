namespace Mono.gtest_optional_08
{
    public class Tests
    {
        string s;
        
        private Tests (string arg = "long")
        {
            this.s = arg;
        }
        
        public Tests (int other)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_optional_08() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var v = new Tests ();
            if (v.s != "long")
                return 1;
            
            return 0;
        }
    }
}
