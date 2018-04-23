namespace Mono.gtest_553
{
    // Compiler options: -r:gtest-553-lib.dll
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_553() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new A.C<int> ();
            new B.C<byte> ();
            return 0;
        }
    }
}
