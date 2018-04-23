using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    static class ObservableListTestHelpers
    {
        public static void Fill(this ObservableList<int> l, int count)
        {
            for (int i = 0; i < count; i++)
            l.Add(i);
        }
    }

    public class ObservableListTest
    {
        static void BlankHandler<T>(T value)
        {
        }

        [Test]
        public void Instantiation()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            Assert.AreNotEqual(null, l);
        }

        [Test]
        public void AddedHandlerNull()
        {
            Assert.Throws<ArgumentNullException>(AddedHandlerNullHelper);
        }

        void AddedHandlerNullHelper()
        {
            var l = new ObservableList<int>(null, BlankHandler);
            // Above should fail; this is to avoid
            // unused variable compiler warnings
            Assert.AreNotEqual(null, l);
        }

        [Test]
        public void RemovedHandlerNull()
        {
            Assert.Throws<ArgumentNullException>(AddedHandlerNullHelper);
        }

        void RemovedHandlerNullHelper()
        {
            var l = new ObservableList<int>(BlankHandler, null);
            // Above should fail; this is to avoid
            // unused variable compiler warnings
            Assert.AreNotEqual(null, l);
        }

        [Test]
        public void AddedAndRemovedHandlerNull()
        {
            Assert.Throws<ArgumentNullException>(AddedHandlerNullHelper);
        }

        void AddedAndRemovedHandlerNullHelper()
        {
            var l = new ObservableList<int>(null, null);
            // Above should fail; this is to avoid
            // unused variable compiler warnings
            Assert.AreNotEqual(null, l);
        }

        [Test]
        public void ClearEmpty()
        {
            var l = new ObservableList<double>(BlankHandler, BlankHandler);
            l.Clear();
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void ClearSingleItem()
        {
            var l = new ObservableList<string>(BlankHandler, BlankHandler);
            l.Add("yes");
            l.Clear();
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void ClearLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(2000);
            l.Clear();
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void ContainsEmpty()
        {
            var l = new ObservableList<bool>(BlankHandler, BlankHandler);
            Assert.IsFalse(l.Contains(true));
            Assert.IsFalse(l.Contains(false));
        }

        [Test]
        public void ContainsSingleItem()
        {
            var l = new ObservableList<bool>(BlankHandler, BlankHandler);
            l.Add(true);
            Assert.IsTrue(l.Contains(true));
            Assert.IsFalse(l.Contains(false));
        }

        [Test]
        public void ContainsLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(3000);
            for (int i = 0; i < 3000; i++)
            Assert.IsTrue(l.Contains(i));
            Assert.IsFalse(l.Contains(-1));
            Assert.IsFalse(l.Contains(3000));
        }

        [Test]
        public void AddSingleItem()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(68);
            Assert.AreEqual(68, l[0]);
        }

        [Test]
        public void AddLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(5000);
            for (int i = 0; i < 5000; i++)
            Assert.AreEqual(i, l[i]);
        }

        [Test]
        public void InsertSingleItem()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Insert(0, 2884);
            Assert.AreEqual(2884, l[0]);
        }

        [Test]
        public void InsertLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            for (int i = 0; i < 1000; i++)
            l.Insert(0, i);
            for (int i = 0; i < 1000; i++)
            Assert.AreEqual(999 - i, l[i]);
        }

        [Test]
        public void InsertLotsAtEnd()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            for (int i = 0; i < 1000; i++)
            l.Insert(i, i);
            for (int i = 0; i < 1000; i++)
            Assert.AreEqual(i, l[i]);
        }

        [Test]
        public void InsertNegativePositionEmpty01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertNegativePositionEmptyHelper01);
        }

        void InsertNegativePositionEmptyHelper01()
        {
            var l = new ObservableList<float>(BlankHandler, BlankHandler);
            l.Insert(-1, 4.5f);
        }

        [Test]
        public void InsertNegativePositionEmpty02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertNegativePositionEmptyHelper02);
        }

        void InsertNegativePositionEmptyHelper02()
        {
            var l = new ObservableList<float>(BlankHandler, BlankHandler);
            l.Insert(-21349028, -12.4444f);
        }

        [Test]
        public void InsertOutOfBoundsEmpty01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertOutOfBoundsEmptyHelper01);
        }

        void InsertOutOfBoundsEmptyHelper01()
        {
            var l = new ObservableList<string>(BlankHandler, BlankHandler);
            l.Insert(1, "asdf");
        }

        [Test]
        public void InsertOutOfBoundsEmpty02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertOutOfBoundsEmptyHelper02);
        }

        void InsertOutOfBoundsEmptyHelper02()
        {
            var l = new ObservableList<string>(BlankHandler, BlankHandler);
            l.Insert(12333, "asdf");
        }

        [Test]
        public void RemoveAtEmpty00()
        {
            Assert.Throws<IndexOutOfRangeException>(RemoveAtEmptyHelper00);
        }

        void RemoveAtEmptyHelper00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.RemoveAt(0);
        }

        [Test]
        public void RemoveAtEmpty01()
        {
            Assert.Throws<IndexOutOfRangeException>(RemoveAtEmptyHelper01);
        }

        void RemoveAtEmptyHelper01()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.RemoveAt(-23);
        }

        [Test]
        public void RemoveAtEmpty02()
        {
            Assert.Throws<IndexOutOfRangeException>(RemoveAtEmptyHelper02);
        }

        void RemoveAtEmptyHelper02()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.RemoveAt(12333);
        }

        [Test]
        public void RemoveAtSingleItem00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(833);
            l.RemoveAt(0);
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void RemoveAtSingleItemOutOfRange00()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtSingleItemOutOfRangeHelper00);
        }

        void RemoveAtSingleItemOutOfRangeHelper00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(833);
            l.RemoveAt(-1);
        }

        [Test]
        public void RemoveAtSingleItemOutOfRange01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtSingleItemOutOfRangeHelper01);
        }

        void RemoveAtSingleItemOutOfRangeHelper01()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(833);
            l.RemoveAt(1);
        }

        [Test]
        public void RemoveAtSingleItemOutOfRange02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtSingleItemOutOfRangeHelper02);
        }

        void RemoveAtSingleItemOutOfRangeHelper02()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(833);
            l.RemoveAt(1123);
        }

        [Test]
        public void RemoveAtLotsTest()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(5000);
            l.RemoveAt(2500);
            l.RemoveAt(1000);
            l.RemoveAt(0);
            Assert.AreEqual(1, l[0]);
            Assert.AreEqual(1002, l[1000]);
            Assert.AreEqual(2503, l[2500]);
            Assert.AreEqual(4999, l[4996]);
        }

        [Test]
        public void RemoveAtLotsOutOfRange00()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtLotsOutOfRangeHelper00);
        }

        void RemoveAtLotsOutOfRangeHelper00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(2000);
            l.RemoveAt(-1);
        }

        [Test]
        public void RemoveAtLotsOutOfRange01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtLotsOutOfRangeHelper01);
        }

        void RemoveAtLotsOutOfRangeHelper01()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(2000);
            l.RemoveAt(2000);
        }

        [Test]
        public void RemoveEmpty()
        {
            var l = new ObservableList<double>(BlankHandler, BlankHandler);
            Assert.IsFalse(l.Remove(6.8));
            Assert.IsFalse(l.Remove(-6.8));
            Assert.IsFalse(l.Remove(3333.4));
            Assert.IsFalse(l.Remove(0.0));
        }

        [Test]
        public void RemoveSingleItem()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(6);
            Assert.IsFalse(l.Remove(5));
            Assert.IsTrue(l.Remove(6));
            Assert.IsFalse(l.Remove(6));
        }

        [Test]
        public void RemoveLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(3000);
            Assert.IsFalse(l.Remove(-1));
            Assert.IsFalse(l.Remove(3000));
            Assert.IsFalse(l.Remove(3044300));
            for (int i = 0; i < 3000; i++)
            Assert.IsTrue(l.Remove(i));
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void CountEmpty()
        {
            var l = new ObservableList<List<int>>(BlankHandler, BlankHandler);
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void CountSingleItem()
        {
            var l = new ObservableList<float>(BlankHandler, BlankHandler);
            l.Add(99955.4f);
            Assert.AreEqual(1, l.Count);
        }

        [Test]
        public void CountLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            for (int i = 0; i < 4000; i++)
            {
                Assert.AreEqual(i, l.Count);
                l.Add(i);
                Assert.AreEqual(i + 1, l.Count);
            }
            for (int i = 0; i < 4000; i++)
            {
                Assert.AreEqual(4000 - i, l.Count);
                l.Remove(i);
                Assert.AreEqual(3999 - i, l.Count);
            }
        }

        [Test]
        public void IndexerEmpty00()
        {
            Assert.Throws<IndexOutOfRangeException>(IndexerEmptyHelper00);
        }

        void IndexerEmptyHelper00()
        {
            var l = new ObservableList<double>(BlankHandler, BlankHandler);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[0]);
        }

        [Test]
        public void IndexerEmpty01()
        {
            Assert.Throws<IndexOutOfRangeException>(IndexerEmptyHelper01);
        }

        void IndexerEmptyHelper01()
        {
            var l = new ObservableList<double>(BlankHandler, BlankHandler);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[-34]);
        }

        [Test]
        public void IndexerEmpty02()
        {
            Assert.Throws<IndexOutOfRangeException>(IndexerEmptyHelper02);
        }

        void IndexerEmptyHelper02()
        {
            var l = new ObservableList<double>(BlankHandler, BlankHandler);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[23444]);
        }

        [Test]
        public void IndexerSingleItem()
        {
            var l = new ObservableList<string>(BlankHandler, BlankHandler);
            l.Add("hello");
            Assert.AreEqual("hello", l[0]);
        }

        [Test]
        public void IndexerSingleItemOutOfBounds00()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerSingleItemOutOfBoundsHelper00);
        }

        void IndexerSingleItemOutOfBoundsHelper00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(8);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[-1]);
        }

        [Test]
        public void IndexerSingleItemOutOfBounds01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerSingleItemOutOfBoundsHelper01);
        }

        void IndexerSingleItemOutOfBoundsHelper01()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(8);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[1]);
        }

        [Test]
        public void IndexerSingleItemOutOfBounds02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerSingleItemOutOfBoundsHelper02);
        }

        void IndexerSingleItemOutOfBoundsHelper02()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(8);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[3332]);
        }

        [Test]
        public void IndexerLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(4000);
            for (int i = 0; i < 4000; i++)
            Assert.AreEqual(i, l[i]);
        }

        [Test]
        public void IndexerLotsOutOfBounds00()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerLotsOutOfBoundsHelper00);
        }

        void IndexerLotsOutOfBoundsHelper00()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(4000);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[-1]);
        }

        [Test]
        public void IndexerLotsOutOfBounds01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerLotsOutOfBoundsHelper01);
        }

        void IndexerLotsOutOfBoundsHelper01()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(4000);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[4000]);
        }

        [Test]
        public void IndexerLotsOutOfBounds02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerLotsOutOfBoundsHelper02);
        }

        void IndexerLotsOutOfBoundsHelper02()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(4000);
            // The indexer should fail, but we put the result in an Add
            // call to avoid compiler warnings for not using the value
            // otherwise
            l.Add(l[4433333]);
        }

        [Test]
        public void EnumeratorEmpty()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            var e = l.GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void EnumeratorSingleItem()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Add(8);
            var e = l.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(8, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void EnumeratorLots()
        {
            var l = new ObservableList<int>(BlankHandler, BlankHandler);
            l.Fill(4000);
            var e = l.GetEnumerator();
            int count = 0;
            while (e.MoveNext())
            {
                Assert.AreEqual(count, e.Current);
                count++;
            }
            Assert.AreEqual(4000, count);
        }

        class AddedRemovedHelper<T>
        {
            public int AddedCount { get; private set; }
            public int RemovedCount { get; private set; }

            public T LastAdded { get; private set; }
            public T LastRemoved { get; private set; }

            public void Added(T item)
            {
                AddedCount = AddedCount + 1;
                LastAdded = item;
            }

            public void Removed(T item)
            {
                RemovedCount = RemovedCount + 1;
                LastRemoved = item;
            }
        }

        [Test]
        public void AddedRemovedEmpty()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, h.Removed);
            Assert.AreEqual(0, h.AddedCount);
            Assert.AreEqual(0, h.RemovedCount);
            // Test is done; this is to avoid compiler warnings about not
            // using l
            l.Add(6);
        }

        [Test]
        public void AddedRemovedClearEmpty()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, h.Removed);
            l.Clear();
            Assert.AreEqual(0, h.AddedCount);
            Assert.AreEqual(0, h.RemovedCount);
        }

        [Test]
        public void AddedRemovedClearSingleItem()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(BlankHandler, h.Removed);
            l.Add(45656);
            l.Clear();
            Assert.AreEqual(1, h.RemovedCount);
            Assert.AreEqual(45656, h.LastRemoved);
        }

        [Test]
        public void AddedRemovedClearLots()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(BlankHandler, h.Removed);
            l.Fill(3000);
            l.Clear();
            Assert.AreEqual(3000, h.RemovedCount);
            Assert.AreEqual(2999, h.LastRemoved);
        }

        [Test]
        public void AddedAddSingleItem()
        {
            var h = new AddedRemovedHelper<string>();
            var l = new ObservableList<string>(h.Added, BlankHandler);
            l.Add("abcde");
            Assert.AreEqual(1, h.AddedCount);
            Assert.AreEqual("abcde", h.LastAdded);
        }

        [Test]
        public void AddedAddLots()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, BlankHandler);
            l.Fill(3000);
            Assert.AreEqual(3000, h.AddedCount);
            Assert.AreEqual(2999, h.LastAdded);
        }

        [Test]
        public void AddedInsertSingleItem()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, BlankHandler);
            l.Insert(0, 64);
            Assert.AreEqual(1, h.AddedCount);
            Assert.AreEqual(64, h.LastAdded);
        }

        [Test]
        public void AddedInsertLots01()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, BlankHandler);
            for (int i = 0; i < 3000; i++)
            l.Insert(i, i);
            Assert.AreEqual(3000, h.AddedCount);
            Assert.AreEqual(2999, h.LastAdded);
        }

        [Test]
        public void AddedInsertLots02()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(h.Added, BlankHandler);
            for (int i = 0; i < 3000; i++)
            l.Insert(0, i);
            l.Insert(1500, 86);
            Assert.AreEqual(3001, h.AddedCount);
            Assert.AreEqual(86, h.LastAdded);
        }

        [Test]
        public void RemovedSingleItem()
        {
            var h = new AddedRemovedHelper<string>();
            var l = new ObservableList<string>(BlankHandler, h.Removed);
            l.Add("cats");
            l.Remove("cats");
            Assert.AreEqual(1, h.RemovedCount);
            Assert.AreEqual("cats", h.LastRemoved);
        }

        [Test]
        public void RemovedLots()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(BlankHandler, h.Removed);
            l.Fill(2000);
            for (int i = 0; i < 2000; i++)
            l.Remove(i);
            Assert.AreEqual(2000, h.RemovedCount);
            Assert.AreEqual(1999, h.LastRemoved);
        }

        [Test]
        public void RemovedAtSingleItem()
        {
            var h = new AddedRemovedHelper<string>();
            var l = new ObservableList<string>(BlankHandler, h.Removed);
            l.Add("cats");
            l.RemoveAt(0);
            Assert.AreEqual(1, h.RemovedCount);
            Assert.AreEqual("cats", h.LastRemoved);
        }

        [Test]
        public void RemovedAtLots()
        {
            var h = new AddedRemovedHelper<int>();
            var l = new ObservableList<int>(BlankHandler, h.Removed);
            l.Fill(2000);
            for (int i = 0; i < 2000; i++)
            l.RemoveAt(1999 - i);
            Assert.AreEqual(2000, h.RemovedCount);
            Assert.AreEqual(0, h.LastRemoved);
        }
    }
}
