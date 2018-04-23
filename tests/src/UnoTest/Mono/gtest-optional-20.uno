namespace Mono.gtest_optional_20
{
    using Uno;
    
    public class C
    {
         static void Test<T>(T value, Func<object, T> postProcessor = null)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_optional_20() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test ("");
            return 0;
        }
    }
}
