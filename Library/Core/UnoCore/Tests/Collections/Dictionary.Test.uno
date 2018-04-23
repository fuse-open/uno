using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    static class DictionaryTestHelpers
    {
        public static void Fill(this IDictionary<int, double> d, int count)
        {
            for (int i = 0; i < count; i++)
                d.Add(i, (double)i);
        }

        public static void FillNegative(this IDictionary<int, double> d, int count)
        {
            for (int i = 0; i < count; i++)
                d.Add(i, -(double)i);
        }

        public static void FillWithIndexer(this IDictionary<int, double> d, int count)
        {
            for (int i = 0; i < count; i++)
                d[i] = (double)i;
        }
    }

    public class DictionaryTest
    {
        [Test]
        public void Instantiation()
        {
            var d = new Dictionary<string, int>();
            Assert.AreNotEqual(null, d);
        }

        [Test]
        public void CopyEmptyDictionary()
        {
            var d = new Dictionary<string, string>();
            var d2 = new Dictionary<string, string>(d);
            Assert.AreEqual(0, d2.Count);
        }

        [Test]
        public void CopyFullDictionary()
        {
            var d = new Dictionary<int, double>();
            d.Fill(700);
            var d2 = new Dictionary<int, double>(d);
            Assert.AreEqual(700, d2.Count);
            int count = 0;
            foreach (var x in d2)
            {
                Assert.AreEqual(count, x.Key);
                Assert.AreEqual((double)count, x.Value);
                count++;
            }
        }

        [Test]
        public void EnumeratorEmpty()
        {
            var d = new Dictionary<int, int>();
            var e = d.GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void EnumeratorSingleItem()
        {
            var d = new Dictionary<int, int>();
            d.Add(4, 5);
            var e = d.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void EnumeratorLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            var e = d.GetEnumerator();
            int count = 0;
            while (e.MoveNext())
            {
                var c = e.Current;
                Assert.AreEqual(count, c.Key);
                Assert.AreEqual((double)count, c.Value);
                count++;
            }
            Assert.AreEqual(2000, count);
        }

        [Test]
        public void EnumeratorLotsForeach()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            int count = 0;
            foreach (var c in d)
            {
                Assert.AreEqual(count, c.Key);
                Assert.AreEqual((double)count, c.Value);
                count++;
            }
            Assert.AreEqual(2000, count);
        }

        [Test]
        public void KeysEmpty()
        {
            var d = new Dictionary<string, bool>();
            var k = d.Keys;
            var e = k.GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void KeysSingleItem()
        {
            var d = new Dictionary<int, double>();
            d.Add(234234, 23.06898);
            var k = d.Keys;
            var e = k.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void KeysLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            var k = d.Keys;
            var e = k.GetEnumerator();
            int count = 0;
            while (e.MoveNext())
            {
                var c = e.Current;
                Assert.AreEqual(count, c);
                count++;
            }
            Assert.AreEqual(3000, count);
        }

        [Test]
        public void KeysLotsForeach()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            var k = d.Keys;
            int count = 0;
            foreach (var c in k)
            {
                Assert.AreEqual(count, c);
                count++;
            }
            Assert.AreEqual(3000, count);
        }

        [Test]
        public void ValuesEmpty()
        {
            var d = new Dictionary<string, bool>();
            var k = d.Values;
            var e = k.GetEnumerator();
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void ValuesSingleItem()
        {
            var d = new Dictionary<int, double>();
            d.Add(234234, 23.06898);
            var k = d.Values;
            var e = k.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void ValuesLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            var k = d.Values;
            var e = k.GetEnumerator();
            int count = 0;
            while (e.MoveNext())
            {
                var c = e.Current;
                Assert.AreEqual((double)count, c);
                count++;
            }
            Assert.AreEqual(3000, count);
        }

        [Test]
        public void ValuesLotsForeach()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            var k = d.Values;
            int count = 0;
            foreach (var c in k)
            {
                Assert.AreEqual((double)count, c);
                count++;
            }
            Assert.AreEqual(3000, count);
        }

        [Test]
        public void CountEmpty()
        {
            var d = new Dictionary<int, int>();
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void CountLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(1234);
            Assert.AreEqual(1234, d.Count);
        }

        [Test]
        public void ClearEmpty()
        {
            var d = new Dictionary<string, string>();
            d.Clear();
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void ClearSingleItem()
        {
            var d = new Dictionary<string, string>();
            d.Add("a", "alskjdfkldjsfkajsd");
            Assert.AreEqual(1, d.Count);
            d.Clear();
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void ClearSingleItemAndIndex()
        {
            var d = new Dictionary<int, string>();
            d.Add(-12, "some string");
            Assert.AreEqual("some string", d[-12]);
            d.Clear();
            Assert.Throws(new ClearSingleItemAndIndexHelper(d).Failure);
        }

        class ClearSingleItemAndIndexHelper
        {
            readonly IDictionary<int, string> _d;

            public ClearSingleItemAndIndexHelper(IDictionary<int, string> d)
            {
                _d = d;
            }

            public void Failure()
            {
                // The indexer is what should break after
                // clearing the collection; we stick it in
                // an Add call to avoid compiler warnings
                // because of the unused value otherwise
                _d.Add(6, _d[-12]);
            }
        }

        [Test]
        public void AddTwoItems()
        {
            var d = new Dictionary<int, int>();
            d.Add(12, 13);
            d.Add(-1, 3);
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual(13, d[12]);
            Assert.AreEqual(3, d[-1]);
        }

        [Test]
        public void AddThreeItems()
        {
            var d = new Dictionary<string, string>();
            d.Add("a", "d");
            d.Add("b", "e");
            d.Add("c", "f");
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual("d", d["a"]);
            Assert.AreEqual("e", d["b"]);
            Assert.AreEqual("f", d["c"]);
        }

        [Test]
        public void AddSameValues()
        {
            Assert.Throws(AddSameValuesHelper00);
            Assert.Throws(AddSameValuesHelper01);
        }

        void AddSameValuesHelper00()
        {
            var d = new Dictionary<int, int>();
            d.Add(1, 2);
            d.Add(1, 3);
        }

        void AddSameValuesHelper01()
        {
            var d = new Dictionary<int, int>();
            d.Add(1, 2);
            d.Add(2, 4);
            d.Add(1, 3);
        }

        [Test]
        public void AddLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            Assert.AreEqual(2000, d.Count);
        }

        [Test]
        public void TryGetEmpty()
        {
            var d = new Dictionary<string, string>();
            string o;
            Assert.IsFalse(d.TryGetValue("some key", out o));
            Assert.AreEqual(null, o);
        }

        [Test]
        public void TryGetSingleItem()
        {
            var d = new Dictionary<int, double>();
            d.Add(5, 6.0);
            double o;
            Assert.IsTrue(d.TryGetValue(5, out o));
            Assert.AreEqual(6.0, o);
        }

        [Test]
        public void TryGetLotsValid()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            double o;
            Assert.IsTrue(d.TryGetValue(1000, out o));
            Assert.AreEqual(1000.0, o);
        }

        [Test]
        public void RemoveEmpty()
        {
            var d = new Dictionary<string, int>();
            Assert.IsFalse(d.Remove("not in there"));
        }

        [Test]
        public void RemoveSingleItem()
        {
            var d = new Dictionary<string, int>();
            d.Add("a key", 42);
            Assert.IsTrue(d.Remove("a key"));
            Assert.IsFalse(d.Remove("a key"));
        }

        [Test]
        public void RemoveLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            Assert.AreEqual(2000, d.Count);
            for (int i = 0; i < 2000; i++)
                Assert.IsTrue(d.Remove(i));
            Assert.IsFalse(d.Remove(1000));
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void ContainsKeyEmpty()
        {
            var d = new Dictionary<string, bool>();
            Assert.IsFalse(d.ContainsKey("nope"));
        }

        [Test]
        public void ContainsKeyLotsInvalid()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            Assert.IsFalse(d.ContainsKey(-1));
            Assert.IsFalse(d.ContainsKey(3000));
        }

        [Test]
        public void ContainsKeyLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(3000);
            for (int i = 0; i < 3000; i++)
                Assert.IsTrue(d.ContainsKey(i));
        }

        [Test]
        public void IndexerEmpty()
        {
            Assert.Throws(IndexerEmptyHelper00);
            Assert.Throws(IndexerEmptyHelper01);
        }

        void IndexerEmptyHelper00()
        {
            var d = new Dictionary<bool, int>();
            // The indexer is what should break here;
            // we stick it in an Add call to avoid compiler
            // warnings because of the unused value otherwise
            d.Add(false, d[false]);
        }

        void IndexerEmptyHelper01()
        {
            var d = new Dictionary<bool, int>();
            // The indexer is what should break here;
            // we stick it in an Add call to avoid compiler
            // warnings because of the unused value otherwise
            d.Add(false, d[true]);
        }

        [Test]
        public void IndexerLots()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            for (int i = 0; i < 2000; i++)
                Assert.AreEqual((double)i, d[i]);
        }

        [Test]
        public void IndexerLotsInvalid()
        {
            Assert.Throws(IndexerLotsInvalidHelper00);
            Assert.Throws(IndexerLotsInvalidHelper01);
        }

        void IndexerLotsInvalidHelper00()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            // The indexer is what should break here;
            // we stick it in an Add call to avoid compiler
            // warnings because of the unused value otherwise
            d.Add(2000, d[-1]);
        }

        void IndexerLotsInvalidHelper01()
        {
            var d = new Dictionary<int, double>();
            d.Fill(2000);
            // The indexer is what should break here;
            // we stick it in an Add call to avoid compiler
            // warnings because of the unused value otherwise
            d.Add(5, d[2000]);
        }

        [Test]
        public void IndexerOverwriteLots()
        {
            var d = new Dictionary<int, double>();
            d.FillNegative(2000);
            Assert.AreEqual(2000, d.Count);
            for (int i = 0; i < 2000; i++)
                Assert.AreEqual(-(double)i, d[i]);

            d.FillWithIndexer(2000);
            Assert.AreEqual(2000, d.Count);
            for (int i = 0; i < 2000; i++)
                Assert.AreEqual((double)i, d[i]);
        }
    }
}
