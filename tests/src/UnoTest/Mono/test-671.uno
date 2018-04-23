namespace Mono.test_671
{
    using Uno;
    
    class C
    {
        [Uno.Testing.Test] public static void test_671() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return Bar (null) ? 1 : 0;
        }
    
        static bool Bar (object t)
        {
            return t is object;
        }
    }
}
