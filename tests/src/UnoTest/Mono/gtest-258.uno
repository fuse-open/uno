namespace Mono.gtest_258
{
    using Uno;
    
    public class A
    {
        public A ()
        { }
    }
    
    public class B
    { }
    
    class Foo<T>
        where T : new ()
    { }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_258() { Main(); }
        public static void Main()
        {
            Foo<A> foo = new Foo<A> ();
        }
    }
}
