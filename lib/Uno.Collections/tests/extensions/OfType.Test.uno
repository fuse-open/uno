using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Extensions.Test
{
    public class OfTypeTest
    {
        class A
        {
        }

        class B : A
        {
        }

        class C : A
        {
        }

        [Test]
        public void SimpleTypeEmpty()
        {
            var l = new List<string>();
            var e = EnumerableExtensions.OfType<string, string>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeSingleItem00()
        {
            var l = new List<string>() { "a" };
            var e = EnumerableExtensions.OfType<string, string>(l);
            var item = "";
            int count = 0;
            foreach (var c in e)
            {
                item = c;
                count++;
            }
            Assert.AreEqual("a", item);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void SimpleTypeSingleItem01()
        {
            var l = new List<int>() { 25 };
            var e = EnumerableExtensions.OfType<int, int>(l);
            int item = 0;
            int count = 0;
            foreach (var c in e)
            {
                item = c;
                count++;
            }
            Assert.AreEqual(25, item);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void SimpleTypeLots00()
        {
            var l = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k"
            };
            var e = EnumerableExtensions.OfType<string, string>(l);
            int count = 0;
            foreach (var c in e)
            {
                Assert.AreEqual(c, l[count]);
                count++;
            }
            Assert.AreEqual(11, count);
        }

        [Test]
        public void SimpleTypeLots01()
        {
            var l = new List<int>() { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var e = EnumerableExtensions.OfType<int, int>(l);
            int count = 0;
            foreach (var c in e)
            {
                Assert.AreEqual(c, l[count]);
                count++;
            }
            Assert.AreEqual(10, count);
        }

        [Test]
        public void SimpleTypeMismatchEmpty00()
        {
            var l = new List<string>();
            var e = EnumerableExtensions.OfType<string, int>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchEmpty01()
        {
            var l = new List<string>();
            var e = EnumerableExtensions.OfType<string, float>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchEmpty02()
        {
            var l = new List<string>();
            var e = EnumerableExtensions.OfType<string, List<int>>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchSingleItem00()
        {
            var l = new List<string>() { "a" };
            var e = EnumerableExtensions.OfType<string, int>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchSingleItem01()
        {
            var l = new List<int>() { 26 };
            var e = EnumerableExtensions.OfType<int, string>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchSingleItem02()
        {
            var l = new List<Dictionary<float, double>>() { new Dictionary<float, double>() };
            var e = EnumerableExtensions.OfType<Dictionary<float, double>, string>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchLots00()
        {
            var l = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k"
            };
            var e = EnumerableExtensions.OfType<string, List<string>>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchLots01()
        {
            var l = new List<int>()
            {
                234, 778, 4958585, 1232, 0, 12333
            };
            var e = EnumerableExtensions.OfType<int, double>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SimpleTypeMismatchLots02()
        {
            var l = new List<List<double>>()
            {
                new List<double> { 5.6, 5.4, 12.7 },
                new List<double> { 5.6, 5.4, 12.7 },
                new List<double> { 333.7, 5.4, 12.7 },
                new List<double> { 5.6, 5.4, 9.7 },
                new List<double> { 5.6, 5.4, 12.7 },
                new List<double> { 5.6, 5.4, .7 },
            };
            var e = EnumerableExtensions.OfType<List<double>, List<int>>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ComplexTypeEmpty00()
        {
            var l = new List<B>();
            var e = EnumerableExtensions.OfType<B, A>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ComplexTypeEmpty01()
        {
            var l = new List<C>();
            var e = EnumerableExtensions.OfType<C, A>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ComplexTypeSingleItem()
        {
            var b = new B();
            var l = new List<B>() { b };
            var e = EnumerableExtensions.OfType<B, A>(l);
            int count = 0;
            A item = null;
            foreach (var c in e)
            {
                item = c;
                count++;
            }
            Assert.AreEqual(b, item);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ComplexTypeSingleItemMismatch()
        {
            var b = new B();
            var l = new List<A>() { b };
            var e = EnumerableExtensions.OfType<A, C>(l);
            int count = 0;
            foreach (var c in e)
                count++;
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ComplexTypeLots00()
        {
            var l = new List<A>()
            {
                new B(), new C(),
                new B(), new C(),
                new B(), new C(),
                new B(), new C(),
                new B(),
            };
            var e = EnumerableExtensions.OfType<A, B>(l);
            int count = 0;
            foreach (var c in e)
            {
                Assert.IsTrue(c is B);
                count++;
            }
            Assert.AreEqual(5, count);
        }

        [Test]
        public void ComplexTypeLots01()
        {
            var l = new List<A>()
            {
                new B(), new C(),
                new B(), new C(),
                new B(), new C(),
                new B(), new C(),
                new B(),
            };
            var e = EnumerableExtensions.OfType<A, C>(l);
            int count = 0;
            foreach (var c in e)
            {
                Assert.IsTrue(c is C);
                count++;
            }
            Assert.AreEqual(4, count);
        }
    }
}
