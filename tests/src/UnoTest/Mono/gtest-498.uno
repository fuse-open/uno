namespace Mono.gtest_498
{
    // Compiler options: -r:gtest-498-lib.dll
    
    class A : C<int>
    {
        public A ()
        {
            base.size = 100;
        }
        
        [Uno.Testing.Test] public static void gtest_498() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            ssize = 101;
            return 0;
        }
    }
}
