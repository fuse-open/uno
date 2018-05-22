using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public class StackTest
    {
        [Test]
        public void Clear()
        {
            var s = CreateDummyStack(15);
            Assert.AreEqual(s.Count, 15);
            s.Clear();
            Assert.AreEqual(s.Count, 0);
        }

        [Test]
        public void ClearMany()
        {
            var s = CreateDummyStack(5000);
            Assert.AreEqual(s.Count, 5000);
            s.Clear();
            Assert.AreEqual(s.Count, 0);
        }

        [Test]
        public void Push()
        {
            var s = new Stack<int>();
            s.Push(6);
            s.Push(7);
            s.Push(123);
            Assert.AreCollectionsEqual<int>(new int[] {123, 7, 6}, s);
        }

        [Test]
        public void PushMany()
        {
            var s = new Stack<int>();
            Assert.AreEqual(s.Count, 0);
            for (int i = 0; i < 5000; i++)
            {
                s.Push(i);
                Assert.AreEqual(i + 1, s.Count);
            }
        }

        [Test]
        public void Contains()
        {
            var s = CreateDummyStack(10);
            Assert.IsTrue(s.Contains(5));
            Assert.IsTrue(s.Contains(9));
            Assert.IsFalse(s.Contains(10));
            Assert.IsFalse(s.Contains(-1));
        }

        [Test]
        public void ContainsMany()
        {
            var s = CreateDummyStack(5000);
            for (int i = 0; i < 5000; i++)
            {
                Assert.IsTrue(s.Contains(i));
            }
        }

        [Test]
        public void Pop()
        {
            var s = CreateDummyStack(5);

            var popItem = s.Pop();
            Assert.AreEqual(4, popItem);
            Assert.AreEqual(4, s.Count);

            popItem = s.Pop();
            Assert.AreEqual(3, popItem);
            Assert.AreEqual(3, s.Count);

            s.Push(10);
            popItem = s.Pop();
            Assert.AreEqual(10, popItem);
            Assert.AreEqual(3, s.Count);
        }

        [Test]
        public void Peek()
        {
            var s = CreateDummyStack(5);

            var peekItem = s.Peek();
            Assert.AreEqual(4, peekItem);
            Assert.AreEqual(5, s.Count);

            peekItem = s.Peek();
            Assert.AreEqual(4, peekItem);
            Assert.AreEqual(5, s.Count);
        }

        [Test]
        public void PopEmptyStack()
        {
            Assert.Throws<InvalidOperationException>(PopEmptyStackInvalidAction);
        }

        public void PopEmptyStackInvalidAction()
        {
            var s = new Stack<int>();
            s.Pop();
        }

        [Test]
        public void PeekEmptyStack()
        {
            Assert.Throws<InvalidOperationException>(PeekEmptyStackInvalidAction);
        }

        public void PeekEmptyStackInvalidAction()
        {
            var s = new Stack<int>();
            s.Peek();
        }

        [Test]
        public void Count()
        {
            var s1 = CreateDummyStack(100);
            Assert.AreEqual(100, s1.Count);
            for (int i = 0; i < 10; i++)
                s1.Pop();
            Assert.AreEqual(90, s1.Count);
            s1.Peek();
            Assert.AreEqual(90, s1.Count);

            var s2 = new Stack<int>();
            Assert.AreEqual(0, s2.Count);
            for (int i = 0; i < 10; i++)
                s2.Push(i);
            Assert.AreEqual(10, s2.Count);

            var s3 = new Stack<int>(50);
            Assert.AreEqual(0, s3.Count);
        }

        private Stack<int> CreateDummyStack(int nItems)
        {
            var ret = new Stack<int>();
            for (int i = 0; i < nItems; i++)
                ret.Push(i);
            return ret;
        }
    }
}
