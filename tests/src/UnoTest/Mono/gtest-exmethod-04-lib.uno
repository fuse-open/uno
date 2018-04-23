namespace Mono.gtest_exmethod_04
{
    // Compiler options: -t:library
    
    using Uno;
    
    namespace A
    {
        public static class A
        {
            public static string Test_1 (this string s)
            {
                return ":";
            }
        }
    }
}
