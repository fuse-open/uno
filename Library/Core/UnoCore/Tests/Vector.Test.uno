using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Uno.Test
{
    public class VectorTest
    {
        float2 _north = float2( 0,  1);
        float2 _south = float2( 0, -1);
        float2 _west  = float2(-1,  0);
        float2 _east  = float2( 1,  0);

        float2 _northWest = float2(-1,  1);
        float2 _northEast = float2( 1,  1);
        float2 _southWest = float2(-1, -1);
        float2 _southEast = float2( 1, -1);

        //Float2
        [Test]
        public void LengthFloat2_1()
        {
            float l1 = Vector.Length(_north);
            float l2 = Vector.Length(_south);
            float l3 = Vector.Length(_east);
            float l4 = Vector.Length(_west);
            Assert.AreEqual(1, l1);
            Assert.AreEqual(1, l2);
            Assert.AreEqual(1, l3);
            Assert.AreEqual(1, l4);
        }

        [Test]
        public void LengthFloat2_2()
        {
            float l1 = Vector.Length(_northWest);
            float l2 = Vector.Length(_northEast);
            float l3 = Vector.Length(_southWest);
            float l4 = Vector.Length(_southEast);
            Assert.AreEqual(1.41421356f, l1);
            Assert.AreEqual(1.41421356f, l2);
            Assert.AreEqual(1.41421356f, l3);
            Assert.AreEqual(1.41421356f, l4);
        }

        [Test]
        public void DistanceFloat2_1()
        {
            var d1 = Vector.Distance(_north, float2(0));
            Assert.AreEqual(1, d1);
        }

        [Test]
        public void DistanceFloat2_2()
        {
            var d1 = Vector.Distance(_south, float2(0));
            Assert.AreEqual(1, d1);
        }

        [Test]
        public void DistanceFloat2_3()
        {
            var d1 = Vector.Distance(_northWest, float2(0));
            Assert.AreEqual(1.41421356f, d1);
        }

        [Test]
        public void DistanceFloat2_4()
        {
            var d1 = Vector.Distance(_northWest, _southEast);
            Assert.AreEqual(1.41421356f*2, d1);
        }

        [Test]
        public void DistanceFloat2_5()
        {
            var d1 = Vector.Distance(float2(1231.43f, -3953.32f), float2(-1395.1234f, 23942.4323f));
            Assert.AreEqual(28019.12976816f, d1, 1e-01);
        }

        [Test]
        public void NormalizeFloat2_1()
        {
            var n1 = Vector.Normalize(_north);
            Assert.AreEqual(1, Vector.Length(n1));
        }

        [Test]
        public void NormalizeFloat2_2()
        {
            var n1 = Vector.Normalize(_southWest);
            Assert.AreEqual(1, Vector.Length(n1));
        }

        [Test]
        public void NormalizeFloat2_3()
        {
            var n1 = Vector.Normalize(float2(4352783.24234f, -3487.00002034f));
            Assert.AreEqual(1, Vector.Length(n1));
        }

        [Test]
        public void DotFloat2_1()
        {
            var d = Vector.Dot(_east, _south);
            Assert.AreEqual(0, d);
        }

        [Test]
        public void DotFloat2_2()
        {
            var d = Vector.Dot(_north, _northEast);
            Assert.AreEqual(1, d);
        }

        [Test]
        public void DotFloat2_3()
        {
            var d = Vector.Dot(_northEast, _west);
            Assert.AreEqual(-1, d);
        }

        [Test]
        public void ReflectFloat2_1()
        {
            var reflection = Vector.Reflect(_east, _east);
            Assert.AreEqual(_west, reflection);
        }

        [Test]
        public void ReflectFloat2_2()
        {
            var reflection = Vector.Reflect(_northWest, _west);
            Assert.AreEqual(_northEast, reflection);
        }

        [Test]
        public void ReflectFloat2_3()
        {
            var reflection = Vector.Reflect(float2(4352783.24234f, -3487.00002034f), _east);
            Assert.AreEqual(float2(-4352783.24234f, -3487.00002034f), reflection);
        }

        [Test]
        public void RefractFloat2_1()
        {
            var refracted = Vector.Refract(_southEast, _north, 1);
            Assert.AreEqual(_southEast, refracted);
        }

        [Test]
        public void RefractFloat2_2()
        {
            var refracted = Vector.Refract(_southEast, _north, 1/1.5f);
            Assert.AreEqual(float2(0.666667f, -1), refracted);
        }

        [Test]
        public void OrthonormalizeFloat2_1()
        {
            var ne = _northEast;
            var e = _east;
            Vector.OrthoNormalize(ref ne, ref e);
            Assert.AreEqual(float2(0.707107f, 0.707107f), ne);
            Assert.AreEqual(float2(0.707107f, -0.707107f), e);
        }

        [Test]
        public void ProjectFloat2_1()
        {
            var proj = Vector.Project(_northEast, _east);
            Assert.AreEqual(_east, proj);
        }

        [Test]
        public void ProjectFloat2_2()
        {
            var proj = Vector.Project(float2(434, -234.54f), float2(12391, 753.452f));
            Assert.AreEqual(float2(418.192f, 25.4288f), proj, 0.001f);
        }

        [Test]
        public void RotateFloat2_1()
        {
            var rotated = Vector.Rotate(_north, Math.PIf/2f);
            Assert.AreEqual(_west, rotated);
        }

        [Test]
        public void RotateFloat2_2()
        {
            var rotated = Vector.Rotate(_north, Math.PIf*2f);
            Assert.AreEqual(_north, rotated);
        }

        [Test]
        public void RotateFloat2_3()
        {
            var rotated = Vector.Rotate(_north, Math.PIf);
            Assert.AreEqual(_south, rotated);
        }

        [Test]
        public void RotateFloat2_4()
        {
            var rotated = Vector.Rotate(_north, Math.PIf*200);
            Assert.AreEqual(_north, rotated, 0.0001f);
        }

    }
}
