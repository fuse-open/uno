using Uno;
using Uno.Diagnostics;
using Uno.Testing;
using Uno.Threading;

namespace Diagnostics.Test
{
    public class ClockTest
    {
        [Test]
        public void GetTicks()
        {
            var start = Clock.GetTicks();
            Thread.Sleep(1000);
            var end = Clock.GetTicks();
            Assert.AreEqual(10000000, end - start, 1000000);
        }
    }
}
