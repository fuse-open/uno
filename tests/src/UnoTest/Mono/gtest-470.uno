namespace Mono.gtest_470
{
    // Compiler options: -r:gtest-470-lib.dll
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_470() { Main(); }
        public static void Main()
        {
            var x = new B ();
            x.Foo<C> ();
        }
    }
}
