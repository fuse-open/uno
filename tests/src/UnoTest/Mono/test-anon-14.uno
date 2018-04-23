namespace Mono.test_anon_14
{
    // Compiler options: -langversion:default
    
    //
    // Anonymous method group conversions
    //
    
    class X {
        delegate void T ();
        static event T Click;
    
        static void Method ()
        {
        }
    
        [Uno.Testing.Test] public static void test_anon_14() { Main(); }
        public static void Main()
        {
            T t;
    
            // Method group assignment
            t = Method;
    
            Click += Method;
        }
    }
}
