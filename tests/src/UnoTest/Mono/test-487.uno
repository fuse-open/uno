namespace Mono.test_487
{
    using Uno;
    struct X {
        int i;
        static bool pass = false;
        
        X (object dummy)
        {
            X x = new X ();
            x.i = 1;
            int n = 0;
    
            if ((this = x).i == 1)
                n ++;
            
            if (this.i == 1)
                n ++;
            
            pass = (n == 2);
        }
        [Uno.Testing.Test] public static void test_487() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new X (null);
            return pass ? 0 : 1;
        }
    }
}
