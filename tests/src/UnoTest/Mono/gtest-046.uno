namespace Mono.gtest_046
{
    // Generic delegates.
    
    using Uno;
    
    delegate void Test<T> (T t);
    
    class Foo<T>
    {
        public event Test<T> MyEvent;
    
        public void Hello (T t)
        {
            if (MyEvent != null)
                MyEvent (t);
        }
    }
    
    class X
    {
        static void do_hello (string hello)
        {
            Console.WriteLine ("Hello: {0}", hello);
        }
    
        [Uno.Testing.Test] public static void gtest_046() { Main(); }
        public static void Main()
        {
            Foo<string> foo = new Foo<string> ();
            foo.MyEvent += new Test<string> (do_hello);
            foo.Hello ("Boston");
        }
    }
}
