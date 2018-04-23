namespace Mono.test_803
{
    using Uno;
    
    class A
    {
        [Uno.Testing.Test] public static void test_803() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int a = 1;
            while (a < 2) {
                try {}
                finally {
                    a++;
                }
            }
            
            return 0;
        }
    }
}
