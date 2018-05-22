using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public class ListTest
    {
        public List<int> CreateDummyList(int nItems)
        {
            var ret = new List<int>();
            for (int i = 0; i < nItems; i++)
                ret.Add(i);
            return ret;
        }

        //IList members

        [Test]
        public void Clear()
        {
            var l = CreateDummyList(15);
            Assert.AreEqual(l.Count, 15);
            l.Clear();
            Assert.AreEqual(l.Count, 0);
        }

        [Test]
        public void ClearMany()
        {
            var l = CreateDummyList(5000);
            Assert.AreEqual(l.Count, 5000);
            l.Clear();
            Assert.AreEqual(l.Count, 0);
        }

        [Test]
        public void Add()
        {
            var l = new List<int>();
            Assert.AreEqual(l.Count, 0);
            l.Add(6);
            l.Add(7);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(6, l[0]);
            Assert.AreEqual(7, l[1]);
        }

        [Test]
        public void AddMany()
        {
            var l = new List<int>();
            Assert.AreEqual(l.Count, 0);
            for (int i = 0; i < 5000; i++)
            {
                l.Add(i);
                Assert.AreEqual(i + 1, l.Count);
            }
        }

        [Test]
        public void InsertUsingCorrectIndex()
        {
            var l = CreateDummyList(5);
            l.Insert(2, 3);
            Assert.AreEqual(3, l[2]);
            Assert.DoesNotThrowAny(InsertWithCorrectIndex);
        }

        [Test]
        public void InsertUsingCorrectIndexMany()
        {
            var l = CreateDummyList(5000);
            l.Insert(2, 3);
            Assert.AreEqual(3, l[2]);
            Assert.DoesNotThrowAny(InsertWithCorrectIndexMany);
        }

        public void InsertWithCorrectIndex()
        {
            var l = CreateDummyList(5);
            l.Insert(2, 3);
        }

        public void InsertWithCorrectIndexMany()
        {
            var l = CreateDummyList(3000);
            for (int i = 0; i < 1000; i++)
            {
                l.Insert(i, i);
                Assert.AreEqual(3000 + i + 1, l.Count);
            }
        }

        [Test]
        public void InsertUsingWrongIndex00()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertWithWrongIndexAction00);
        }

        public void InsertWithWrongIndexAction00()
        {
            InsertWithWrongIndex(10);
        }

        public void InsertWithWrongIndex(int index)
        {
            var l = CreateDummyList(3);
            l.Insert(index, 10);
        }

        [Test]
        public void InsertUsingWrongIndex01()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertWithWrongIndexAction01);
        }

        public void InsertWithWrongIndexAction01()
        {
            InsertWithWrongIndex(-1);
        }

        [Test]
        public void InsertUsingWrongIndex02()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertWithWrongIndexAction02);
        }

        public void InsertWithWrongIndexAction02()
        {
            InsertWithWrongIndex(int.MinValue);
        }

        [Test]
        public void InsertUsingWrongIndex03()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertWithWrongIndexAction03);
        }

        public void InsertWithWrongIndexAction03()
        {
            InsertWithWrongIndex(int.MaxValue);
        }

        [Test]
        public void InsertAtTheEndOfList()
        {

            var l = CreateDummyList(5);
            l.Insert(l.Count, 10);
            Assert.AreEqual(10, l[l.Count - 1]);
            Assert.DoesNotThrowAny(InsertAtTheEndOFList);
        }

        public void InsertAtTheEndOFList()
        {
            var l = CreateDummyList(5);
            l.Insert(l.Count, 10);
        }

        [Test]
        public void InsertAtTheEndOfListMany()
        {
            var l = new List<int>();
            for (int i = 0; i < 5000; i++)
            {
                l.Insert(i, i);
            }
            Assert.AreEqual(5000, l.Count);
        }

        [Test]
        public void RemoveFromListWithOneOccurrence()
        {
            var l1 = CreateDummyList(5);

            l1.Remove(3);
            Assert.AreEqual(4, l1.Count);
            Assert.IsFalse(l1.Contains(3));

            var l2 = CreateDummyList(5);

            l2.Remove(3);
            Assert.AreCollectionsEqual(l1, l2);
        }

        [Test]
        public void RemoveFromListWithMoreThanOneOccurence()
        {
            var l = CreateDummyList(5);
            l.Add(3);
            l.Remove(3);
            Assert.IsTrue(l.Contains(3));
            Assert.AreEqual(l.IndexOf(3), 4);
        }

        [Test]
        public void RemoveAtWithCorrectIndex()
        {
            var l = CreateDummyList(5);
            l.RemoveAt(4);
            Assert.IsFalse(l.Contains(4));
            Assert.AreEqual(4, l.Count);
        }

        [Test]
        public void RemoveAtWithWrongIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(RemoveAtWithWrongIndexAction);
        }

        public void RemoveAtWithWrongIndexAction()
        {
            var l = CreateDummyList(5);
            l.RemoveAt(-1);
            l.RemoveAt(int.MaxValue);
            l.RemoveAt(int.MinValue);
        }

        [Test]
        public void Count()
        {
            var l1 = CreateDummyList(100);
            Assert.AreEqual(100, l1.Count);
            for (int i = 0; i < 10; i++)
                l1.Remove(i);
            Assert.AreEqual(90, l1.Count);

            var l2 = new List<int>();
            Assert.AreEqual(0, l2.Count);
            for (int i = 0; i < 10; i++)
                l2.Add(i);
            Assert.AreEqual(10, l2.Count);

            var l3 = new List<int>(50);
            Assert.AreEqual(0, l3.Count);

        }

        [Test]
        public void CountMany()
        {
            var l = CreateDummyList(5000);
            Assert.AreEqual(5000, l.Count);
            var l1 = new List<int>();
            for (int i = 0; i < 5000; i++)
            {
                l1.Add(i);
                Assert.AreEqual(i + 1, l1.Count);
            }
        }

        [Test]
        public void ToArray()
        {
            var l = CreateDummyList(10);
            var a = l.ToArray();
            Assert.AreEqual(10, a.Length);
            Assert.AreCollectionsEqual(a, l);

            var l2 = CreateDummyList(0);
            var a2 = l2.ToArray();
            Assert.AreCollectionsEqual(l2,a2);
        }

        [Test]
        public void AddRange()
        {
            var l1 = CreateDummyList(10);
            var l2 = CreateDummyList(10);

            l1.AddRange(l2);

            Assert.AreEqual(20, l1.Count);
            Assert.AreEqual(10, l2.Count);
        }

        [Test]
        public void AddRangeMany()
        {
            var l1 = CreateDummyList(1000);
            var l2 = CreateDummyList(10000);
            Assert.AreEqual(1000, l1.Count);
            Assert.AreEqual(10000, l2.Count);
            l1.AddRange(l2);
            Assert.AreEqual(11000, l1.Count);
        }

        [Test]
        public void IndexOf()
        {
            var l = CreateDummyList(10);
            Assert.AreEqual(1, l.IndexOf(1));
            Assert.AreEqual(5, l.IndexOf(5));
            Assert.AreEqual( -1, l.IndexOf(100));
            Assert.AreEqual(-1, l.IndexOf(-2));

            l.Add(5);
            l.Add(5);
            Assert.AreNotEqual(10, l.IndexOf(5));
            Assert.AreNotEqual(11, l.IndexOf(5));
        }

        [Test]
        public void Contains()
        {
            var l = CreateDummyList(10);
            Assert.IsTrue(l.Contains(5));
            Assert.IsTrue(l.Contains(9));
            Assert.IsFalse(l.Contains(10));
            Assert.IsFalse(l.Contains(-1));
        }

        [Test]
        public void ContainsMany()
        {
            var l = CreateDummyList(5000);
            for (int i = 0; i < 5000; i++)
            {
                Assert.IsTrue(l.Contains(i));
            }
        }

        [Test]
        public void IndexerOperatorValidIndex()
        {
            var l = CreateDummyList(1000);
            Assert.AreEqual(10, l[10]);
            Assert.AreEqual(500, l[500]);
        }

        [Test]
        public void IndexerOperatorInvalidIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexerOperatorInvalidAction);
        }

        public void IndexerOperatorInvalidAction()
        {
            var l = CreateDummyList(10);
            l[-100] = 10;
            var item = l[100];
        }

        [Test]
        public void IndexerOperatorAssignment()
        {

            var l = CreateDummyList(10);
            l[5] = 10;
            Assert.AreEqual(10, l[5]);
        }

        [Test]
        public void SortTest1()
        {
            var l1 = new List<int>(){5, 7, 2, 18, 54, 13};
            var l2 = new List<int>(){2, 18, 5, 13, 7, 54};
            var corr = new List<int>(){2, 5, 7, 13, 18, 54 };
            l1.Sort(Comparison);
            l2.Sort(Comparison);
            Assert.AreCollectionsEqual(l1, l2);
            Assert.AreCollectionsEqual(l1, corr);
        }

        [Test]
        public void SortTest2()
        {
            var l1 = new List<int>(){5, 7, 2, 18, 54, 13, 18};
            var l2 = new List<int>(){2, 18, 5, 13, 7, 54, 18};
            var corr = new List<int>(){2, 5, 7, 13, 18, 18, 54 };
            l1.Sort(Comparison);
            l2.Sort(Comparison);
            Assert.AreCollectionsEqual(l1, l2);
            Assert.AreCollectionsEqual(l1, corr);
        }

        [Test]
        public void SortMany()
        {
            var l = new List<int>();
            var r = new Random(1231);
            for (int i = 0; i < 5000; i++)
            {
                l.Add(r.Next());
            }
            l.Sort(Comparison);
            Assert.AreEqual(5000, l.Count);
            for (int i = 1; i < 5000; i++)
            {
                Assert.IsTrue(l[i] >= l[i-1]);
            }
        }

        [Test]
        public void SortManyEqualData()
        {
            var l = new List<int>();
            var r = new Random(1231);
            var randData = new List<int>();

            for (int i = 0; i < 100; i++)
            {
                randData.Add(r.Next());
            }

            for(int i = 0;i < 5000;++i)
            {
                l.Add(randData[i/50]);
            }

            l.Sort(Comparison);
            Assert.AreEqual(5000, l.Count);
            for (int i = 1; i < 5000; i++)
            {
                Assert.IsTrue(l[i] >= l[i-1]);
            }
        }

        public int Comparison(int i1, int i2)
        {
            return i1 - i2;
        }

        [Test]
        public void Capacity()
        {
            for (int n = 0; n < 10; ++n)
            {
                var lists = new List<int>[n];
                for (int i = 0; i < n; ++i)
                {
                    lists[i] = new List<int>(i);
                    for (int j = 0; j < n; ++j)
                        lists[i].Add(j);
                }
                for (int i = 0; i < n - 1; ++i)
                {
                    Assert.AreCollectionsEqual(lists[i], lists[i + 1]);
                }
            }
        }
    }
}
