using Uno;
using Uno.Testing;

namespace Uno.Test
{
    internal static class ArrayHelper
    {
        internal static void AssertArrayEqual<T>(T[] a, T[] b)
        {
            Assert.AreEqual(a.Length, b.Length);

            for (int i = 0; i < a.Length; ++i)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }

        internal static void AssertArrayEqual<T>(T[] a, T[] b, int length)
        {
            Assert.IsTrue(length < a.Length);
            Assert.IsTrue(length < b.Length);

            for (int i = 0; i < length; ++i)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }
    }
}