using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class TupleTest
    {

        //1 tuple

        [Test]
        public void Tuple1CreateAndAccess()
        {
            var t = Tuple.Create("foo");
            Assert.AreEqual("foo", t.Item1);
        }

        [Test]
        public void Tuple1Equality()
        {
            var t1 = Tuple.Create("foo");
            var t2 = Tuple.Create("foo");
            var t3 = Tuple.Create("bar");
            Assert.AreEqual(t1, t2);
            Assert.AreNotEqual(t1, t3);
        }

        [Test]
        public void Tuple1ToString()
        {
            var t1 = Tuple.Create("foo", 42);
            Assert.AreEqual("(foo, 42)", t1.ToString());
        }

        //2 tuple

        [Test]
        public void Tuple2CreateAndAccess()
        {
            var t = Tuple.Create("foo", 42);
            Assert.AreEqual("foo", t.Item1);
            Assert.AreEqual(42, t.Item2);
        }

        [Test]
        public void Tuple2Equality()
        {
            var t1 = Tuple.Create("foo", 42);
            var t2 = Tuple.Create("foo", 42);
            var t3 = Tuple.Create("bar", 42);
            var t4 = Tuple.Create("foo", 44);
            Assert.AreEqual(t1, t2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreNotEqual(t1, t4);
        }

        [Test]
        public void Tuple2ToString()
        {
            var t1 = Tuple.Create("foo", 42);
            Assert.AreEqual("(foo, 42)", t1.ToString());
        }

        //3 tuple

        [Test]
        public void Tuple3CreateAndAccess()
        {
            var t = Tuple.Create("foo", 42, 'a');
            Assert.AreEqual("foo", t.Item1);
            Assert.AreEqual(42, t.Item2);
            Assert.AreEqual('a', t.Item3);
        }

        [Test]
        public void Tuple3Equality()
        {
            var t1 = Tuple.Create("foo", 42, 'a');
            var t2 = Tuple.Create("foo", 42, 'a');
            var t3 = Tuple.Create("bar", 42, 'a');
            var t4 = Tuple.Create("foo", 44, 'a');
            var t5 = Tuple.Create("foo", 42, 'c');
            Assert.AreEqual(t1, t2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreNotEqual(t1, t4);
            Assert.AreNotEqual(t1, t5);
        }

        [Test]
        public void Tuple3ToString()
        {
            var t1 = Tuple.Create("foo", 42, 'a');
            Assert.AreEqual("(foo, 42, a)", t1.ToString());
        }

        //4 tuple

        [Test]
        public void Tuple4CreateAndAccess()
        {
            var t = Tuple.Create(1, 2, 3, 4);
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
        }

        [Test]
        public void Tuple4Equality()
        { 
            var t1 = Tuple.Create(1, 1, 1, 1);
            Assert.AreEqual(t1, Tuple.Create(1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(2, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 2, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 2, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 2));
        }

        [Test]
        public void Tuple4ToString()
        {
            var t1 = Tuple.Create(1, 2, 3, 4);
            Assert.AreEqual("(1, 2, 3, 4)", t1.ToString());
        }

        //5 tuple

        [Test]
        public void Tuple5CreateAndAccess()
        {
            var t = Tuple.Create(1, 2, 3, 4, 5);
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
            Assert.AreEqual(5, t.Item5);
        }

        [Test]
        public void Tuple5Equality()
        {
            var t1 = Tuple.Create(1, 1, 1, 1, 1);
            Assert.AreEqual(t1, Tuple.Create(1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(2, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 2, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 2, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 2, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 2));
        }

        [Test]
        public void Tuple5ToString()
        {
            var t1 = Tuple.Create(1, 2, 3, 4, 5);
            Assert.AreEqual("(1, 2, 3, 4, 5)", t1.ToString());
        }

        //6 tuple

        [Test]
        public void Tuple6CreateAndAccess()
        {
            var t = Tuple.Create(1, 2, 3, 4, 5, 6);
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
            Assert.AreEqual(5, t.Item5);
            Assert.AreEqual(6, t.Item6);
        }

        [Test]
        public void Tuple6Equality()
        {
            var t1 = Tuple.Create(1, 1, 1, 1, 1, 1);
            Assert.AreEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(2, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 2, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 2, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 2, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 2, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 2));
        }

        [Test]
        public void Tuple6ToString()
        {
            var t1 = Tuple.Create(1, 2, 3, 4, 5, 6);
            Assert.AreEqual("(1, 2, 3, 4, 5, 6)", t1.ToString());
        }

        //7 tuple

        [Test]
        public void Tuple7CreateAndAccess()
        {
            var t = Tuple.Create(1, 2, 3, 4, 5, 6, 7);
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
            Assert.AreEqual(5, t.Item5);
            Assert.AreEqual(6, t.Item6);
            Assert.AreEqual(7, t.Item7);
        }

        [Test]
        public void Tuple7Equality()
        {
            var t1 = Tuple.Create(1, 1, 1, 1, 1, 1, 1);
            Assert.AreEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(2, 1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 2, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 2, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 2, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 2, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 2, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1, 2));
        }

        [Test]
        public void Tuple7ToString()
        {
            var t1 = Tuple.Create(1, 2, 3, 4, 5, 6, 7);
            Assert.AreEqual("(1, 2, 3, 4, 5, 6, 7)", t1.ToString());
        }

        //8 tuple

        [Test]
        public void Tuple8CreateAndAccess()
        {
            var t = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
            Assert.AreEqual(5, t.Item5);
            Assert.AreEqual(6, t.Item6);
            Assert.AreEqual(7, t.Item7);
            Assert.AreEqual(Tuple.Create(8), t.Rest);
        }

        [Test]
        public void Tuple8Equality()
        {
            var t1 = Tuple.Create(1, 1, 1, 1, 1, 1, 1, 1);
            Assert.AreEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(2, 1, 1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 2, 1, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 2, 1, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 2, 1, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 2, 1, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 2, 1, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1, 2, 1));
            Assert.AreNotEqual(t1, Tuple.Create(1, 1, 1, 1, 1, 1, 1, 2));
        }

        [Test]
        public void Tuple8ToString()
        {
            var t1 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.AreEqual("(1, 2, 3, 4, 5, 6, 7, 8)", t1.ToString());
        }

        // Tuples with more than 8 elements

        [Test]
        public void Tuple9CreateAndAccess()
        {
            var t = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 2, 3, 4, 5, 6, 7, new Tuple<int, int>(8, 9));
            Assert.AreEqual(1, t.Item1);
            Assert.AreEqual(2, t.Item2);
            Assert.AreEqual(3, t.Item3);
            Assert.AreEqual(4, t.Item4);
            Assert.AreEqual(5, t.Item5);
            Assert.AreEqual(6, t.Item6);
            Assert.AreEqual(7, t.Item7);
            Assert.AreEqual(Tuple.Create(8, 9), t.Rest);
        }

        [Test]
        public void Tuple9Equality()
        {
            var t = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 1, 1, new Tuple<int, int>(1, 1));
            Assert.AreEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(2, 1, 1, 1, 1, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 2, 1, 1, 1, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 2, 1, 1, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 2, 1, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 2, 1, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 2, 1, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 1, 2, new Tuple<int, int>(1, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 1, 1, new Tuple<int, int>(2, 1)));
            Assert.AreNotEqual(t, new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 1, 1, 1, 1, 1, 1, new Tuple<int, int>(1, 2)));
        }

        [Test]
        public void Tuple9ToString()
        {
            var t = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 2, 3, 4, 5, 6, 7, new Tuple<int, int>(8, 9));
            Assert.AreEqual("(1, 2, 3, 4, 5, 6, 7, 8, 9)", t.ToString());
        }

        [Test]
        public void UberTupleToString()
        {
            var t3 = new Tuple<int, int>(15, 16);
            var t2 = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(8, 9, 10, 11, 12, 13, 14, t3);
            var t1 = new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int>>> (1, 2, 3, 4, 5, 6, 7, t2);
            Assert.AreEqual("(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)", t1.ToString());
        }

        // Hash codes

        [Test]
        public void HashCodesAreDifferent()
        {
            //It's hard to test this without the test code duplicating the
            //implementation. Instead we make one tuple of each length, all
            //starting with identical elements, and make sure they are
            //different. If they are the same, we probably forgot one component
            //in a GetHashCode().

            var codes = new int[8];
            codes[0] = Tuple.Create(1).GetHashCode();
            codes[1] = Tuple.Create(1, 2).GetHashCode();
            codes[2] = Tuple.Create(1, 2, 4).GetHashCode();
            codes[3] = Tuple.Create(1, 2, 4, 8).GetHashCode();
            codes[4] = Tuple.Create(1, 2, 4, 8, 16).GetHashCode();
            codes[5] = Tuple.Create(1, 2, 4, 8, 16, 32).GetHashCode();
            codes[6] = Tuple.Create(1, 2, 4, 8, 16, 32, 64).GetHashCode();
            codes[7] = Tuple.Create(1, 2, 4, 8, 16, 32, 64, 128).GetHashCode();

            for (int i=0; i<codes.Length; i++)
            {
                for (int j=0; j<codes.Length; j++)
                {
                    if (i != j)
                    {
                        Assert.AreNotEqual(codes[i], codes[j], "codes[" + i + "] == codes[" + j + "] == " + codes[i]);
                    }
                }
            }
        }

        // Type safety

        [Test]
        public void CanOnlyUseTuplesForRest()
        {
            Assert.Throws<ArgumentException>(UseIntegerForRest);
        }

        private void UseIntegerForRest()
        {
            new Tuple<int, int, int, int, int, int, int, int>(1,2,3,4,5,6,7,8);
        }
    }
}
