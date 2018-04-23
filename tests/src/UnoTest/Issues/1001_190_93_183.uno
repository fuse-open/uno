using Uno.Testing;
using Uno;

namespace UnoTest.Issues
{
    public class Issue1001_190_93_183
    {
        [Test]
        public void Run()
        {
            // This should not crash/U_FATAL on C++ #1001
            var a = new[] {"a", "b"};
            var list = (Uno.Collections.IList<string>) a;

            Assert.Throws<InvalidCastException>(Unbox1);
            Assert.Throws<NullReferenceException>(Unbox2);
        }

        // #190
        void Unbox1()
        {
            double number = 3.3;
            object obj = number;
            int intNumber = (int) obj;
        }

        // #93
        void Unbox2()
        {
            object a = null;
            var b = (AttributeTargets) a;
        }

        // #183
        public struct NodeBounds
        {
            static NodeBounds _empty = new NodeBounds{ _isInfinite = false, _isEmpty = true };
            static public NodeBounds Empty
            {
                get { return _empty; }
            }

            bool _isInfinite, _isEmpty;
        }
    }
}
