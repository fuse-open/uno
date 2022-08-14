using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class RandomTest
    {
        [Test]
        public void NextDistribution()
        {
            var r = new Random(1337);
            const int iterations = 1000000;

            int sum = 0;
            for (int i = 0; i < iterations; ++i)
                sum += r.Next(2);

            Assert.AreEqual(0.5, (double)sum / iterations, 0.0001);

            r = new Random(1337);
            sum = 0;
            for (int i = 0; i < iterations; ++i)
                sum += r.Next(1, 3) - 1;

            Assert.AreEqual(0.5, (double)sum / iterations, 0.0001);
        }

        [Test]
        public void IntOnZeroRange_ReturnsThatNumber()
        {
            var r = new Random(1337);
            Assert.AreEqual(16, r.Next(16,16));
        }

        void NextNegativeLow()
        {
            var r = new Random(1337);
            r.Next(-1);
        }

        [Test]
        public void Next()
        {
            var r = new Random(1337);

            Assert.AreEqual(0, r.Next(0));
            Assert.Throws<ArgumentOutOfRangeException>(NextNegativeLow);
        }

        [Test]
        public void IntWhenHighIsLowerThanLow_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IntHighIsLowerThanLow);
        }

        private void IntHighIsLowerThanLow()
        {
            var r = new Random(1337);
            r.Next(2,1);
        }

        [Test]
        [Obsolete]
        public void FloatOnZeroRange_ReturnsThatNumber()
        {
            var r = new Random(1337);
            Assert.AreEqual(13f, r.NextFloat(13f, 13f));
        }

        [Test]
        [Obsolete]
        public void FloatWhenHighIsLowerThanLow_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(FloatHighIsLowerThanLow);
        }

        [Obsolete]
        private void FloatHighIsLowerThanLow()
        {
            var r = new Random(1337);
            r.NextFloat(1.1f, 1.0f);
        }
    }
}
