using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public class LinkedListFactory : CollectionFactory
    {
        public ICollection<T> CreateCollection<T>()
        {
            return new LinkedList<T>();
        }
    }

    public class LinkedListTest
    {
        void AddAfterNull1()
        {
            var l = new LinkedList<int>();
            l.AddAfter(null, new LinkedListNode<int>(123));
        }

        void AddAfterNull2()
        {
            var l = new LinkedList<int>();
            l.AddFirst(123);
            l.AddAfter(l.First, null);
        }

        void AddAfterFromOtherList()
        {
            var l1 = new LinkedList<int>();
            var l2 = new LinkedList<int>();
            l1.AddFirst(123);
            var n = l2.AddFirst(123);
            l1.AddAfter(l1.First, n);
        }

        void AddAfterTwice()
        {
            var l = new LinkedList<int>();
            l.AddFirst(123);
            var n = l.AddAfter(l.First, 456);
            l.AddAfter(n, n);
        }

        [Test]
        public void AddAfter()
        {
            {
                var l = new LinkedList<int>();
                var n = l.AddFirst(1);
                l.AddAfter(n, 3);
                l.AddAfter(n, 2);
                Assert.AreCollectionsEqual<int>(new int[] {1, 2, 3}, l);
            }

            {
                var l = new LinkedList<int>();
                var n = l.AddFirst(1);
                l.AddAfter(n, 4);
                var m = l.AddAfter(n, 2);
                l.AddAfter(m, 3);
                Assert.AreCollectionsEqual<int>(new int[] {1, 2, 3, 4}, l);
            }

            Assert.Throws<ArgumentNullException>(AddAfterNull1);
            Assert.Throws<ArgumentNullException>(AddAfterNull2);
            Assert.Throws<InvalidOperationException>(AddAfterFromOtherList);
            Assert.Throws<InvalidOperationException>(AddAfterTwice);
        }

        void AddBeforeNull1()
        {
            var l = new LinkedList<int>();
            l.AddBefore(null, new LinkedListNode<int>(123));
        }

        void AddBeforeNull2()
        {
            var l = new LinkedList<int>();
            l.AddFirst(123);
            l.AddBefore(l.First, null);
        }

        void AddBeforeFromOtherList()
        {
            var l1 = new LinkedList<int>();
            var l2 = new LinkedList<int>();
            l1.AddFirst(123);
            var n = l2.AddFirst(123);
            l1.AddBefore(l1.First, n);
        }

        void AddBeforeTwice()
        {
            var l = new LinkedList<int>();
            l.AddFirst(123);
            var n = l.AddBefore(l.First, 456);
            l.AddBefore(n, n);
        }

        [Test]
        public void AddBefore()
        {
            {
                var l = new LinkedList<int>();
                var n = l.AddFirst(1);
                l.AddBefore(n, 3);
                l.AddBefore(n, 2);
                Assert.AreCollectionsEqual<int>(new int[] {3, 2, 1}, l);
            }

            {
                var l = new LinkedList<int>();
                var n = l.AddFirst(1);
                l.AddBefore(n, new LinkedListNode<int>(4));
                var m = l.AddBefore(n, 2);
                l.AddBefore(m, 3);
                Assert.AreCollectionsEqual<int>(new int[] {4, 3, 2, 1}, l);
            }

            Assert.Throws<ArgumentNullException>(AddBeforeNull1);
            Assert.Throws<ArgumentNullException>(AddBeforeNull2);
            Assert.Throws<InvalidOperationException>(AddBeforeFromOtherList);
            Assert.Throws<InvalidOperationException>(AddBeforeTwice);
        }

        void AddFirstNull()
        {
            var l = new LinkedList<int>();
            LinkedListNode<int> n = null;
            l.AddFirst(n);
        }

        void AddFirstFromOtherList()
        {
            var l1 = new LinkedList<int>();
            var e = l1.AddFirst(5);
            var l2 = new LinkedList<int>();
            l2.AddFirst(e);
        }

        void AddFirstTwice()
        {
            var l = new LinkedList<int>();
            var e = l.AddFirst(5);
            l.AddFirst(e);
        }

        [Test]
        public void AddFirst()
        {
            var l = new LinkedList<int>();
            var n = l.AddFirst(6);
            Assert.AreEqual(n, l.First);
            Assert.AreEqual(n, l.Last);

            l.AddFirst(7);
            l.AddFirst(123);
            Assert.AreCollectionsEqual<int>(new int[] {123, 7, 6}, l);

            Assert.Throws<ArgumentNullException>(AddFirstNull);
            Assert.Throws<InvalidOperationException>(AddFirstTwice);
            Assert.Throws<InvalidOperationException>(AddFirstFromOtherList);
        }

        void AddLastNull()
        {
            var l = new LinkedList<int>();
            LinkedListNode<int> n = null;
            l.AddLast(n);
        }

        void AddLastFromOtherList()
        {
            var l1 = new LinkedList<int>();
            var e = l1.AddLast(5);
            var l2 = new LinkedList<int>();
            l2.AddLast(e);
        }

        void AddLastTwice()
        {
            var l = new LinkedList<int>();
            var e = l.AddLast(5);
            l.AddLast(e);
        }

        [Test]
        public void AddLast()
        {
            var l = new LinkedList<int>();
            var n = l.AddLast(6);
            Assert.AreEqual(n, l.First);
            Assert.AreEqual(n, l.Last);

            l.AddLast(7);
            l.AddLast(123);
            Assert.AreCollectionsEqual<int>(new int[] {6, 7, 123}, l);

            Assert.Throws<ArgumentNullException>(AddLastNull);
            Assert.Throws<InvalidOperationException>(AddLastTwice);
            Assert.Throws<InvalidOperationException>(AddLastFromOtherList);
        }

        void RemoveNull()
        {
            var l = new LinkedList<int>();
            l.Remove(null);
        }

        void RemoveDangling()
        {
            var l = new LinkedList<int>();
            l.Remove(new LinkedListNode<int>(0));
        }

        [Test]
        public void Remove()
        {
            var l = new LinkedList<int>();
            var a = l.AddLast(1);
            var b = l.AddLast(2);
            var c = l.AddLast(3);
            var d = l.AddLast(4);
            Assert.AreCollectionsEqual<int>(new int[] {1, 2, 3, 4}, l);

            l.Remove(c);
            Assert.AreCollectionsEqual<int>(new int[] {1, 2, 4}, l);

            l.Remove(b);
            Assert.AreCollectionsEqual<int>(new int[] {1, 4}, l);

            l.Remove(d);
            Assert.AreCollectionsEqual<int>(new int[] {1}, l);

            l.Remove(a);
            Assert.AreCollectionsEqual<int>(new int[] {}, l);

            Assert.Throws<ArgumentNullException>(RemoveNull);
            Assert.Throws<InvalidOperationException>(RemoveDangling);
        }

        [Test]
        public void Find()
        {
            var l = new LinkedList<int>();
            var a = l.AddLast(1);
            var b = l.AddLast(2);
            var c = l.AddLast(4);

            Assert.AreEqual(null, l.Find(0));
            Assert.AreEqual(a,    l.Find(1));
            Assert.AreEqual(b,    l.Find(2));
            Assert.AreEqual(null, l.Find(3));
            Assert.AreEqual(c,    l.Find(4));
            Assert.AreEqual(null, l.Find(5));
        }

        static bool IsConsistent<T>(LinkedList<T> list)
        {
            LinkedListNode<T> prev = null, curr = list.First;
            while (curr != null)
            {
                if (curr.Previous != prev)
                    return false;

                prev = curr;
                curr = curr.Next;
            }

            return list.Last == prev;
        }

        [Test]
        public void MutationConsistensy()
        {
            var r = new Random(42);
            var l = new LinkedList<int>();
            var reference = new List<int>();
            var nodes = new List<LinkedListNode<int>>();
            for (int i = 0; i < 1000; ++i)
            {
                LinkedListNode<int> n;
                var mode = r.Next(3);

                switch (mode) {
                case 0: {
                        var val = r.Next();
                        LinkedListNode<int> node;
                        if (r.Next(2) > 0)
                        {
                            node = l.AddFirst(i);
                            reference.Insert(0, i);
                        }
                        else
                        {
                            node = l.AddLast(i);
                            reference.Add(i);
                        }
                        nodes.Add(node);
                    }
                    break;

                case 1:
                    if (nodes.Count > 0)
                    {
                        // remove node
                        var idx = r.Next(nodes.Count);
                        var node = nodes[idx];
                        l.Remove(node);
                        reference.Remove(node.Value);
                        nodes.RemoveAt(idx);
                    }
                    break;

                case 2:
                    if (nodes.Count > 0)
                    {
                        // reinsert node
                        var idx = r.Next(nodes.Count);
                        var node = nodes[idx];
                        l.Remove(node);
                        reference.Remove(node.Value);
                        if (r.Next(2) > 0)
                        {
                            l.AddFirst(node);
                            reference.Insert(0, node.Value);
                        }
                        else
                        {
                            l.AddLast(node);
                            reference.Add(node.Value);
                        }
                    }
                    break;
                }

                Assert.IsTrue(IsConsistent(l));
                Assert.AreCollectionsEqual<int>(reference, l);
            }
        }

        public class Foo
        {
            public readonly int Bar;
            public Foo(int bar)
            {
                Bar = bar;
            }

            public override bool Equals(Object obj)
            {
                Foo foo = obj as Foo;
                if (foo != null)
                    return foo.Bar == Bar;
                return false;
            }
        }

        [Test]
        public void FindObject()
        {
            var l = new LinkedList<Foo>();
            var a = l.AddLast(new Foo(1));
            var b = l.AddLast(new Foo(2));
            var c = l.AddLast(new Foo(4));

            Assert.AreEqual(null, l.Find(new Foo(0)));
            Assert.AreEqual(a,    l.Find(new Foo(1)));
            Assert.AreEqual(b,    l.Find(new Foo(2)));
            Assert.AreEqual(null, l.Find(new Foo(3)));
            Assert.AreEqual(c,    l.Find(new Foo(4)));
            Assert.AreEqual(null, l.Find(new Foo(5)));
        }

        [Test]
        public void Collection()
        {
            CollectionTester.All(new LinkedListFactory());
        }
    }
}
