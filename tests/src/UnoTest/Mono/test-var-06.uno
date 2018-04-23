namespace Mono.test_var_06
{
    // Tests variable type inference with the var keyword when using the for-statement
    
    using Uno;
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_var_06() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            for (var i = 0; i < 1; ++i)
                if (i.GetType() != typeof (int))
                    return 1;
            
            return 0;
        }
    }
}
