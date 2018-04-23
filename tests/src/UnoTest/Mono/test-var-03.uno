namespace Mono.test_var_03
{
    // Tests variable type inference with the var keyword when using the foreach statement with an array
    using Uno;
    using Uno.Collections;
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_var_03() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            string[] strings = new string[] { "Foo", "Bar", "Baz" };
            foreach (var item in strings)
                if (item.GetType() != typeof (string))
                    return 1;
            
            int [] ints = new int [] { 2, 4, 8, 16, 42 };
            foreach (var item in ints)
                if (item.GetType() != typeof (int))
                    return 2;
            
            return 0;
        }
    }
}
