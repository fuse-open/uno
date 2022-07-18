using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class Thing : IEquatable<Thing>
    {
        readonly int number;

        public Thing(int n)
        {
            number = n;
        }

        public bool Equals(Thing other)
        {
            return number == other.number;
        }
    }

    public class IEquatableTest
    {

        [Test]
        public void Equals()
        {
            var one = new Thing(1);
            var oneMore = new Thing(1);
            var two = new Thing(2);
            Assert.IsTrue(one.Equals(one));
            Assert.IsTrue(one.Equals(oneMore));
            Assert.IsFalse(one.Equals(two));
        }
    }
}
