namespace Mono.test_nameof_01
{
    class X
    {
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_nameof_01() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            const string s = nameof (X);
            Console.WriteLine (s);
            if (s != "X")
                return 1;
    
            return 0;
        }
    }
}
