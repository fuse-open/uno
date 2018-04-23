using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Extensions.Test
{
    public class IListExtensionsTest
    {
        [Test]
        public void RemoveLastEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveLastEmptyHelper);
        }

        void RemoveLastEmptyHelper()
        {
            var l = new List<int>();
            // Remove last should throw, but we put it in an add call to
            // avoid compiler warnings about not using the return value
            l.Add(l.RemoveLast());
        }

        [Test]
        public void RemoveLastSingleItem()
        {
            var l = new List<string>();
            l.Add("87");
            Assert.AreEqual("87", l.RemoveLast());
        }

        [Test]
        public void RemoveLastLots()
        {
            var l = new List<int>();
            for (int i = 0; i < 2000; i++)
                l.Add(i);
            for (int i = 0; i < 2000; i++)
                Assert.AreEqual(1999 - i, l.RemoveLast());
        }

        [Test]
        public void AddRange()
        {
            var l = new List<int>();
            var itemsToAdd = new int[] { 1, 2, 3};
            l.AddRange(itemsToAdd);

            Assert.AreEqual(3, l.Count);
            Assert.AreEqual(itemsToAdd, l.ToArray());
        }

        [Test]
        public void Find()
        {
            var l = new List<int>() { 1, 2, 5, 8, 9, 2 };

            Assert.AreEqual(l[2], l.Find(new MatchExpressions() { Integer = 5 }.MatchInt));
            Assert.AreEqual(l[1], l.Find(new MatchExpressions() { Integer = 2 }.MatchInt));
            Assert.AreEqual(0, l.Find(new MatchExpressions() { Integer = 12 }.MatchInt));
        }

        [Test]
        public void FindObject()
        {
            var l = new List<DummyItem>() { new DummyItem("name1"), new DummyItem("name2"), new DummyItem("name3"), new DummyItem("name4"), new DummyItem("name2") };

            Assert.AreEqual(l[2], l.Find(new MatchExpressions() { String = "name3" }.MatchDummyItemName));
            Assert.AreEqual(l[1], l.Find(new MatchExpressions() { String = "name2" }.MatchDummyItemName));
            Assert.AreEqual(null, l.Find(new MatchExpressions() { String = "name22" }.MatchDummyItemName));
        }

        [Test]
        public void FindAll()
        {
            var l = new List<int>() { 1, 2, 5, 8, 9, 2 };

            Assert.AreEqual(1, l.FindAll(new MatchExpressions() { Integer = 5 }.MatchInt).Count);
            Assert.AreEqual(2, l.FindAll(new MatchExpressions() { Integer = 2 }.MatchInt).Count);
            Assert.AreEqual(0, l.FindAll(new MatchExpressions() { Integer = 12 }.MatchInt).Count);
        }

        [Test]
        public void FindAllObjects()
        {
            var l = new List<DummyItem>() { new DummyItem("name1"), new DummyItem("name2"), new DummyItem("name3"), new DummyItem("name4"), new DummyItem("name2") };

            Assert.AreEqual(1, l.FindAll(new MatchExpressions() { String = "name3" }.MatchDummyItemName).Count);
            Assert.AreEqual(2, l.FindAll(new MatchExpressions() { String = "name2" }.MatchDummyItemName).Count);
            Assert.AreEqual(0, l.FindAll(new MatchExpressions() { String = "name22" }.MatchDummyItemName).Count);
        }

        [Test]
        public void InsertRange()
        {
            var l = new List<int>() { 1, 2, 3, 4, 5, 6 };

            l.InsertRange(0, new List<int>() { 10, 11 });
            Assert.AreEqual(8, l.Count);
            Assert.AreEqual(new int[] { 10, 11, 1, 2, 3, 4, 5, 6 }, l.ToArray());

            l.InsertRange(4, new List<int>() { 20, 21 });
            Assert.AreEqual(10, l.Count);
            Assert.AreEqual(new int[] { 10, 11, 1, 2, 20, 21, 3, 4, 5, 6 }, l.ToArray());

            l.InsertRange(10, new List<int>() { 30, 31 });
            Assert.AreEqual(12, l.Count);
            Assert.AreEqual(new int[] { 10, 11, 1, 2, 20, 21, 3, 4, 5, 6, 30, 31 }, l.ToArray());
        }

        [Test]
        public void Last()
        {
            var l = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.AreEqual(l[5], l.Last());
        }

        [Test]
        public void LastOrDefault()
        {
            var l = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.AreEqual(l[5], l.LastOrDefault());

            l = new List<int>();
            Assert.AreEqual(0, l.LastOrDefault());
        }

        [Test]
        public void Reverse()
        {
            var l = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.AreEqual(new int[] { 6, 5, 4, 3, 2, 1 }, l.Reverse().ToArray());

            l = new List<int>();
            Assert.AreEqual(new int[] { }, l.Reverse().ToArray());
        }

        [Test]
        public void InsertRangeOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertRangeOutOfRangeFunc);
        }

        private void InsertRangeOutOfRangeFunc()
        {
            var l = new List<int>() { 1, 2, 3 };
            l.InsertRange(4, new List<int>() { 10, 11, 12 });
        }
    }
}
