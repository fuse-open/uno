namespace Mono.gtest_031
{
    // Compiler options: -r:gtest-031-lib.dll

    public class X
    {
        public static void Test (Bar<int,string> bar)
        {
            bar.Hello ("Test");
            bar.Test (7, "Hello");
        }

        [Uno.Testing.Test] public static void gtest_031() { Main(); }
        public static void Main()
        { }
    }
}
