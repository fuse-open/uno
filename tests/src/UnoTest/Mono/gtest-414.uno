namespace Mono.gtest_414
{
    using Uno;

    namespace MonoTest
    {
        public class MainClass
        {
            [Uno.Testing.Test] public static void gtest_414() { Main(); }
        public static void Main()
            {
            }
        }

        public interface ITest
        {
            void Get<T> (object o);
        }

        public class TestImpl : ITest
        {
            public void Get<T> ()
            {
            }

            public void Get<T> (object o)
            {
            }

            void ITest.Get<T> (object o)
            {
            }
        }

        interface IG<T>
        {
            void M ();
        }

        class C : IG<int>, IG<string>
        {
            void IG<int>.M ()
            {
            }

            void IG<string>.M ()
            {
            }
        }
    }
}
