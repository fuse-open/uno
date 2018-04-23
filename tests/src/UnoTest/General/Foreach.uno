using Uno;
using Uno.Collections;
using Uno.Testing;

namespace UnoTest.General
{
    public class Foreach
    {
        public class FooEnumerator : IEnumerator<string>
        {
            public string Current
            {
                get { return ""; }
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
            }

            public bool MoveNext()
            {
                return false;
            }
        }

        public class Foo : IEnumerable<string>
        {
            public IEnumerator<string> GetEnumerator()
            {
                return new FooEnumerator();
            }
        }

        [Test]
        public void Run()
        {
            {
                var arr = new byte[] { 0, 1, 2, 3, 4, };
                byte i = 0;

                foreach (var elm in arr)
                {
                    Assert.AreEqual(i, elm);
                    i++;
                }

                Assert.AreEqual(i, arr.Length);
            }
            {
                var i = 0;
                foreach (var a in new[] { 1, 2, 3 })
                {
                    Assert.AreEqual(i + 1, a);
                    i = a;
                    //a = 10; // Gives error as expected
                }
            }
            {
                var list = new List<int>() { 0, 1, 2, 3, 4, };
                var i = 0;

                foreach (var elm in list)
                {
                    Assert.AreEqual(i, elm);
                    i++;
                }

                Assert.AreEqual(i, list.Count);
            }
            {
                var i = 0;

                foreach (var c in "foobar")
                    i++;

                Assert.AreEqual(6, i);
            }
            {
                IEnumerable<char> a = "foo";

                foreach (var e in a)
                    ;

                IEnumerable<int> b = new[] { 1, 2, 3, };

                foreach (object e in b)
                    ;

                foreach (var e in b)
                    ;
            }
        }
    }
}
