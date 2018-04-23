namespace Mono.gtest_exmethod_07
{
    using Uno.Collections;
    
    interface IA
    {
    }
    
    interface IB : IA
    {
    }
    
    static class E
    {
        internal static void ToReadOnly<T>(this IEnumerable<T> source)
        {
        }
        
        internal static void To (this IA i)
        {
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_exmethod_07() { Main(); }
        public static void Main()
        {
        }
        
        public static void Test (IEnumerable<bool> bindings)
        {
            bindings.ToReadOnly();
            
            IB ib = null;
            ib.To ();
        }
    }
}
