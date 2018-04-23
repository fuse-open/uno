using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public interface CollectionFactory
    {
        ICollection<T> CreateCollection<T>();
    }

    public static class CollectionTester
    {

        public static void Add(CollectionFactory factory)
        {
            var col = factory.CreateCollection<int>();
            col.Add(6);
            col.Add(7);
            col.Add(123);
            Assert.AreCollectionsEqual<int>(new int[] {6, 7, 123}, col);
        }

        public static void Clear(CollectionFactory factory)
        {
            var col = factory.CreateCollection<int>();
            col.Add(6);
            col.Add(7);
            col.Add(123);
            col.Clear();
            Assert.AreCollectionsEqual<int>(new int[] {}, col);
            Assert.AreEqual(0, col.Count);
        }

        public static void Contains(CollectionFactory factory)
        {
            var col = factory.CreateCollection<int>();
            col.Add(1);
            col.Add(2);
            col.Add(4);
            Assert.IsFalse(col.Contains(0));
            Assert.IsTrue(col.Contains(1));
            Assert.IsTrue(col.Contains(2));
            Assert.IsFalse(col.Contains(3));
            Assert.IsTrue(col.Contains(4));
            Assert.IsFalse(col.Contains(5));
        }

        public static void Remove(CollectionFactory factory)
        {
            var col = factory.CreateCollection<int>();
            col.Add(6);
            col.Add(7);
            col.Add(123);
            Assert.IsFalse(col.Remove(5));
            Assert.IsTrue(col.Remove(6));
            Assert.IsFalse(col.Remove(6));
            Assert.AreCollectionsEqual<int>(new int[] {7, 123}, col);
        }

        public static void Count(CollectionFactory factory)
        {
            var col = factory.CreateCollection<int>();
            Assert.AreEqual(0, col.Count);
            col.Add(6);
            Assert.AreEqual(1, col.Count);
            col.Add(7);
            Assert.AreEqual(2, col.Count);
            col.Add(123);
            Assert.AreEqual(3, col.Count);
            col.Remove(7);
            Assert.AreEqual(2, col.Count);
        }

        public static void All(CollectionFactory factory)
        {
            Add(factory);
            Clear(factory);
            Contains(factory);
            Remove(factory);
            Count(factory);
        }
    }
}
