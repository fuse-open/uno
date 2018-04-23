namespace Mono.gtest_optional_37
{
    using Uno;
    
    class Test1
    {
        static object Foo (int arg = 1, int arg2 = 2)
        {
            return null;
        }
    
        static object Foo (object arg, object arg2)
        {
            return null;
        }
    
        [Uno.Testing.Test] public static void gtest_optional_37() { Main(); }
        public static void Main()
        {
            Func<int, int, object> o = Foo;
        }
    }
}
