namespace Mono.test_partial_19
{
    using Uno;
    
    namespace Bug
    {
        public static partial class GL
        {
            static partial class Core
            {
                internal static bool A () { return true; }
            }
            
            /*internal static partial class Bar
            {
                internal static bool A () { return true; }
            }*/
        }
    
        partial class GL
        {
            [Uno.Testing.Test] public static void test_partial_19() { Main(); }
        public static void Main()
            {
                Core.A ();
                //Bar.A ();
            }
    
            internal partial class Core
            {
            }
            
            /*partial class Bar
            {
            }*/
        }
    }
}
