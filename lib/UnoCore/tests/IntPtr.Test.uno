using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class IntPtrTest
    {
        [Test]
        public void Add()
        {
            IntPtr a = IntPtr.Add(IntPtr.Zero, 2);
            IntPtr b = IntPtr.Add(IntPtr.Add(IntPtr.Zero, 1), 1);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void GetHashCode()
        {
            var a = IntPtr.Add(IntPtr.Zero, 2);
            var b = IntPtr.Add(IntPtr.Add(IntPtr.Zero, 1), 1);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
