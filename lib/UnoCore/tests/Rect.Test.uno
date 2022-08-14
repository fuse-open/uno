using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class RectTest
    {
        [Test]
        public void InfCtor()
        {
            var r = new Rect(float2(float.NegativeInfinity), float2(float.PositiveInfinity));
            Assert.IsTrue(float.IsInfinity(r.Minimum.X));
            Assert.IsTrue(float.IsInfinity(r.Minimum.Y));
            Assert.IsTrue(float.IsInfinity(r.Maximum.X));
            Assert.IsTrue(float.IsInfinity(r.Maximum.X));

            r = new Rect(float2(0), float2(float.NegativeInfinity));
            Assert.AreEqual(0,r.Minimum.X);
            Assert.AreEqual(0,r.Minimum.Y);
            Assert.IsTrue(float.IsInfinity(r.Maximum.X));
            Assert.IsTrue(float.IsInfinity(r.Maximum.X));
        }

        [Test]
        public void InfSize()
        {
            var r = new Rect(float2(float.NegativeInfinity), float2(float.PositiveInfinity, float.NegativeInfinity));
            Assert.IsTrue(float.IsPositiveInfinity(r.Size.X));
            Assert.IsTrue(float.IsNegativeInfinity(r.Size.Y));
        }

        [Test]
        public void InfArea()
        {
            var r = new Rect(float2(-10), float2(float.PositiveInfinity, 10));
            Assert.IsTrue(float.IsPositiveInfinity(r.Area));

            r.Width = 10;
            r.Height = float.NegativeInfinity;
            Assert.IsTrue(float.IsNegativeInfinity(r.Area));
        }

        [Test]
        public void InfPosition()
        {
            var r = new Rect(float2(0), float2(float.PositiveInfinity, float.NegativeInfinity));
            r.Position = float2(-10);
            Assert.AreEqual(float2(-10), r.Position);
            //just ensure they haven't become NaN
            Assert.IsTrue(float.IsPositiveInfinity(r.Width));
            Assert.IsTrue(float.IsNegativeInfinity(r.Height));
        }

        [Test]
        public void InfUnion()
        {
            var r = new Rect(float2(10,20), float2(float.PositiveInfinity));
            var s = new Rect(float2(-50,-60),float2(100,200));
            var t = Rect.Union(r,s);
            Assert.AreEqual(float2(-50,-60),t.Minimum);
            //TODO: this doesn't work on DotNet for some reason
            //Assert.AreEqual(float2(float.PositiveInfinity),t.Maximum);
            Assert.IsTrue(float.IsPositiveInfinity(t.Maximum.X));
            Assert.IsTrue(float.IsPositiveInfinity(t.Maximum.Y));
        }

        [Test]
        public void InfIntersect()
        {
            var r = new Rect(float2(10,20), float2(float.PositiveInfinity));
            var s = new Rect(float2(-50,-60),float2(100,200));
            var t = Rect.Intersect(r,s);
            Assert.AreEqual(float2(10,20),t.Minimum);
            Assert.AreEqual(float2(50,140),t.Maximum);
        }
    }
}

