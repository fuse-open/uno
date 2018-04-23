namespace Mono.gtest_473
{
    class A<X>
    {
        public virtual void Foo<T> () where T : A<T>
        {
        }
    }
    
    class B : A<int>
    {
        public override void Foo<T> ()
        {
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_473() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new B ();
            return 0;
        }
    }
}
