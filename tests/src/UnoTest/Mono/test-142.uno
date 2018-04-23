namespace Mono.test_142
{
    using Uno;
    
    public class TestClass : TestBaseClass {
    
        public TestClass (EventHandler hndlr) : base ()
        {
            Blah += hndlr;
        }
    
        [Uno.Testing.Test] public static void test_142() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
    
    public class TestBaseClass {
    
        public event EventHandler Blah;
    
    }
}
