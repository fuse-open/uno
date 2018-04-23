namespace Mono.test_190
{
    class A
    {
        private int foo = 0;
    
        class B : A
        {
            void Test ()
            {
                foo = 3;
            }
        }
    
        class C
        {
            void Test (A a)
            {
                a.foo = 4;
            }
        }
    
        [Uno.Testing.Test] public static void test_190() { Main(); }
        public static void Main()
        { }
    }
}
