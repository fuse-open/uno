namespace Mono.test_779
{
    using Uno;
    
    public static class Test
    {
        [Uno.Testing.Test] public static void test_779() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (test1 (15, 15))
                return 1;
    
            return 0;
        }
    
        //Bug #610126
        static bool test1 (long a, long b)
        {
            if ((a & b) == 0L) return true;
            return false;
        }
    }
}
