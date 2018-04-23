namespace Mono.gtest_partial_010
{
    using Uno;
    
    namespace A
    {
        interface IA<T>
        {
            int Foo (T value);
        }
    
        internal partial class C : IA<C.NA>
        {
            private abstract class NA
            {
            }
    
            int IA<NA>.Foo (NA value)
            {
                return 0;
            }
    
            [Uno.Testing.Test] public static void gtest_partial_010() { Main(); }
        public static void Main()
            {
            }
        }
    }
    
    namespace A
    {
        internal partial class C : IA<C.NB>
        {
            private class NB
            {
            }
    
            int IA<NB>.Foo (NB value)
            {
                return 0;
            }
        }
    }
}
