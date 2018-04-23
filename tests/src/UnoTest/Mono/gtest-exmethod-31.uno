namespace Mono.gtest_exmethod_31
{
    using Uno;
    using Uno.Collections;
    using N2;
    
    namespace N
    {
        static class S
        {
            internal static void Map<T>(this int i, Func<T, string> f)
            {
            }
        }
    }
    
    namespace N2
    {
        static class S2
        {
            internal static void Map(this int i, int k)
            {
            }
        }
    }
    
    namespace M
    {
        using N;
        
        class C
        {
            [Uno.Testing.Test] public static void gtest_exmethod_31() { Main(); }
        public static void Main()
            {
                1.Map(2);
            }
        }
    }
}
