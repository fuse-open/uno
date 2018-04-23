namespace Mono.gtest_exmethod_04
{
    // Compiler options: -r:gtest-exmethod-04-lib.dll
    
    namespace A
    {
        public static class Test
        {
            public static string Test_1 (this bool t)
            {
                return ":";
            }
        }
    }
    
    namespace B
    {
        using A;
        
        public class M
        {
            [Uno.Testing.Test] public static void gtest_exmethod_04() { Main(); }
        public static void Main()
            {
                "".Test_1();
            }
        }
    }
}
