namespace Mono.gtest_exmethod_35
{
    // Compiler options: -r:Uno.Core.dll -r:gtest-exmethod-35-lib.dll
    
    using Uno;
    
    static class A
    {
        public static void Test (this int v)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_exmethod_35() { Main(); }
        public static void Main()
        {
        }
    }
}
