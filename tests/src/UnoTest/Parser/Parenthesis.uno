using Uno;
using Uno.Testing;

namespace UnoTest.Parser
{
    public class Parenthesis
    {
        // Issue #1251
        const double señor = 10;

        [Test]
        public void Run()
        {
            float a = 4.0f;
            float b = (a) - 1.0f;
            Assert.AreEqual(4f, a);
            Assert.AreEqual(3f, b);
        }
    }
}
#pragma reset
using Uno;
