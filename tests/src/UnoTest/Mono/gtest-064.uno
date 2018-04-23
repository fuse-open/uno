namespace Mono.gtest_064
{
    using Uno;
    
    public interface IHello<T>
    { }
    
    public interface IFoo<T>
    {
        IHello<T> GetHello ();
    }
    
    public interface IBar<T> : IFoo<T>
    { }
    
    public class Foo<T> : IBar<T>, IFoo<T>
    {
        public Hello GetHello ()
        {
            return new Hello (this);
        }
    
        IHello<T> IFoo<T>.GetHello ()
        {
            return new Hello (this);
        }
    
        public class Hello : IHello<T>
        {
            public readonly Foo<T> Foo;
    
            public Hello (Foo<T> foo)
            {
                this.Foo = foo;
            }
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_064() { Main(); }
        public static void Main()
        { }
    }
}
