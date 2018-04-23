namespace Mono.test_partial_16
{
    namespace A
    {
        partial class C
        {
        }
    }
    
    namespace A
    {
        using B;
        
        partial class C
        {
            public static bool f = C2.Test ();
            object o = new C2().Test_I ();
        }
    }
    
    namespace B
    {
        partial class C2
        {
            public static bool Test ()
            {
                return false;
            }
            
            public object Test_I ()
            {
                return this;
            }
            
            [Uno.Testing.Test] public static void test_partial_16() { Main(); }
        public static void Main()
            {
            }
    
        }
    }
}
