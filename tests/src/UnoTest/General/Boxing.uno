using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Boxing
    {
        [Test]
        public void Run()
        {
            float3 f = float3(1,2,3);

            Assert.AreEqual(1, f.X);
            Assert.AreEqual(2, f.Y);
            Assert.AreEqual(3, f.Z);

            object k = f;

            float3 m = (float3)k;

            f.X = 10;
            Assert.AreEqual(1, m.X);
            Assert.AreEqual(10, f.X);
            m.X = 20;

            float3 n = (float3)k;
            Assert.AreEqual(1, n.X);
            Assert.AreEqual(10, f.X);
            Assert.AreEqual(20, m.X);

            float3 t = n;
            n.X = 1337;
            Assert.AreEqual(1, t.X);
            t.X = 1338;
            Assert.AreEqual(1337, n.X);
            Assert.AreEqual(1338, t.X);
        }
    }
}
