using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;
using Uno.Testing.Assert;

namespace Uno.Test
{
    public class ArrayIndexTest
    {
        [Test]
        public void WriteOutOfBounds()
        {
            Assert.Throws<IndexOutOfRangeException>(WriteAfterBounds);
            Assert.Throws<IndexOutOfRangeException>(WriteBeforeBounds);
        }

        void WriteAfterBounds()
        {
            var a = new [] { 0, 1, 2 };
            a[3] = 3;
        }

        void WriteBeforeBounds()
        {
            var a = new [] { 0, 1, 2 };
            a[-1] = -1;
        }
    }
}
