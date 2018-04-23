namespace Mono.test_462
{
    class X {
        bool ok = false;
        
        void Method (X x)
        {
        }
    
        void Method (string x)
        {
            ok = true;
        }
    
        [Uno.Testing.Test] public static void test_462() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            X x = new X ();
    
            x.Method ((string) null);
            if (x.ok)
                return 0;
            return 1;
        }
    }
}
