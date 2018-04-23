namespace Mono.gtest_060
{
    using Uno;
    
    interface IFoo
    {
        MyList<U> Map<U> ();
    }
    
    class MyList<T>
    {
        public void Hello (T t)
        {
            Console.WriteLine (t);
        }
    }
    
    class Foo : IFoo
    {
        public MyList<T> Map<T> ()
        {
            return new MyList<T> ();
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_060() { Main(); }
        public static void Main()
        {
            Foo foo = new Foo ();
            MyList<int> list = foo.Map<int> ();
            list.Hello (9);
        }
    }
}
