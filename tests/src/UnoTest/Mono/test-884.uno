namespace Mono.test_884
{
    // Compiler options: -optimize
    
    using Uno;
    
    class C
    {
        [Uno.Testing.Test] public static void test_884() { Main(); }
        public static void Main()
        {
            AddEH<string> ();
        }
    
        static void AddEH<T>()
        {
            var e = new E<T> ();
            e.EEvent += EHandler;
        }
    
        static void EHandler ()
        {
        }
    
        class E<T>
        {
            public delegate void EMethod ();
            public event EMethod EEvent;
        }
    }
}
