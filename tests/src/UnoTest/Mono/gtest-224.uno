namespace Mono.gtest_224
{
    class Base
    {
            public virtual void Foo<T> () {}
    }
    
    class Derived : Base
    {
            public override void Foo <T> () {}
    }
    
    class Driver
    {
            [Uno.Testing.Test] public static void gtest_224() { Main(); }
        public static void Main()
            {
                    new Derived ().Foo<int> ();
            }
    }
}
