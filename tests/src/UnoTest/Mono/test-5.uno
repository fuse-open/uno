namespace Mono.test_5
{
    using Uno;
    
    class X {
    
        [Uno.Testing.Test] public static void test_5() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Console.WriteLine ("From 0 to 9");
            
            for (int i = 0; i < 10; i++)
                Console.WriteLine (i);
    
            return 0;
        }
    }
}
