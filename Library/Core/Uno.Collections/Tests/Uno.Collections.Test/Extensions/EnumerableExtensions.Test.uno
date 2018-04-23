using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Extensions.Test
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void OrderByForObject()
        {
            int index = 0;
            var orderedVlues = new int[] { -23, 1, 4, 7, 11 };

            var elems = new DummyItem[] { new DummyItem(1), new DummyItem(11), new DummyItem(7), new DummyItem(4), new DummyItem(-23) }.AsEnumerable();
            var orderedItems = elems.OrderBy(new OrderByExpressions().CompareDummyItem);

            foreach (var item in orderedItems)
            {
                Assert.AreEqual(orderedVlues[index], item.Value);
                index++;
            }
        }

        [Test]
        public void OrderByForObject_2()
        {
            int index = 0;
            var orderedVlues = new int[] { -23, 1, 4, 7, 11 };

            var elems = new DummyItem[] { new DummyItem(1), new DummyItem(11), new DummyItem(7), new DummyItem(4), new DummyItem(-23) }.AsEnumerable();
            var orderedItems = elems.OrderBy(new OrderByExpressions().CompareDummyItemEx);

            foreach (var item in orderedItems)
            {
                Assert.AreEqual(orderedVlues[index], item.Value);
                index++;
            }
        }

        [Test]
        public void OrderByForInt()
        {
            int index = 0;
            var orderedVlues = new int[] { -23, 1, 4, 7, 11 };

            var elems = new int[] { 1, 11, 7, 4, -23 }.AsEnumerable();
            var orderedItems = elems.OrderBy(new OrderByExpressions().CompareInt);

            foreach (var item in orderedItems)
            {
                Assert.AreEqual(orderedVlues[index], item);
                index++;
            }
        }

        [Test]
        public void OrderByForInt_2()
        {
            int index = 0;
            var orderedVlues = new int[] { -23, 1, 4, 7, 11 };

            var elems = new int[] { 1, 11, 7, 4, -23 }.AsEnumerable();
            var orderedItems = elems.OrderBy(new OrderByExpressions().CompareIntEx);

            foreach (var item in orderedItems)
            {
                Assert.AreEqual(orderedVlues[index], item);
                index++;
            }
        }

        [Test]
        public void Union()
        {
            var elems1 = new int[] { 1, 11, 7 }.AsEnumerable();
            var elems2 = new int[] { 4, -23 }.AsEnumerable();
            var elems = elems1.Union(elems2);
            Assert.AreEqual(new int[] { 1, 11, 7, 4, -23 }, elems.ToArray());
        }

        [Test]
        public void WhereForInt()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            var res = elems.Where(new MatchExpressions() { Integer = 12 }.MatchInt).ToList();

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(12, res[0]);
            Assert.AreEqual(12, res[1]);
        }

        [Test]
        public void WhereForObject()
        {
            var elems = new DummyItem[] { new DummyItem("name1"), new DummyItem("name2"), new DummyItem("name3"), new DummyItem("name3"), new DummyItem("name2") }.AsEnumerable();
            var res = elems.Where(new MatchExpressions() { String = "name2" }.MatchDummyItemName).ToList();

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("name2", res[0].Name);
            Assert.AreEqual("name2", res[1].Name);
        }

        [Test]
        public void IndexOf()
        {
            var arr = new DummyItem[] { new DummyItem("name1"), new DummyItem("name2"), new DummyItem("name3"), new DummyItem("name3"), new DummyItem("name2") };
            var elems = arr.AsEnumerable();
            Assert.AreEqual(2, elems.IndexOf(arr[2]));
            Assert.AreEqual(-1, elems.IndexOf(null));
        }

        [Test]
        public void Contains()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            Assert.IsTrue(elems.Contains(12));
            Assert.IsFalse(elems.Contains(150));
        }

        [Test]
        public void Count()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            Assert.AreEqual(6, elems.Count());
        }

        [Test]
        public void Last()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 17 }.AsEnumerable();
            Assert.AreEqual(17, elems.Last());
        }

        [Test]
        public void LastOrDefault()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 17 }.AsEnumerable();
            Assert.AreEqual(17, elems.LastOrDefault());

            elems = new int[0].AsEnumerable();
            Assert.AreEqual(0, elems.LastOrDefault());
        }

        [Test]
        public void First()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 17 }.AsEnumerable();
            Assert.AreEqual(12, elems.First());
        }

        [Test]
        public void FirstOrDefault()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 17 }.AsEnumerable();
            Assert.AreEqual(12, elems.FirstOrDefault());

            elems = new int[0].AsEnumerable();
            Assert.AreEqual(0, elems.FirstOrDefault());
        }

        [Test]
        public void FirstOrDefaultWithPredicate()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 17 }.AsEnumerable();
            Assert.AreEqual(3, elems.FirstOrDefault(new MatchExpressions() { Integer = 3 }.MatchInt));

            elems = new int[0].AsEnumerable();
            Assert.AreEqual(0, elems.FirstOrDefault(new MatchExpressions() { Integer = 33 }.MatchInt));
        }

        [Test]
        public void Any()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            Assert.IsTrue(elems.Any());

            elems = new int[0].AsEnumerable();
            Assert.IsFalse(elems.Any());
        }

        [Test]
        public void AnyWithPredicate()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            Assert.IsTrue(elems.Any(new MatchExpressions() { Integer = 3 }.MatchInt));
            Assert.IsFalse(elems.Any(new MatchExpressions() { Integer = 33 }.MatchInt));
        }

        [Test]
        public void All()
        {
            {
                var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
                Assert.IsFalse(elems.All(new MatchExpressions() { Integer = 3 }.MatchInt));
            }
            {
                var elems = new int[] { 12, 12, 12, 12, 12, 12 }.AsEnumerable();
                Assert.IsTrue(elems.All(new MatchExpressions() { Integer = 12 }.MatchInt));
            }
            {
                var elems = new int[0].AsEnumerable();
                Assert.IsTrue(elems.All(new MatchExpressions().MatchInt));
            }
        }

        [Test]
        public void Distinct()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12, 7 }.AsEnumerable();
            Assert.AreEqual(new int[] { 12, 22, 3, 7, 6 }, elems.Distinct().ToArray());
        }

        [Test]
        public void Skip()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            var res = elems.Skip(2);
            Assert.AreEqual(4, res.Count());
            Assert.AreEqual(new int[] { 3, 7, 6, 12 }, res.ToArray());
        }

        [Test]
        public void Take()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            var res = elems.Take(4);
            Assert.AreEqual(4, res.Count());
            Assert.AreEqual(new int[] { 12, 22, 3, 7 }, res.ToArray());
        }

        [Test]
        public void SkipTake()
        {
            var elems = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
            var res = elems.Skip(2).Take(2);
            Assert.AreEqual(2, res.Count());
            Assert.AreEqual(new int[] { 3, 7 }, res.ToArray());
        }

        [Test]
        public void SequenceEqual()
        {
            {
                var elems1 = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
                var elems2 = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
                var elems3 = new int[] { 12, 22, 3, 123, 6, 12 }.AsEnumerable();
                Assert.IsTrue(elems1.SequenceEqual(elems1));
                Assert.IsTrue(elems1.SequenceEqual(elems2));
                Assert.IsFalse(elems1.SequenceEqual(elems3));
                Assert.IsFalse(elems3.SequenceEqual(elems2));
            }
            {
                var elems1 = new int[] { 12, 22, 3, 7, 6, 12 }.AsEnumerable();
                var elems2 = new int[] { 12, 22, 3, 7, 6 }.AsEnumerable();
                Assert.IsFalse(elems1.SequenceEqual(elems2));
                Assert.IsFalse(elems2.SequenceEqual(elems1));
            }
        }
    }
}
