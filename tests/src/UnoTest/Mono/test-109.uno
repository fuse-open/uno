namespace Mono.test_109
{
    using Uno;
    
    class T {
            [Uno.Testing.Test] public static void test_109() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            //
            // Just a test to compile the following:
            //
                    string a = "Time is: " + DateTime.UtcNow;
    
            return 0;
            }
    }
}
