using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class Number : IComparable<Number>
    {
        readonly int number;

        public Number(int n)
        {
            number = n;
        }

        public int CompareTo(Number other)
        {
            if (number < other.number)
                return -1;
            if (number > other.number)
                return 1;
            return 0;
        }
    }

    public class IComparableTest
    {

        [Test]
        public void CompareTo()
        {
            var one = new Number(1);
            var oneMore = new Number(1);
            var two = new Number(2);
            Assert.IsTrue(one.CompareTo(two) < 0);
            Assert.IsTrue(one.CompareTo(oneMore) == 0);
            Assert.IsTrue(two.CompareTo(one) > 0);
        }
    }
}
