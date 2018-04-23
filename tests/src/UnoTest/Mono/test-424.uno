namespace Mono.test_424
{
    using Uno;
    
    class C
    {
        [Uno.Testing.Test] public static void test_424() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            const string s = "oups";
            if (s.Length != 4) {
                Console.WriteLine (s.Length);
                return 2;
            }
            
            return 0;
        }
    }
}
