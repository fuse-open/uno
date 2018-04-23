namespace Mono.gtest_025
{
    class Foo
    {
        public Foo ()
        { }
    
        public void Hello<T> (T t)
        {
            // We're boxing the type parameter `T' to an object here.
            Whatever (t);
        }
    
        public void Whatever (object o)
        {
            Console.WriteLine (o.GetType ());
        }
    }
    
    class X
    {
        static void Test (Foo foo)
        {
            foo.Hello<int> (531);
        }
    
        [Uno.Testing.Test] public static void gtest_025() { Main(); }
        public static void Main()
        {
            Foo foo = new Foo ();
            Test (foo);
        }
    }
}
