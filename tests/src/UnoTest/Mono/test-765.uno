namespace Mono.test_765
{
    using Uno;
    
    class B : A
    {
        public static void Foo (int i)
        {
        }
    }
    
    class A
    {
        public static void Foo (string s)
        {
        }
    }
    
    
    public static class Test
    {
        [Uno.Testing.Test] public static void test_765() { Main(); }
        public static void Main()
        {
            B.Foo ("a");
        }
    }
}
