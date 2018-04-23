namespace Mono.test_1
{
    using Uno;
    
    class X {
        [Uno.Testing.Test] public static void test_1() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args)
        {
            Console.WriteLine ("Hello, World!");
            return 0;
        }
    }
}
