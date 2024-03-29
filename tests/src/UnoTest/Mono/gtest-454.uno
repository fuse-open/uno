namespace Mono.gtest_454
{
    using Uno;

    namespace TestNS
    {
        public class TestCase
        {
            public static int Compare<T> (T[] array1, T[] array2)
            {
                return 0;
            }

            public static void DoSomething<T> (Func<T, T, int> fn)
            {
                Console.WriteLine (fn (default (T), default (T)));
            }

            [Uno.Testing.Test] public static void gtest_454() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                DoSomething<byte[]> (TestCase.Compare);
                return 0;
            }
        }
    }
}
