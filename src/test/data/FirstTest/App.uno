using Uno;
using Uno.Testing;

namespace FirstTest
{
    public class StuffWorks
    {
        [Test]
        public void itDoes()
        {
            Assert.IsTrue( 1 + 1 == 2);
        }
        [Test]
        public void noItDoesnt()
        {
            Assert.IsTrue( 1 + 2 == 5);
        }
        [Test]
        public void itThrows()
        {
            throw new Exception("oops with a space in it");
        }
        [Test]
        [Ignore("Skip this")]
        public void dontRun()
        {
        }

        [Test]
        public void runButIgnore()
        {
            Assert.Ignore("Skip this, too");
        }
    }
}
