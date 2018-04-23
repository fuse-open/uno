namespace Mono.test_var_08
{
    using Uno;
    
    class X
    {
            int var;
    
            public X (int var, int i)
            {
                    var = i;
            }
    
            [Uno.Testing.Test] public static void test_var_08() { Main(); }
        public static void Main()
            {
            }
    }
}
