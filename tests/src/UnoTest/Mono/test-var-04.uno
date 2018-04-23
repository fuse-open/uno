namespace Mono.test_var_04
{
    namespace Test
    {
        public class A
        {
            [Uno.Testing.Test] public static void test_var_04() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                var x = 1;
                return 0;
            }
        }
    
        namespace var
        {
        }
    }
}
