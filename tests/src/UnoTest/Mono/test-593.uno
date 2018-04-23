namespace Mono.test_593
{
    using Uno;
    
    interface I
    {
        void Finalize ();
    }
    
    class MainClass
    {
        void Foo (I i)
        {
            i.Finalize ();
        }
    
        [Uno.Testing.Test] public static void test_593() { Main(); }
        public static void Main()
        {
        }
    }
}
