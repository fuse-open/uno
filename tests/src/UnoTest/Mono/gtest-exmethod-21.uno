namespace Mono.gtest_exmethod_21
{
    using Uno;
    using External;
    
    interface I
    {
    }
    
    namespace Outer.Inner
    {
        class Test {
            static void M (I list)
            {
                list.AddRange();
            }
            
            [Uno.Testing.Test] public static void gtest_exmethod_21() { Main(); }
        public static void Main()
            {
            }
        }
    }
    
    namespace Outer
    {
    }
    
    namespace External
    {
        static class ExtensionMethods {
            public static void AddRange (this I list)
            {
            }
        }
    }
}
