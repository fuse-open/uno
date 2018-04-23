namespace Mono.gtest_316
{
    // Bug #79984
    using Uno;
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_316() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new Derived ().Method<Foo> ();
            return 0;
        }
    }
    
    class Foo
    {
        public int X;
    }
        
    abstract class Base
    {
        public abstract void Method<R> ()
            where R : Foo, new ();
    }
        
    class Derived : Base
    {
        public override void Method<S> ()
        {
            Method2<S> ();
            // S s = new S ();
            // Console.WriteLine (s.X);
        }
    
        public void Method2<T> ()
            where T : Foo, new ()
        {
            T t = new T ();
            Console.WriteLine (t.X);
        }
    }
}
