namespace Mono.test_88
{
    class X {
    
    static void f (string s)
    {
    s. Split ('a');
    }
    
        [Uno.Testing.Test] public static void test_88() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            string s = "";
            
            s.Split ('a');
            s.Split ();
            s.Split ('a', 'b', 'c');
            return 0;
        }
    }
}
