namespace Mono.gtest_exmethod_20
{
    using Uno;
    using Uno.Collections;
    
    interface I
    {
    }
    
    namespace Outer.Inner
    {
        class Test {
            static void M (I list)
            {
                list.AddRange(new Test[0]);
            }
            
            [Uno.Testing.Test] public static void gtest_exmethod_20() { Main(); }
        public static void Main()
            {
            }
        }
    }
    
    namespace Outer {
        static class ExtensionMethods {
            public static void AddRange<T>(this I list, IEnumerable<T> items)
            {
            }
        }
    }
}
