using Uno;
using Uno.Collections;
using Uno.Testing;


namespace Collections.Test
{
    public class HashSetTest
    {
        public class DummyClass
        {
            string _f;
            string _b;
            int _l;
            public DummyClass(string f, string b, int l)
            {
            }
        }

        public HashSet<int> CreateDummyHashSet(int n)
        {
            var h = new HashSet<int>();
            for (int i = 0; i < n; i++)
                h.Add(i);
            return h;
        }

        static string alph = "abcdefghijklmnopqrstuvwxyzæøå";
        public HashSet<DummyClass> CreateHashSetRandomDummyClass(int n)
        {
            var h = new HashSet<DummyClass>();
            var r = new Random(3123);
            for (int i = 0; i < n; i++)
            {
                var l = r.Next();
                var nf = r.Next(0, 100);
                var nb = r.Next(0, 100);
                string f = "";
                string b = "";
                for (int j = 0; j < nf; j++)
                    f += alph[r.Next(0,28)];
                for (int j = 0; j < nb; j++)
                    b += alph[r.Next(0,28)];
                h.Add(new DummyClass(f,b,l));
            }
            return h;
        }

        [Test]
        public void ConstructionFromListTest()
        {
            var l = new List<int>(){1,2,3,4,5};
            var h = new HashSet<int>(l);
            Assert.AreCollectionsEqual(l, h);
        }

        [Test]
        public void HashSetEqualsHashSetTrueTest()
        {
            var h1 = CreateDummyHashSet(10);
            var h2 = CreateDummyHashSet(10);
            Assert.IsTrue(h1.SetEquals(h2));
        }

        [Test]
        public void HashSetEqualsHashSetFalseTest()
        {
            var h1 = CreateDummyHashSet(10);
            var h2 = CreateDummyHashSet(5);
            Assert.IsFalse(h1.SetEquals(h2));
        }

        [Test]
        public void HashSetEqualsListTrueTest()
        {
            var h1 = CreateDummyHashSet(10);
            var l1 = new List<int>(){0,1,2,3,4,5,6,7,8,9};
            Assert.IsTrue(h1.SetEquals(l1));

        }

        [Test]
        public void HashSetEqualsListFalseTest()
        {
            var h1 = CreateDummyHashSet(10);
            var l1 = new List<int>(){0,1,2,3,42,52,6,7,8,9};
            Assert.IsFalse(h1.SetEquals(l1));
        }

        [Test]
        public void AddTest()
        {
            var h = new HashSet<int>();
            Assert.AreEqual(0, h.Count);
            h.Add(10);
            Assert.IsTrue(h.Contains(10));

            h.Add(10);
            Assert.AreEqual(1, h.Count);

            h.Add(30);
            Assert.AreEqual(2, h.Count);
        }

        [Test]
        public void ContainsIntTest()
        {
            var h = CreateDummyHashSet(5);
            Assert.IsTrue(h.Contains(0));
            Assert.IsTrue(h.Contains(1));
            Assert.IsTrue(h.Contains(2));
            Assert.IsTrue(h.Contains(3));
            Assert.IsTrue(h.Contains(4));

            Assert.IsFalse(h.Contains(5));
            Assert.IsFalse(h.Contains(-1));
        }

        [Test]
        public void ContainsDummyClassTest()
        {
            var dc = new DummyClass("3213492", "dfosidj", 23);
            var h = CreateHashSetRandomDummyClass(30);
            Assert.IsFalse(h.Contains(dc));
            h.Add(dc);
            Assert.IsTrue(h.Contains(dc));
        }

        [Test]
        public void RemoveValidItemsTest()
        {
            var h = CreateDummyHashSet(10);
            h.Remove(5);
            Assert.AreEqual(9, h.Count);
            Assert.IsFalse(h.Contains(5));
        }

        [Test]
        public void RemoveInvalidItemsTest()
        {
            Assert.DoesNotThrowAny(RemoveSameItemTwiceAction);
        }

        public void RemoveSameItemTwiceAction()
        {
            var h = CreateDummyHashSet(10);
            h.Remove(5);
            h.Remove(5);
        }

        [Test]
        public void CountTest()
        {
            var h = CreateDummyHashSet(50);
            Assert.AreEqual(50, h.Count);
            h.Remove(10);
            h.Remove(30);
            h.Remove(35);
            Assert.AreEqual(47, h.Count);
        }

        [Test]
        public void ClearTest()
        {
            var h = CreateDummyHashSet(10);
            h.Clear();
            Assert.AreEqual(0, h.Count);
        }

    }
}
