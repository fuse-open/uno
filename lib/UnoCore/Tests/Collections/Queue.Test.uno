using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public class QueueTest
    {
        public Queue<int> CreateDummyQueue(int count)
        {
            var result = new Queue<int>();
            for (var i = 0; i < count; i++)
            {
                result.Enqueue(i);
            }
            return result;
        }

        [Test]
        public void ClearEmpty()
        {
            var queue = CreateDummyQueue(0);
            Assert.AreEqual(0, queue.Count);
            queue.Clear();
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void ClearOne()
        {
            var queue = CreateDummyQueue(1);
            Assert.AreEqual(1, queue.Count);
            queue.Clear();
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void ClearMany()
        {
            var queue = CreateDummyQueue(25);
            Assert.AreEqual(queue.Count, 25);
            queue.Clear();
            Assert.AreEqual(queue.Count, 0);
        }

        [Test]
        public void Contains()
        {
            var queue = CreateDummyQueue(12);
            Assert.IsTrue(queue.Contains(7));
            Assert.IsTrue(queue.Contains(0));
            Assert.IsTrue(queue.Contains(11));
            Assert.IsFalse(queue.Contains(12));
            Assert.IsFalse(queue.Contains(-2));
        }

        [Test]
        public void ContainsMany()
        {
            var l = CreateDummyQueue(5000);
            for (int i = 0; i < 5000; i++)
            {
                Assert.IsTrue(l.Contains(i));
            }
        }

        [Test]
        public void Enqueue()
        {
            var queue = CreateDummyQueue(5);
            Assert.AreEqual(5, queue.Count);
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2,3,4}, queue);
            queue.Enqueue(17);
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2,3,4,17}, queue);
            queue.Enqueue(2);
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2,3,4,17,2}, queue);
        }

        [Test]
        public void EnqueueMany()
        {
            var queue = CreateDummyQueue(1000);
            for (var i = 0; i < 10000; i++)
            {
                Assert.AreEqual(1000+i, queue.Count);
                queue.Enqueue(i);
            }
        }

        [Test]
        public void Dequeue()
        {
            var queue = CreateDummyQueue(9);
            Assert.AreEqual(9, queue.Count);
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2,3,4,5,6,7,8}, queue);
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreCollectionsEqual<int>(new int[] {1,2,3,4,5,6,7,8}, queue);
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreCollectionsEqual<int>(new int[] {2,3,4,5,6,7,8}, queue);
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreCollectionsEqual<int>(new int[] {3,4,5,6,7,8}, queue);
        }

        [Test]
        public void DequeueEmpty()
        {
            Assert.Throws<InvalidOperationException>(DequeueEmptyInvalidAction);
        }

        public void DequeueEmptyInvalidAction()
        {
            var queue = CreateDummyQueue(0);
            queue.Dequeue();
        }

        [Test]
        public void Peek()
        {
            var queue = CreateDummyQueue(3);
            Assert.AreEqual(3, queue.Count);
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2}, queue);
            Assert.AreEqual(0, queue.Peek());
            Assert.AreEqual(0, queue.Peek());
            Assert.AreCollectionsEqual<int>(new int[] {0,1,2}, queue);
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreEqual(1, queue.Peek());
            Assert.AreEqual(1, queue.Peek());
            Assert.AreCollectionsEqual<int>(new int[] {1,2}, queue);
            queue.Enqueue(7);
            Assert.AreEqual(1, queue.Peek());
            Assert.AreEqual(1, queue.Peek());
            Assert.AreCollectionsEqual<int>(new int[] {1,2,7}, queue);
        }

        [Test]
        public void PeekEmpty()
        {
            Assert.Throws<InvalidOperationException>(PeekEmptyInvalidAction);
        }

        public void PeekEmptyInvalidAction()
        {
            var queue = CreateDummyQueue(0);
            queue.Peek();
        }

        [Test]
        public void Enumeration()
        {
            var queue = CreateDummyQueue(10);
            var enumerator = queue.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(index, (int)enumerator.Current);
                Assert.AreEqual(0, queue.Peek());
                index++;
            }
        }

        [Test]
        public void EnumerationChange()
        {
            Assert.Throws<InvalidOperationException>(EnumerationChangeInvalidAction);
        }

        public void EnumerationChangeInvalidAction()
        {
            var queue = CreateDummyQueue(8);
            var enumerator = queue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                queue.Enqueue(7);
            }
        }
    }
}
