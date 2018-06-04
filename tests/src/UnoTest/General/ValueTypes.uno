using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    struct Value
    {
        int A, B;

        public Value(int a, int b)
        {
            A = a;
            B = b;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }
    }

    class ValueTypes
    {
        [Test]
        [Ignore("Fails on .NET", "DOTNET")]
        public static void Test()
        {
            var a = (object) new Value(1, 2);
            var b = (object) new Value(1, 2);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }
    }
}
