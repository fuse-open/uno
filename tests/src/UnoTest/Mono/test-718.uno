namespace Mono.test_718
{
    using Uno;
    
    class A
    {
        public static void Foo (int x, int y)
        {
        }
    }
    
    sealed class B : A
    {
        [Uno.Testing.Test] public static void test_718() { Main(); }
        public static void Main()
        {
            Foo (1, 2);
        }
        
        void Foo (int i)
        {
        }
    }
}
