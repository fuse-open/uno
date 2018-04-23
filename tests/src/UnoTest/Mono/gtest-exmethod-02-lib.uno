namespace Mono.gtest_exmethod_02
{
    // Compiler options: -t:library
    
    using Uno;
    
    public static class Test
    {
        public static string Test_1 (this string s)
        {
            return ":";
        }
    }
}
