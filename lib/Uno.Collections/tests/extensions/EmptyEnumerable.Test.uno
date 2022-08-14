using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Extensions.Test
{
    public class EmptyEnumerableTest
    {
        [Test]
        public void Instantiation()
        {
            var c = new EmptyEnumerable<string>();
            Assert.AreNotEqual(null, c);
        }

        [Test]
        public void GetEnumerator()
        {
            var c = new EmptyEnumerable<double>();
            var e = c.GetEnumerator();
            Assert.AreNotEqual(null, e);
        }

        [Test]
        public void EnumeratorCurrent()
        {
            Assert.Throws<InvalidOperationException>(EnumeratorCurrentHelper);
        }

        void EnumeratorCurrentHelper()
        {
            var c = new EmptyEnumerable<int>();
            var e = c.GetEnumerator();
            // Current should throw, but we use the AreNotEqual call to
            // suppress compiler warnings about not using the value
            // otherwise
            Assert.AreNotEqual(null, e.Current);
        }

        [Test]
        public void EnumeratorReset()
        {
            var c = new EmptyEnumerable<int>();
            var e = c.GetEnumerator();
            e.Reset();
        }

        [Test]
        public void EnumeratorMoveNext()
        {
            var c = new EmptyEnumerable<int>();
            var e = c.GetEnumerator();
            for (int i = 0; i < 100; i++)
                Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void EnumeratorDispose()
        {
            var c = new EmptyEnumerable<int>();
            var e = c.GetEnumerator();
            e.Dispose();
        }
    }
}
