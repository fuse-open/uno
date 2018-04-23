namespace Mono.gtest_396
{
    using Uno;
    
    public class Test
    {
        public static void Invoke<A, TR>(Func<A, Func<TR>> callee, A arg1, TR result)
        {
        }
    
        static Func<int> Method (string arg)
        {
            return null;
        }
    
        [Uno.Testing.Test] public static void gtest_396() { Main(); }
        public static void Main()
        {
            Invoke(Method, "one", 1);
        }
    }
}
