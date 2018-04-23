namespace Mono.gtest_238
{
    // Compiler options: /r:gtest-238-lib.dll
    // Dependencies: gtest-238-lib.cs
    class X
    {
        [Uno.Testing.Test] public static void gtest_238() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo<long> foo = new Foo<long> ();
            if (foo.Test (3) != 1)
                return 1;
            if (foo.Test (5L) != 2)
                return 2;
            return 0;
        }
    }
}
