namespace Mono.test_210
{
    delegate void FooHandler ();
    
    class X
    {
        public static void foo ()
        { }
    
        [Uno.Testing.Test] public static void test_210() { Main(); }
        public static void Main()
        {
            object o = new FooHandler (foo);
            ((FooHandler) o) ();
        }
    }
}
