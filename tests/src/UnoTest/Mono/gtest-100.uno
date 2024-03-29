namespace Mono.gtest_100
{
    using Uno;

    // Runtime test for mono_class_setup_vtable()
    namespace C5
    {
        public interface ICollection<T>
        {
            void Test<U> ();
        }

        public abstract class ArrayBase<T> : ICollection<T>
        {
            void ICollection<T>.Test<U> ()
            { }
        }

        public class ArrayList<V> : ArrayBase<V>
        {
        }
    }

    class X
    {
        [Uno.Testing.Test] public static void gtest_100() { Main(); }
        public static void Main()
        {
            C5.ArrayList<int> array = new C5.ArrayList<int> ();
        }
    }
}
