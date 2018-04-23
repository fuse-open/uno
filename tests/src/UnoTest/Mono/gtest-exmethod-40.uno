namespace Mono.gtest_exmethod_40
{
    // Compiler options: -r:gtest-exmethod-40-lib.dll
    
    namespace N.Extensions
    {
        public static class s
        {
            public static void ShouldEqual (this string text, string name, string value, string domain, string path)
            {
            }
        }
    }
    
    
    namespace N.Main
    {
        using N.Extensions;
    
        public class C
        {
            [Uno.Testing.Test] public static void gtest_exmethod_40() { Main(); }
        public static void Main()
            {
                string v = "";
                v.ShouldEqual ("");
            }
        }
    }
}
