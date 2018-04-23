namespace Mono.test_663
{
    class A
    {
        public static implicit operator int (A a)
        {
            return 1;
        }
        
        public static implicit operator bool (A a)
        {
            return false;
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void test_663() { Main(); }
        public static void Main()
        {
            switch (new A ())
            {
                default: break;
            }
        }
    }
}
