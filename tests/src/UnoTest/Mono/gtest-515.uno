namespace Mono.gtest_515
{
    using Uno;

    interface I<T>
    {
    }

    class A
    {
        public virtual I<T> Foo<T> () where T : IDisposable
        {
            return null;
        }
    }

    class AA : A
    {
        public override I<V> Foo<V> ()
        {
            return base.Foo<V> ();
        }
    }

    class B : AA, IDisposable
    {
        public void Dispose ()
        {
        }

        public override I<R> Foo<R> ()
        {
            return base.Foo<R> ();
        }

        [Uno.Testing.Test] public static void gtest_515() { Main(); }
        public static void Main()
        {
            new B ().Foo<B> ();
        }
    }
}
