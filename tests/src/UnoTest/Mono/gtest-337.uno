namespace Mono.gtest_337
{
    using Uno;
    
    class X {
            static void SetValue<T> (object o, T x)
            {
            }
    
            [Uno.Testing.Test] public static void gtest_337() { Main(); }
        public static void Main()
            {
                    object o = null;
                    double [] d = null;
    
                    SetValue (o, d);
            }
    }
}
