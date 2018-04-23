namespace Mono.test_126
{
    //
    // It is possible to invoke object methods in an interface.
    //
    using Uno;
    
    interface Iface {
        void Method ();
    }
    
    class X : Iface {
    
        void Iface.Method () {} 
        
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_126() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            X x = new X ();
            Iface f = x;
    
            if (f.ToString () != "X")
                return 1;
    
            return 0;
        }
    }
}
