namespace Mono.test_391
{
    class C
    {
        void Foo (int i)
        {
        }
        
        void Foo (ref int i)
        {
        }
        
        void Bar (out bool b)
        {
            b = false;
        }
        
        void Bar (bool b)
        {
        }
        
        [Uno.Testing.Test] public static void test_391() { Main(); }
        public static void Main()
        {
        }
    }
}
