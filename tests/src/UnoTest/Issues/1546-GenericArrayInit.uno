using Uno;
using Uno.Collections;
using Uno.Testing;

namespace GenericArrayInit
{
    struct Struct
    {
        public readonly object A;
        public readonly object B;
        public Struct(object a, object b)
        {
            A = a;
            B = b;
        }
    }

    class Class
    {
        [Test]
        public void Run()
        {
            Run(1, 2);
            Run("foo", "bar");
            Run(new Struct("foo", 1), new Struct("bar", 2));
        }

        public void Run<T>(T a, T b)
        {
            var list = new List<T>();
            list.Add(a);
            list.Add(b);
            Assert.AreCollectionsEqual(new T[] { a, b }, list);
        }
    }
}