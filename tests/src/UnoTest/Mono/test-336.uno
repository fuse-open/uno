namespace Mono.test_336
{
    using Uno;
    
    public delegate void Foo ();
    public delegate void Bar (int x);
    
    class X
    {
        public X (Foo foo)
        { }
    
        public X (Bar bar)
        { }
    
        static void Test ()
        { }
    
        [Uno.Testing.Test] public static void test_336() { Main(); }
        public static void Main()
        {
            X x = new X (Test);
        }
    }
}
