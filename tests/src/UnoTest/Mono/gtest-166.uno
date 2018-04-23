namespace Mono.gtest_166
{
    // Compiler options: -r:gtest-166-lib.dll
    
    using Uno;
    
    public class Foo
    {
        [Uno.Testing.Test] public static void gtest_166() { Main(); }
        public static void Main() 
        {
            Comparison<TestClass.A<TestClass.Nested>> b = TestClass.B.Compare;
        }
    }
}
