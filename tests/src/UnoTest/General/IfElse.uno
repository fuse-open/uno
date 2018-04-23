using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class IfElse
    {
        [Test]
        public void Run()
        {
            if (true)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }

            if (false)
                Assert.IsTrue(false);
            else
                Assert.IsTrue(true);

            int a = 5;
            int b = 6;

            if (a < b)
            {
                Assert.IsTrue(true);
            }
            else if (a == b)
            {
                Assert.IsTrue(false);
            }
            else if (a > b)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(false);
            }

            if (true) Assert.IsTrue(true);
            else Assert.IsTrue(false);

            if (!false) Assert.IsTrue(true);

            if (false) ;

            if (true) ;

            bool elseHit = false;
            if (true) ;
            else elseHit = true;
            Assert.IsTrue(!elseHit);

            elseHit = false;
            if (false) ;
            else elseHit = true;
            Assert.IsTrue(elseHit);
        }
    }
}
