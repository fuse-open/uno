namespace Mono.gtest_245
{
    using Uno;
    
    class DerivedGenericClass<T> : BaseClass
    {
            public override void Foo () {}
    
            public void Baz ()
            {
                    Foo ();
            }
    }
    
    abstract class BaseClass
    {
            public abstract void Foo ();
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_245() { Main(); }
        public static void Main()
        {
        }
    }
}
