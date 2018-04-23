using Uno.Testing;

namespace Generic1
{
    class Class
    {
        [Test]
        public static void Main()
        {
            var elm = default(Optional<int>);
            var result = Extensions.ToList(elm);
            Assert.IsTrue(CreateArray(1)[0] == 1);
        }

        static T[] CreateArray<T>(T value)
        {
            return new T[] {value};
        }
    }

    class Extensions
    {
        public static ImmutableList<T> ToList<T>(this Optional<T> element)
        {
            return element.HasValue
                ? new ImmutableList<T>(new T[] { element.Value })
                : ImmutableList<T>.Empty;
        }
    }

    class ImmutableList<T>
    {
        public static ImmutableList<T> Empty
        {
            get { return null; }
        }

        public ImmutableList(T[] args)
        {
        }
    }

    struct Optional<T>
    {
        public T Value;
        public bool HasValue;
    }
}
