namespace Mono.gtest_named_02
{
    public class D
    {
        public static void Foo (int d = true ? 1 : 0)
        {
        }
    
        [Uno.Testing.Test] public static void gtest_named_02() { Main(); }
        public static void Main()
        {
        }
    }
}
