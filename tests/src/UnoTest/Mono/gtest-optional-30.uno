namespace Mono.gtest_optional_30
{
    // Compiler options: -r:gtest-optional-30-lib.dll
    
    public static class Program
    {
        [Uno.Testing.Test] public static void gtest_optional_30() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Lib.Foo<object>() != null)
                return 1;
    
            return 0;
        }
    }
}
