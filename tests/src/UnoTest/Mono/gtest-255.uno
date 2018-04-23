namespace Mono.gtest_255
{
    using Uno;
    
    public abstract class A
    {
            public abstract T Foo<T> ();
    }
    
    public abstract class B : A
    {
            public override T Foo<T> ()
            {
                    return default (T);
            }
    }
    
    public class C : B
    {
        [Uno.Testing.Test] public static void gtest_255() { Main(); }
        public static void Main()
        { }
    }
}
