namespace Mono.test_225
{
    using Uno;
    
    class A {
        public int foo = 1;
    }
    
    class B : A {
        public new int foo ()
        {
            return 1;
        }
        
        [Uno.Testing.Test] public static void test_225() { Main(); }
        public static void Main()
        {
            B b = new B ();
            Console.WriteLine (b.foo ());
        }
    }
}
