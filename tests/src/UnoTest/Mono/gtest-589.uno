namespace Mono.gtest_589
{
    using Uno;

    public class Z : IGenericInterface<Z>
    {
        public Z Start ()
        {
            return this;
        }

        Z IGenericInterface<Z>.Start ()
        {
            throw new ApplicationException ();
        }
    }

    public interface IGenericInterface<T>
    {
        T Start ();
    }

    public class A<T> where T : Z, IGenericInterface<int>
    {
        public void SomeOperation (T t)
        {
            t.Start ();
        }
    }

    public class C : Z, IGenericInterface<int>
    {
        int IGenericInterface<int>.Start ()
        {
            throw new NotImplementedException ();
        }

        [Uno.Testing.Test] public static void gtest_589() { Main(); }
        public static void Main()
        {
            new A<C> ().SomeOperation (new C ());
        }
    }
}
