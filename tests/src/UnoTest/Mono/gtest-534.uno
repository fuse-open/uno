namespace Mono.gtest_534
{
    // Compiler options: -r:gtest-534-lib.dll
    
    class A : IA
    {
        public void Method (IG<double[][]> arg)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_534() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new A ().Method (null);
            return 0;
        }
    }
}
