namespace Mono.test_75
{
    //
    // This test probes using an operator overloaded in a parents' parent
    //
    
    class X {
        public static bool called = false;
        
        static public X operator + (X a, X b)
        {
            called = true;
            return null;
        }
    }
    
    class Y : X {
    }
    
    class Z : Y {
    }
    
    class driver {
    
        [Uno.Testing.Test] public static void test_75() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Z a = new Z ();
            Z b = new Z ();
            X c = a + b;
    
            if (X.called)
                return 0;
    
            return 1;
        }
    
    }
}
