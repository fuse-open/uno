namespace Mono.gtest_248
{
    using Uno;
    
    public class Foo<T>
    { }
    
    class X
    {
        static bool Test (object o)
        {
            return o is Foo<int> ? true : false;
        }
    
        [Uno.Testing.Test] public static void gtest_248() { Main(); }
        public static void Main()
        { }
    }
}
