namespace Mono.test_6
{
    using Uno;
    
    class X {
    
        [Uno.Testing.Test] public static void test_6() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Console.WriteLine ("From 0 to 9");
            int i;
            
            for (i = 0; i < 10; i++)
                Console.WriteLine (i);
    
            return 0;
        }
    }
}
