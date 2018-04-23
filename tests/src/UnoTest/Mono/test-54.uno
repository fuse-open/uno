namespace Mono.test_54
{
    //
    // This test does not pass peverify because we dont return properly
    // from catch blocks
    //
    using Uno;
    
    class X {
    
        bool v ()
        {
            try {
                throw new Exception ();
            } catch {
                return false;
            }
            return true;
        }
    
        [Uno.Testing.Test] public static void test_54() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }        
    }
}
