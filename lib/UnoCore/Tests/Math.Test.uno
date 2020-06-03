using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;
using Uno.Testing.Assert;

namespace Uno.Test
{
    public class MathTest
    {
        [Test]
        [Obsolete]
        public void RoundingMode()
        {
            Assert.AreEqual(1, Math.Round(1.5 - 1e-10), 0);
            Assert.AreEqual(1, Math.Round((float)(1.5 - 1e-5)), 0);

            Assert.AreEqual(2, Math.Round(1.5), 0);
            Assert.AreEqual(2, Math.Round(1.5f), 0);

            Assert.AreEqual(3, Math.Round(2.5), 0);
            Assert.AreEqual(3, Math.Round(2.5f), 0);

            Assert.AreEqual(3, Math.Round(2.5 + 1e-10), 0);
            Assert.AreEqual(3, Math.Round((float)(2.5 + 1e-5)), 0);

            Assert.AreEqual(3, Math.Round(3.5 - 1e-10), 0);
            Assert.AreEqual(3, Math.Round((float)(3.5 - 1e-5)), 0);

            Assert.AreEqual(4, Math.Round(3.5), 0);
            Assert.AreEqual(4, Math.Round(3.5f), 0);

            Assert.AreEqual(5, Math.Round(4.5), 0);
            Assert.AreEqual(5, Math.Round(4.5f), 0);
        }

        [Test]
        [Obsolete]
        public void RoundWithDecimals()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Math.Round(Math.PI, -1));
            Assert.AreEqual(3, Math.Round(Math.PI, 0));
            Assert.AreEqual(3.141592653589793, Math.Round(Math.PI, 15));
            Assert.Throws<ArgumentOutOfRangeException>(() => Math.Round(Math.PI, 16));

            Assert.AreEqual(1.5, Math.Round(1.51, 1), 0);
            Assert.AreEqual(1.5f, Math.Round(1.51f, 1), 0);

            Assert.AreEqual(1.5, Math.Round(1.49, 1), 0);
            Assert.AreEqual(1.5f, Math.Round(1.49f, 1), 0);

            Assert.AreEqual(1.123, Math.Round(1.1234, 3), 0);
            Assert.AreEqual(1.123f, Math.Round(1.1234f, 3), 0);

            Assert.AreEqual(1, Math.Round(1.31, 0), 0);
            Assert.AreEqual(1f, Math.Round(1.31f, 0), 0);

            Assert.AreEqual(-1.5, Math.Round(-1.51, 1), 0);
            Assert.AreEqual(-1.5f, Math.Round(-1.51f, 1), 0);

            Assert.AreEqual(-1.5, Math.Round(-1.49, 1), 0);
            Assert.AreEqual(-1.5f, Math.Round(-1.49f, 1), 0);

            Assert.AreEqual(-1.123, Math.Round(-1.1234, 3), 0);
            Assert.AreEqual(-1.123f, Math.Round(-1.1234f, 3), 0);

            Assert.AreEqual(-1, Math.Round(-1.31, 0), 0);
            Assert.AreEqual(-1f, Math.Round(-1.31f, 0), 0);

            Assert.AreEqual(Double.MaxValue, Math.Round(Double.MaxValue, 0), 0);
            Assert.AreEqual(Double.MaxValue, Math.Round(Double.MaxValue, 15), 0);
        }

        [Test]
        [Obsolete]
        public void RoundsVectorsWithNoDecimals()
        {
            Assert.AreEqual(float2(1,3), Math.Round(float2(1.4f, 2.9f)), 0);
            Assert.AreEqual(float3(2,3,5), Math.Round(float3(2.1f, 2.8f, 4.6f)), 0);
            Assert.AreEqual(float4(2,3,5,1), Math.Round(float4(2.1f, 2.8f, 4.6f, 0.7f)), 0);
        }

        [Test]
        [Obsolete]
        public void RoundsVectorsWithDecimals()
        {
            Assert.AreEqual(float2(1, 3), Math.Round(float2(1.4f, 2.9f), 0), 0);
            Assert.AreEqual(float3(2.1f, 2.8f, 4.7f), Math.Round(float3(2.1f, 2.811111f, 4.666f), 1), 0);
            Assert.AreEqual(float4(42.23f, 12.33f, 12.76f, 75.12f), Math.Round(float4(42.2345f, 12.331f, 12.7649f, 75.116f), 2), 0);
        }


        [Test]
        public void DoubleDegreesToRadians()
        {
            Assert.AreEqual(0.0, Math.DegreesToRadians(0.0));
            Assert.AreEqual(0.0f, Math.DegreesToRadians(0.0f));

            Assert.AreEqual(Math.PI / 2, Math.DegreesToRadians(90.0));
            Assert.AreEqual((float)Math.PI / 2, Math.DegreesToRadians(90.0f));

            Assert.AreEqual(Math.PI, Math.DegreesToRadians(180.0));
            Assert.AreEqual((float)Math.PI, Math.DegreesToRadians(180.0f));

            Assert.AreEqual(Math.PI * 2, Math.DegreesToRadians(360.0));
            Assert.AreEqual((float)Math.PI * 2, Math.DegreesToRadians(360.0f));

            Assert.AreEqual(Math.PI * 4, Math.DegreesToRadians(720.0));
            Assert.AreEqual((float)Math.PI * 4, Math.DegreesToRadians(720.0f));

            // verify that vector versions work as expected
            var random = new Random(1337);
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.DegreesToRadians(x);
                Assert.AreEqual(float2(expected), Math.DegreesToRadians(float2(x)));
                Assert.AreEqual(float3(expected), Math.DegreesToRadians(float3(x)));
                Assert.AreEqual(float4(expected), Math.DegreesToRadians(float4(x)));
            }
        }

        [Test]
        public void RadiansToDegrees()
        {
            Assert.AreEqual(0.0, Math.RadiansToDegrees(0.0));
            Assert.AreEqual(0.0f, Math.RadiansToDegrees(0.0f));

            Assert.AreEqual(90.0, Math.RadiansToDegrees(Math.PI / 2));
            Assert.AreEqual(90.0f, Math.RadiansToDegrees((float)(Math.PI / 2)));

            Assert.AreEqual(180.0, Math.RadiansToDegrees(Math.PI));
            Assert.AreEqual(180.0f, Math.RadiansToDegrees((float)Math.PI));

            Assert.AreEqual(360.0, Math.RadiansToDegrees(Math.PI * 2));
            Assert.AreEqual(360.0f, Math.RadiansToDegrees((float)(Math.PI * 2)));

            Assert.AreEqual(720.0, Math.RadiansToDegrees(Math.PI * 4));
            Assert.AreEqual(720.0f, Math.RadiansToDegrees((float)(Math.PI * 4)));

            // verify that vector versions work as expected
            var random = new Random(1337);
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.RadiansToDegrees(x);
                Assert.AreEqual(float2(expected), Math.RadiansToDegrees(float2(x)));
                Assert.AreEqual(float3(expected), Math.RadiansToDegrees(float3(x)));
                Assert.AreEqual(float4(expected), Math.RadiansToDegrees(float4(x)));
            }
        }

        [Test]
        public void Cos()
        {
            // cos(0) = 1
            Assert.AreEqual(1.0, Math.Cos(0.0));
            Assert.AreEqual(1.0f, Math.Cos(0.0f));

            // cos(π) = -1
            Assert.AreEqual(-1.0, Math.Cos(Math.PI));
            Assert.AreEqual(-1.0f, Math.Cos((float)Math.PI));

            // cos(-π) = -1
            Assert.AreEqual(-1.0, Math.Cos(-Math.PI));
            Assert.AreEqual(-1.0f, Math.Cos((float)-Math.PI));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble() * 100.0;

                // cos(-θ) = cos(θ)
                Assert.AreEqual(Math.Cos(x),
                                Math.Cos(-x));
                Assert.AreEqual(Math.Cos((float)x),
                                Math.Cos((float)x));

                // cos(θ + 2π) = cos(θ)
                Assert.AreEqual(Math.Cos(x),
                                Math.Cos(x + 2 * Math.PI));
                Assert.AreEqual(Math.Cos((float)x),
                                Math.Cos((float)(x + 2 * Math.PI)));

                // cos(π - θ) = -cos(θ)
                Assert.AreEqual(-Math.Cos(x),
                                Math.Cos(Math.PI - x));
                Assert.AreEqual(-Math.Cos((float)x),
                                Math.Cos((float)(Math.PI - x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Cos(x);
                Assert.AreEqual(float2(expected), Math.Cos(float2(x)));
                Assert.AreEqual(float3(expected), Math.Cos(float3(x)));
                Assert.AreEqual(float4(expected), Math.Cos(float4(x)));
            }
        }

        [Test]
        public void Sin()
        {
            // sin(0) = 0
            Assert.AreEqual(0.0, Math.Sin(0.0));
            Assert.AreEqual(0.0f, Math.Sin(0.0f));

            // sin(π / 2) = 1
            Assert.AreEqual(1.0, Math.Sin(Math.PI / 2));
            Assert.AreEqual(1.0f, Math.Sin((float)(Math.PI / 2)));

            // sin(-π / 2) = -1
            Assert.AreEqual(-1.0, Math.Sin(-Math.PI / 2));
            Assert.AreEqual(-1.0f, Math.Sin((float)(-Math.PI / 2)));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble() * 100.0;

                // sin(-θ) = -sin(θ)
                Assert.AreEqual(-Math.Sin(x),
                                Math.Sin(-x));
                Assert.AreEqual(-Math.Sin((float)x),
                                Math.Sin((float)-x));

                // sin(θ + 2π) = sin(θ)
                Assert.AreEqual(Math.Sin(x),
                                Math.Sin(x + 2 * Math.PI));
                Assert.AreEqual(Math.Sin((float)x),
                                Math.Sin((float)(x + 2 * Math.PI)));

                // sin(π - θ) = sin(θ)
                Assert.AreEqual(Math.Sin(x),
                                Math.Sin(Math.PI - x));
                Assert.AreEqual(Math.Sin((float)x),
                                Math.Sin((float)(Math.PI - x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Sin(x);
                Assert.AreEqual(float2(expected), Math.Sin(float2(x)));
                Assert.AreEqual(float3(expected), Math.Sin(float3(x)));
                Assert.AreEqual(float4(expected), Math.Sin(float4(x)));
            }
        }

        [Test]
        public void SinCosRotation()
        {
            // simulate sin/cos by continously rotating a point
            var px = 0.0;
            var py = 1.0;
            var step = 1.0 / 100;
            var cth = Math.Cos(step);
            var sth = Math.Sin(step);
            for (int i = 0; i < 1000; ++i)
            {
                var th = i * step;

                Assert.AreEqual(px, Math.Sin(th));
                Assert.AreEqual(py, Math.Cos(th));

                var tx = px * cth + py * sth;
                var ty = -px * sth + py * cth;
                px = tx;
                py = ty;
            }
        }

        [Test]
        public void Tan()
        {
            // verify trigonometric identity tan(0) = 0
            Assert.AreEqual(0.0, Math.Tan(0.0));
            Assert.AreEqual(0.0f, Math.Tan(0.0f));
            Assert.AreEqual(float2(0.0f), Math.Tan(float2(0.0f)));
            Assert.AreEqual(float3(0.0f), Math.Tan(float3(0.0f)));
            Assert.AreEqual(float4(0.0f), Math.Tan(float4(0.0f)));

            // verify trigonometric identity tan(π / 4) = 1
            Assert.AreEqual(1.0, Math.Tan(Math.PI / 4));
            Assert.AreEqual(1.0f, Math.Tan((float)(Math.PI / 4)));
            Assert.AreEqual(float2(1.0f), Math.Tan(float2((float)Math.PI / 4)));
            Assert.AreEqual(float3(1.0f), Math.Tan(float3((float)Math.PI / 4)));
            Assert.AreEqual(float4(1.0f), Math.Tan(float4((float)Math.PI / 4)));

            // verify trigonometric identity tan(-π / 4) = -1
            Assert.AreEqual(-1.0, Math.Tan(-Math.PI / 4));
            Assert.AreEqual(-1.0f, Math.Tan((float)(-Math.PI / 4)));
            Assert.AreEqual(float2(-1.0f), Math.Tan(float2((float)(-Math.PI / 4))));
            Assert.AreEqual(float3(-1.0f), Math.Tan(float3((float)(-Math.PI / 4))));
            Assert.AreEqual(float4(-1.0f), Math.Tan(float4((float)(-Math.PI / 4))));

            // verify trigonometric identity tan(θ) = sin(θ) / cos(θ)
            for (int i = -499; i < 500; ++i)
            {
                // avoid sampling at |θ| == Math.PI / 2
                var th = i * ((Math.PI * 0.5) / 500.0);

                var expected = Math.Sin(th) / Math.Cos(th);
                Assert.AreEqual(expected, Math.Tan(th));

                float floatTolerance = (float)Math.Max(1e-5, Math.Abs(expected * 1e-5));
                Assert.AreEqual((float)expected, Math.Tan((float)th), floatTolerance);
                Assert.AreEqual(float2((float)expected), Math.Tan(float2((float)th)), floatTolerance);
                Assert.AreEqual(float3((float)expected), Math.Tan(float3((float)th)), floatTolerance);
                Assert.AreEqual(float4((float)expected), Math.Tan(float4((float)th)), floatTolerance);
            }

            // verify trigonometric identity tan(θ) = tan(θ + π)
            for (int i = -499; i < 500; ++i)
            {
                // avoid sampling at |θ| == π / 2
                var th = i * ((Math.PI * 0.5) / 500.0);

                var expected = Math.Tan(th + Math.PI);
                Assert.AreEqual(expected, Math.Tan(th));

                float floatTolerance = (float)Math.Max(1e-5, Math.Abs(expected * 1e-5));
                Assert.AreEqual((float)expected, Math.Tan((float)th), floatTolerance);
                Assert.AreEqual(float2((float)expected), Math.Tan(float2((float)th)), floatTolerance);
                Assert.AreEqual(float3((float)expected), Math.Tan(float3((float)th)), floatTolerance);
                Assert.AreEqual(float4((float)expected), Math.Tan(float4((float)th)), floatTolerance);
            }

            // verify that vector versions work as expected
            var random = new Random(1337);
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Tan(x);
                Assert.AreEqual(float2(expected), Math.Tan(float2(x)));
                Assert.AreEqual(float3(expected), Math.Tan(float3(x)));
                Assert.AreEqual(float4(expected), Math.Tan(float4(x)));
            }
        }

        [Test]
        public void Asin()
        {
            // asin(nan) = nan
            Assert.IsTrue(double.IsNaN(Math.Asin(double.NaN)));
            Assert.IsTrue(float.IsNaN(Math.Asin(float.NaN)));

            // asin(θ) = nan if θ < -1
            Assert.IsTrue(double.IsNaN(Math.Asin(-1.0 - 1e-10)));
            Assert.IsTrue(float.IsNaN(Math.Asin(-1.0f - 1e-5f)));

            // asin(θ) = nan if θ > 1
            Assert.IsTrue(double.IsNaN(Math.Asin(-1.0 - 1e-10)));
            Assert.IsTrue(float.IsNaN(Math.Asin(-1.0f - 1e-5f)));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();

                // θ = sin(asin(θ))
                Assert.AreEqual(x,
                                Math.Sin(Math.Asin(x)));
                Assert.AreEqual((float)x,
                                Math.Sin(Math.Asin((float)x)));

                // -θ = sin(asin(-θ))
                Assert.AreEqual(-x,
                                Math.Sin(Math.Asin(-x)));
                Assert.AreEqual((float)-x,
                                Math.Sin(Math.Asin((float)-x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Asin(x);
                Assert.AreEqual(float2(expected), Math.Asin(float2(x)));
                Assert.AreEqual(float3(expected), Math.Asin(float3(x)));
                Assert.AreEqual(float4(expected), Math.Asin(float4(x)));
            }
        }

        [Test]
        public void Acos()
        {
            // acos(nan) = nan
            Assert.IsTrue(double.IsNaN(Math.Acos(double.NaN)));
            Assert.IsTrue(float.IsNaN(Math.Acos(float.NaN)));

            // acos(θ) = nan if θ < -1
            Assert.IsTrue(double.IsNaN(Math.Acos(-1.0 - 1e-10)));
            Assert.IsTrue(float.IsNaN(Math.Acos(-1.0f - 1e-5f)));

            // acos(θ) = nan if θ > 1
            Assert.IsTrue(double.IsNaN(Math.Acos(-1.0 - 1e-10)));
            Assert.IsTrue(float.IsNaN(Math.Acos(-1.0f - 1e-5f)));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();

                // θ = cos(acos(θ))
                Assert.AreEqual(x,
                                Math.Cos(Math.Acos(x)));
                Assert.AreEqual((float)x,
                                Math.Cos(Math.Acos((float)x)));

                // -θ = cos(acos(-θ))
                Assert.AreEqual(-x,
                                Math.Cos(Math.Acos(-x)));
                Assert.AreEqual((float)-x,
                                Math.Cos(Math.Acos((float)-x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Acos(x);
                Assert.AreEqual(float2(expected), Math.Acos(float2(x)));
                Assert.AreEqual(float3(expected), Math.Acos(float3(x)));
                Assert.AreEqual(float4(expected), Math.Acos(float4(x)));
            }
        }

        [Test]
        public void Atan()
        {
            // atan(0) = 0
            Assert.AreEqual(0.0, Math.Atan(0.0));
            Assert.AreEqual(0.0f, Math.Atan(0.0f));

            // atan(nan) = nan
            Assert.IsTrue(double.IsNaN(Math.Atan(double.NaN)));
            Assert.IsTrue(float.IsNaN(Math.Atan(float.NaN)));

            // atan(-∞) = -π / 2
            Assert.AreEqual(-Math.PI / 2, Math.Atan(double.NegativeInfinity));
            Assert.AreEqual((float)(-Math.PI / 2), Math.Atan(float.NegativeInfinity));

            // atan(∞) = π / 2
            Assert.AreEqual(Math.PI / 2, Math.Atan(double.PositiveInfinity));
            Assert.AreEqual((float)(Math.PI / 2), Math.Atan(float.PositiveInfinity));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();

                // tan(atan(θ)) = θ
                Assert.AreEqual(x,
                                Math.Tan(Math.Atan(x)));
                Assert.AreEqual((float)x,
                                Math.Tan(Math.Atan((float)x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Atan(x);
                Assert.AreEqual(float2(expected), Math.Atan(float2(x)));
                Assert.AreEqual(float3(expected), Math.Atan(float3(x)));
                Assert.AreEqual(float4(expected), Math.Atan(float4(x)));
            }
        }

        [Test]
        public void Pow()
        {
            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();

                // verify identity x ^ 0 = 1
                Assert.AreEqual(1.0, Math.Pow(x, 0.0));
                Assert.AreEqual(1.0f, Math.Pow((float)x, 0.0f));

                // verify identity x ^ 1 = x
                Assert.AreEqual(x, Math.Pow(x, 1));
                Assert.AreEqual((float)x, Math.Pow((float)x, 1.0f));

                // verify identity x ^ 2 = x * x
                Assert.AreEqual(x * x, Math.Pow(x, 2.0));
                Assert.AreEqual((float)(x * x), Math.Pow(x, 2.0f));
            }

            for (int i = 0; i < 1000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();
                var a = random.NextDouble();
                var b = random.NextDouble();

                // verify identity x ^ a * x ^ b = x ^ (a + b)
                Assert.AreEqual(Math.Pow(x, a) * Math.Pow(x, b),
                                Math.Pow(x, a + b));
                Assert.AreEqual(Math.Pow((float)x, (float)a) * Math.Pow((float)x, (float)b),
                                Math.Pow((float)x, (float)(a + b)));

                // verify identity (x ^ a) ^ b = x ^ (a * b)
                Assert.AreEqual(Math.Pow(Math.Pow(x, a), b),
                                Math.Pow(x, a * b));
                Assert.AreEqual(Math.Pow(Math.Pow((float)x, (float)a), (float)b),
                                Math.Pow((float)x, (float)(a * b)));

                // verify identity x ^ a * y ^ a = (x * y) ^ a
                Assert.AreEqual(Math.Pow(x, a) * Math.Pow(y, a),
                                Math.Pow(x * y, a));
                Assert.AreEqual(Math.Pow((float)x, (float)a) * Math.Pow((float)y, (float)a),
                                Math.Pow((float)(x * y), (float)a));

                // verify identity x ^ -a = 1 / x ^ a
                Assert.AreEqual(Math.Pow(x, -a),
                                1.0 / Math.Pow(x, a), 1e-4);
                Assert.AreEqual(Math.Pow((float)x, (float)-a),
                                1.0f / Math.Pow((float)x, (float)a), 1e-4);

                // verify identity x ^ (a - b) = x ^ a / x ^ b
                Assert.AreEqual(Math.Pow(x, a - b), Math.Pow(x, a) / Math.Pow(x, b));
                Assert.AreEqual(Math.Pow((float)x, (float)(a - b)),
                                Math.Pow((float)x, (float)a) / Math.Pow((float)x, (float)b));

                // verify identity exp(log(x) * a) = x ^ a
                Assert.AreEqual(Math.Exp(Math.Log(x) * a),
                                Math.Pow(x, a));
                Assert.AreEqual(Math.Exp(Math.Log((float)x) * (float)a),
                                Math.Pow((float)x, (float)a));
            }

            // verify range
            for (int i = 0; i < 63; ++i)
            {
                for (int j = 0; j < 100; ++j)
                {
                    var significand = 1.0 + (float)random.NextDouble();
                    var x = significand * (1L << i);
                    var a = (float)random.NextDouble();

                    var expected = Math.Exp(Math.Log(x) * a);
                    var doubleTolerance = Math.Max(1e-12, Math.Abs(expected * 1e-12));
                    var floatTolerance = (float)Math.Max(1e-6, Math.Abs(expected * 1e-6));

                    Assert.AreEqual(expected, Math.Pow(x, a), doubleTolerance);
                    Assert.AreEqual((float)expected, Math.Pow((float)x, (float)a), floatTolerance);
                }
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var a = (float)random.NextDouble();

                var expected = Math.Pow(x, a);
                Assert.AreEqual(float2(expected), Math.Pow(float2(x), float2(a)));
                Assert.AreEqual(float3(expected), Math.Pow(float3(x), float3(a)));
                Assert.AreEqual(float4(expected), Math.Pow(float4(x), float4(a)));
            }
        }

        [Test]
        public void Exp()
        {
            // e ^ 0 = 0
            Assert.AreEqual(1.0, Math.Exp(0.0));
            Assert.AreEqual(1.0f, Math.Exp(0.0f));

            // e ^ 1 = e
            Assert.AreEqual(Math.E, Math.Exp(1.0));
            Assert.AreEqual((float)Math.E, Math.Exp(1.0f));

            // e ^ 2 = e * e
            Assert.AreEqual(Math.E * Math.E, Math.Exp(2.0));
            Assert.AreEqual((float)(Math.E * Math.E), Math.Exp(2.0f));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                // e ^ (x + y) = (e ^ x) * (e ^ y)
                Assert.AreEqual(Math.Exp(x + y),
                                Math.Exp(x) * Math.Exp(y));
                Assert.AreEqual(Math.Exp((float)(x + y)),
                                Math.Exp((float)x) * Math.Exp((float)y));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Exp(x);
                Assert.AreEqual(float2(expected), Math.Exp(float2(x)));
                Assert.AreEqual(float3(expected), Math.Exp(float3(x)));
                Assert.AreEqual(float4(expected), Math.Exp(float4(x)));
            }
        }

        [Test]
        public void Log()
        {
            // log(0) = -∞
            Assert.IsTrue(double.IsNegativeInfinity(Math.Log(0.0)));
            Assert.IsTrue(float.IsNegativeInfinity(Math.Log(0.0f)));

            // log(∞) = ∞
            Assert.IsTrue(double.IsPositiveInfinity(Math.Log(double.PositiveInfinity)));
            Assert.IsTrue(float.IsPositiveInfinity(Math.Log(float.PositiveInfinity)));

            // log(-1) = NaN
            Assert.IsTrue(double.IsNaN(Math.Log(-1.0)));
            Assert.IsTrue(float.IsNaN(Math.Log(-1.0f)));

            // log(NaN) = NaN
            Assert.IsTrue(double.IsNaN(double.NaN));
            Assert.IsTrue(float.IsNaN(float.NaN));

            // log(1) = 0
            Assert.AreEqual(0.0, Math.Log(1.0));
            Assert.AreEqual(0.0f, Math.Log(1.0f));

            // log(e) = 1
            Assert.AreEqual(1.0, Math.Log(Math.E));
            Assert.AreEqual(1.0f, Math.Log((float)Math.E));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                // log(x * y) = log(x) + log(y)
                Assert.AreEqual(Math.Log(x * y),
                                Math.Log(x) + Math.Log(y));
                Assert.AreEqual(Math.Log((float)(x * y)),
                                Math.Log((float)x) + Math.Log((float)y));

                // log(x / y) = log(x) - log(y)
                Assert.AreEqual(Math.Log(x / y),
                                Math.Log(x) - Math.Log(y));
                Assert.AreEqual(Math.Log((float)(x / y)),
                                Math.Log((float)x) - Math.Log((float)y));

                // log(e ^ x) = x
                Assert.AreEqual(x, Math.Log(Math.Exp(x)));
                Assert.AreEqual((float)x, Math.Log((float)Math.Exp(x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Log(x);
                Assert.AreEqual(float2(expected), Math.Log(float2(x)));
                Assert.AreEqual(float3(expected), Math.Log(float3(x)));
                Assert.AreEqual(float4(expected), Math.Log(float4(x)));
            }
        }

        [Test]
        public void Exp2()
        {
            // 2 ^ 0 = 0
            Assert.AreEqual(1.0, Math.Exp2(0.0));
            Assert.AreEqual(1.0f, Math.Exp2(0.0f));

            // 2 ^ 1 = 2
            Assert.AreEqual(2.0, Math.Exp2(1.0));
            Assert.AreEqual(2.0f, Math.Exp2(1.0f));

            // e ^ 2 = e * e
            Assert.AreEqual(4.0, Math.Exp2(2.0));
            Assert.AreEqual(4.0f, Math.Exp2(2.0f));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                // e ^ (x + y) = (e ^ x) * (e ^ y)
                Assert.AreEqual(Math.Exp2(x + y),
                                Math.Exp2(x) * Math.Exp2(y));
                Assert.AreEqual(Math.Exp2((float)(x + y)),
                                Math.Exp2((float)x) * Math.Exp2((float)y));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Exp2(x);
                Assert.AreEqual(float2(expected), Math.Exp2(float2(x)));
                Assert.AreEqual(float3(expected), Math.Exp2(float3(x)));
                Assert.AreEqual(float4(expected), Math.Exp2(float4(x)));
            }
        }


        [Test]
        public void Log2()
        {
            // log(0) = -∞
            Assert.IsTrue(double.IsNegativeInfinity(Math.Log2(0.0)));
            Assert.IsTrue(float.IsNegativeInfinity(Math.Log2(0.0f)));

            // log(∞) = ∞
            Assert.IsTrue(double.IsPositiveInfinity(Math.Log2(double.PositiveInfinity)));
            Assert.IsTrue(float.IsPositiveInfinity(Math.Log2(float.PositiveInfinity)));

            // log(-1) = NaN
            Assert.IsTrue(double.IsNaN(Math.Log2(-1.0)));
            Assert.IsTrue(float.IsNaN(Math.Log2(-1.0f)));

            // log(NaN) = NaN
            Assert.IsTrue(double.IsNaN(double.NaN));
            Assert.IsTrue(float.IsNaN(float.NaN));

            // log(1) = 0
            Assert.AreEqual(0.0, Math.Log2(1.0));
            Assert.AreEqual(0.0f, Math.Log2(1.0f));

            // log(e) = 1
            Assert.AreEqual(1.0, Math.Log2(2.0));
            Assert.AreEqual(1.0f, Math.Log2(2.0f));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                // log(x * y) = log(x) + log(y)
                Assert.AreEqual(Math.Log2(x * y),
                                Math.Log2(x) + Math.Log2(y));
                Assert.AreEqual(Math.Log2((float)(x * y)),
                                Math.Log2((float)x) + Math.Log2((float)y));

                // log(x / y) = log(x) - log(y)
                Assert.AreEqual(Math.Log2(x / y),
                                Math.Log2(x) - Math.Log2(y));
                Assert.AreEqual(Math.Log2((float)(x / y)),
                                Math.Log2((float)x) - Math.Log2((float)y));

                // log(e ^ x) = x
                Assert.AreEqual(x, Math.Log2(Math.Exp2(x)));
                Assert.AreEqual((float)x, Math.Log2((float)Math.Exp2(x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Log2(x);
                Assert.AreEqual(float2(expected), Math.Log2(float2(x)));
                Assert.AreEqual(float3(expected), Math.Log2(float3(x)));
                Assert.AreEqual(float4(expected), Math.Log2(float4(x)));
            }
        }

        [Test]
        public void Log10()
        {
            // log(0) = -∞
            Assert.IsTrue(double.IsNegativeInfinity(Math.Log10(0.0)));

            // log(∞) = ∞
            Assert.IsTrue(double.IsPositiveInfinity(Math.Log10(double.PositiveInfinity)));

            // log(-1) = NaN
            Assert.IsTrue(double.IsNaN(Math.Log10(-4.940656E-324)));

            // log(NaN) = NaN
            Assert.IsTrue(double.IsNaN(double.NaN));

            // log(10 ^ 1..3) = 1..3
            Assert.AreEqual(0.0, Math.Log10(1.0));
            Assert.AreEqual(1.0, Math.Log10(10.0));
            Assert.AreEqual(2.0, Math.Log10(100.0));

            Assert.AreEqual(308.254715559917, Math.Log10(double.MaxValue));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                // log(x * y) = log(x) + log(y)
                Assert.AreEqual(Math.Log10(x * y),
                                Math.Log10(x) + Math.Log10(y));

                // log(x / y) = log(x) - log(y)
                Assert.AreEqual(Math.Log10(x / y),
                                Math.Log10(x) - Math.Log10(y));

                // log(e ^ x) = x
                Assert.AreEqual(x, Math.Log10(Math.Pow(10, x)));
            }
        }

        [Test]
        public void DoubleSqrt()
        {
            Assert.AreEqual(0.0, Math.Sqrt(0.0));
            Assert.AreEqual(0.0f, Math.Sqrt(0.0f));

            Assert.IsTrue(double.IsNaN(Math.Sqrt(-1.0)));
            Assert.IsTrue(float.IsNaN(Math.Sqrt(-1.0f)));

            Assert.AreEqual(1.0, Math.Sqrt(1.0));
            Assert.AreEqual(1.0f, Math.Sqrt(1.0f));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var x = random.NextDouble() * 100.0;

                // sqrt(x^2) = x
                Assert.AreEqual(x, Math.Sqrt(x * x));
                Assert.AreEqual((float)x, Math.Sqrt((float)(x * x)));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var x = (float)random.NextDouble();
                var expected = Math.Sqrt(x);
                Assert.AreEqual(float2(expected), Math.Sqrt(float2(x)));
                Assert.AreEqual(float3(expected), Math.Sqrt(float3(x)));
                Assert.AreEqual(float4(expected), Math.Sqrt(float4(x)));
            }
        }

        [Test]
        public void DoubleAbs()
        {
            double a = -100.0;
            Assert.AreEqual(100.0, Math.Abs(a));

            double b = 100.0;
            Assert.AreEqual(100.0, Math.Abs(b));
        }

        [Test]
        public void FloatAbs()
        {
            float a = -100f;
            Assert.AreEqual(100f, Math.Abs(a));

            float b = 100f;
            Assert.AreEqual(100f, Math.Abs(b));
        }

        [Test]
        public void Float2Abs()
        {
            float2 a = float2(-100f);
            Assert.AreEqual(float2(100f), Math.Abs(a));

            float2 b = float2(100f);
            Assert.AreEqual(float2(100f), Math.Abs(b));
        }


        [Test]
        public void Float3Abs()
        {
            float3 a = float3(-100f);
            Assert.AreEqual(float3(100f), Math.Abs(a));

            float3 b = float3(100f);
            Assert.AreEqual(float3(100f), Math.Abs(b));
        }

        [Test]
        public void Float4Abs()
        {
            float4 a = float4(-100f);
            Assert.AreEqual(float4(100f), Math.Abs(a));

            float4 b = float4(100f);
            Assert.AreEqual(float4(100f), Math.Abs(b));
        }

        [Test]
        public void IntAbs()
        {
            Assert.AreEqual((sbyte)100, Math.Abs((sbyte)-100));
            Assert.AreEqual((sbyte)100, Math.Abs((sbyte)100));

            Assert.AreEqual((short)100, Math.Abs((short)-100));
            Assert.AreEqual((short)100, Math.Abs((short)100));

            Assert.AreEqual(100, Math.Abs(-100));
            Assert.AreEqual(100, Math.Abs(100));

            Assert.AreEqual(100L, Math.Abs(-100L));
            Assert.AreEqual(100L, Math.Abs(100L));

            Assert.Throws<OverflowException>(() => Math.Abs(sbyte.MinValue));
            Assert.AreEqual(sbyte.MaxValue, Math.Abs(sbyte.MinValue + 1));

            Assert.Throws<OverflowException>(() => Math.Abs(short.MinValue));
            Assert.AreEqual(short.MaxValue, Math.Abs(short.MinValue + 1));

            Assert.Throws<OverflowException>(() => Math.Abs(int.MinValue));
            Assert.AreEqual(int.MaxValue, Math.Abs(int.MinValue + 1));

            Assert.Throws<OverflowException>(() => Math.Abs(long.MinValue));
            Assert.AreEqual(long.MaxValue, Math.Abs(long.MinValue + 1));
        }

        [Test]
        public void Int2Abs()
        {
            int2 a = int2(-100);
            Assert.AreEqual(int2(100), Math.Abs(a));

            int2 b = int2(100);
            Assert.AreEqual(int2(100), Math.Abs(b));
        }

        [Test]
        public void Int3Abs()
        {
            int3 a = int3(-100);
            Assert.AreEqual(int3(100), Math.Abs(a));

            int3 b = int3(100);
            Assert.AreEqual(int3(100), Math.Abs(b));
        }

        [Test]
        public void Int4Abs()
        {
            int4 a = int4(-100);
            Assert.AreEqual(int4(100), Math.Abs(a));

            int4 b = int4(100);
            Assert.AreEqual(int4(100), Math.Abs(b));
        }

        [Test]
        public void DoubleSign()
        {
            double x = -100.0;
            Assert.AreEqual(-1.0, Math.Sign(x));

            double y = 100.0;
            Assert.AreEqual(1.0, Math.Sign(y));

            double z = 0.0;
            Assert.AreEqual(0.0, Math.Sign(z));
        }

        [Test]
        public void FloatSign()
        {
            float x = -100.0f;
            Assert.AreEqual(-1.0f, Math.Sign(x));

            float y = 100.0f;
            Assert.AreEqual(1.0f, Math.Sign(y));

            float z = 0.0f;
            Assert.AreEqual(0.0f, Math.Sign(z));
        }

        [Test]
        public void Float2Sign()
        {
            float2 x = float2(-100.0f, 100.0f);
            Assert.AreEqual(float2(-1.0f, 1.0f), Math.Sign(x));

            float2 y = float2(100.0f, -100.0f);
            Assert.AreEqual(float2(1.0f, -1.0f), Math.Sign(y));

            float2 z = float2(0.0f);
            Assert.AreEqual(float2(0.0f), Math.Sign(z));
        }

        [Test]
        public void Float3Sign()
        {
            float3 x = float3(-100.0f, 100.0f, -100.0f);
            Assert.AreEqual(float3(-1.0f, 1.0f, -1.0f), Math.Sign(x));

            float3 y = float3(100.0f, -100.0f, 100.0f);
            Assert.AreEqual(float3(1.0f, -1.0f, 1.0f), Math.Sign(y));

            float3 z = float3(0.0f);
            Assert.AreEqual(float3(0.0f), Math.Sign(z));
        }

        [Test]
        public void Float4Sign()
        {
            float4 x = float4(-100.0f, 100.0f, -100.0f, 100.0f);
            Assert.AreEqual(float4(-1.0f, 1.0f, -1.0f, 1.0f), Math.Sign(x));

            float4 y = float4(100.0f, -100.0f, 100.0f, -100.0f);
            Assert.AreEqual(float4(1.0f, -1.0f, 1.0f, -1.0f), Math.Sign(y));

            float4 z = float4(0.0f);
            Assert.AreEqual(float4(0.0f), Math.Sign(z));
        }

        [Test]
        public void IntSign()
        {
            int x = -100;
            Assert.AreEqual(-1, Math.Sign(x));

            int y = 100;
            Assert.AreEqual(1, Math.Sign(y));

            int z = 0;
            Assert.AreEqual(0, Math.Sign(z));
        }

        [Test]
        public void Int2Sign()
        {
            int2 x = int2(-100, 100);
            Assert.AreEqual(int2(-1, 1), Math.Sign(x));

            int2 y = int2(100, -100);
            Assert.AreEqual(int2(1, -1), Math.Sign(y));

            int2 z = int2(0);
            Assert.AreEqual(int2(0), Math.Sign(z));
        }

        [Test]
        public void Int3Sign()
        {
            int3 x = int3(-100, 100, -100);
            Assert.AreEqual(int3(-1, 1, -1), Math.Sign(x));

            int3 y = int3(100, -100, 100);
            Assert.AreEqual(int3(1, -1, 1), Math.Sign(y));

            int3 z = int3(0);
            Assert.AreEqual(int3(0), Math.Sign(z));
        }

        [Test]
        public void Int4Sign()
        {
            int4 x = int4(-100, 100, -100, 100);
            Assert.AreEqual(int4(-1, 1, -1, 1), Math.Sign(x));

            int4 y = int4(100, -100, 100, -100);
            Assert.AreEqual(int4(1, -1, 1, -1), Math.Sign(y));

            int4 z = int4(0);
            Assert.AreEqual(int4(0), Math.Sign(z));
        }

        [Test]
        public void DoubleFloor()
        {
            double x = 1.1;
            Assert.AreEqual(1.0, Math.Floor(x));

            double y = 1.99;
            Assert.AreEqual(1.0, Math.Floor(y));

            double z = -1.99;
            Assert.AreEqual(-2.0, Math.Floor(z));

            double w = 1.0;
            Assert.AreEqual(1.0, Math.Floor(w));
        }

        [Test]
        public void FloatFloor()
        {
            float x = 1.1f;
            Assert.AreEqual(1.0f, Math.Floor(x));

            float y = 1.99f;
            Assert.AreEqual(1.0f, Math.Floor(y));

            float z = -1.99f;
            Assert.AreEqual(-2.0f, Math.Floor(z));

            float w = 1.0f;
            Assert.AreEqual(1.0f, Math.Floor(w));
        }

        [Test]
        public void Float2Floor()
        {
            float2 x = float2(1.1f);
            Assert.AreEqual(float2(1.0f), Math.Floor(x));

            float2 y = float2(1.99f);
            Assert.AreEqual(float2(1.0f), Math.Floor(y));

            float2 z = float2(-1.99f);
            Assert.AreEqual(float2(-2.0f), Math.Floor(z));

            float2 w = float2(1.0f);
            Assert.AreEqual(float2(1.0f), Math.Floor(w));
        }

        [Test]
        public void Float3Floor()
        {
            float3 x = float3(1.1f);
            Assert.AreEqual(float3(1.0f), Math.Floor(x));

            float3 y = float3(1.99f);
            Assert.AreEqual(float3(1.0f), Math.Floor(y));

            float3 z = float3(-1.99f);
            Assert.AreEqual(float3(-2.0f), Math.Floor(z));

            float3 w = float3(1.0f);
            Assert.AreEqual(float3(1.0f), Math.Floor(w));
        }

        [Test]
        public void Float4Floor()
        {
            float4 x = float4(1.1f);
            Assert.AreEqual(float4(1.0f), Math.Floor(x));

            float4 y = float4(1.99f);
            Assert.AreEqual(float4(1.0f), Math.Floor(y));

            float4 z = float4(-1.99f);
            Assert.AreEqual(float4(-2.0f), Math.Floor(z));

            float4 w = float4(1.0f);
            Assert.AreEqual(float4(1.0f), Math.Floor(w));
        }

        [Test]
        public void DoubleCeil()
        {

            double x = 1.1;
            Assert.AreEqual(2.0, Math.Ceil(x));

            double y = 1.99;
            Assert.AreEqual(2.0, Math.Ceil(y));

            double z = -1.99;
            Assert.AreEqual(-1.0, Math.Ceil(z));

            double w = 1.0;
            Assert.AreEqual(1.0, Math.Ceil(w));

        }

        [Test]
        public void FloatCeil()
        {
            float x = 1.1f;
            Assert.AreEqual(2.0f, Math.Ceil(x));

            float y = 1.99f;
            Assert.AreEqual(2.0f, Math.Ceil(y));

            float z = -1.99f;
            Assert.AreEqual(-1.0f, Math.Ceil(z));

            float w = 1.0f;
            Assert.AreEqual(1.0f, Math.Ceil(w));
        }

        [Test]
        public void Float2Ceil()
        {
            float2 x = float2(1.1f);
            Assert.AreEqual(float2(2.0f), Math.Ceil(x));

            float2 y = float2(1.99f);
            Assert.AreEqual(float2(2.0f), Math.Ceil(y));

            float2 z = float2(-1.99f);
            Assert.AreEqual(float2(-1.0f), Math.Ceil(z));

            float2 w = float2(1.0f);
            Assert.AreEqual(float2(1.0f), Math.Ceil(w));
        }

        [Test]
        public void Float3Ceil()
        {
            float3 x = float3(1.1f);
            Assert.AreEqual(float3(2.0f), Math.Ceil(x));

            float3 y = float3(1.99f);
            Assert.AreEqual(float3(2.0f), Math.Ceil(y));

            float3 z = float3(-1.99f);
            Assert.AreEqual(float3(-1.0f), Math.Ceil(z));

            float3 w = float3(1.0f);
            Assert.AreEqual(float3(1.0f), Math.Ceil(w));
        }

        [Test]
        public void Float4Ceil()
        {
            float4 x = float4(1.1f);
            Assert.AreEqual(float4(2.0f), Math.Ceil(x));

            float4 y = float4(1.99f);
            Assert.AreEqual(float4(2.0f), Math.Ceil(y));

            float4 z = float4(-1.99f);
            Assert.AreEqual(float4(-1.0f), Math.Ceil(z));

            float4 w = float4(1.0f);
            Assert.AreEqual(float4(1.0f), Math.Ceil(w));
        }

        [Test]
        public void DoubleFract()
        {
            double x = 1.1;
            Assert.AreEqual(0.1, Math.Fract(x));

            double y = 1.99;
            Assert.AreEqual(0.99, Math.Fract(y));

            double z = -1.99;
            Assert.AreEqual(0.01, Math.Fract(z));

            double w = 1.0;
            Assert.AreEqual(0.0, Math.Fract(w));
        }

        [Test]
        public void FloatFract()
        {
            float x = 1.1f;
            Assert.AreEqual(0.1f, Math.Fract(x));

            float y = 1.99f;
            Assert.AreEqual(0.99f, Math.Fract(y));

            float z = -1.99f;
            Assert.AreEqual(0.01f, Math.Fract(z));

            float w = 1.0f;
            Assert.AreEqual(0.0f, Math.Fract(w));
        }

        [Test]
        public void Float2Fract()
        {
            float2 x = float2(1.1f);
            Assert.AreEqual(float2(0.1f), Math.Fract(x));

            float2 y = float2(1.99f);
            Assert.AreEqual(float2(0.99f), Math.Fract(y));

            float2 z = float2(-1.99f);
            Assert.AreEqual(float2(0.01f), Math.Fract(z));

            float2 w = float2(1.0f);
            Assert.AreEqual(float2(0.0f), Math.Fract(w));
        }

        [Test]
        public void Float3Fract()
        {
            float3 x = float3(1.1f);
            Assert.AreEqual(float3(0.1f), Math.Fract(x));

            float3 y = float3(1.99f);
            Assert.AreEqual(float3(0.99f), Math.Fract(y));

            float3 z = float3(-1.99f);
            Assert.AreEqual(float3(0.01f), Math.Fract(z));

            float3 w = float3(1.0f);
            Assert.AreEqual(float3(0.0f), Math.Fract(w));
        }

        [Test]
        public void Float4Fract()
        {
            float4 x = float4(1.1f);
            Assert.AreEqual(float4(0.1f), Math.Fract(x));

            float4 y = float4(1.99f);
            Assert.AreEqual(float4(0.99f), Math.Fract(y));

            float4 z = float4(-1.99f);
            Assert.AreEqual(float4(0.01f), Math.Fract(z));

            float4 w = float4(1.0f);
            Assert.AreEqual(float4(0.0f), Math.Fract(w));
        }

        ////
        ////
        ////
        ////
        ////
        ////
        ////    TODO:
        ////    MAKE TEST FOR Math.Mod
        ////
        ////
        ////
        ////
        ////



        [Test]
        public void DoubleMax()
        {
            double x = 99.999;
            double y = 99.9999;
            Assert.AreEqual(y, Math.Max(x, y));

            double z = -0.0001;
            double w = -0.0002;
            Assert.AreEqual(z, Math.Max(z, w));
        }

        [Test]
        public void FloatMax()
        {
            float x = 99.999f;
            float y = 99.9999f;
            Assert.AreEqual(y, Math.Max(x, y));

            float z = -0.0001f;
            float w = -0.0002f;
            Assert.AreEqual(z, Math.Max(z, w));
        }

        [Test]
        public void Float2Max()
        {

            float2 x = float2(99.99f, 100.99f);
            float2 y = float2(100.99f, 99.99f);

            Assert.AreEqual(float2(100.99f, 100.99f), Math.Max(x, y));

            float2 z = float2(0.001f, 0.002f);
            float w = 0.0015f;

            Assert.AreEqual(float2(0.0015f, 0.002f), Math.Max(z, w));

        }

        [Test]
        public void Float3Max()
        {

            float3 x = float3(99.99f, 100.99f, 88.88f);
            float3 y = float3(100.99f, 99.99f, 88.88f);

            Assert.AreEqual(float3(100.99f, 100.99f, 88.88f), Math.Max(x, y));

            float3 z = float3(0.001f, 0.002f, 0.15f);
            float w = 0.0015f;

            Assert.AreEqual(float3(0.0015f, 0.002f, 0.15f), Math.Max(z, w));

        }

        [Test]
        public void Float4Max()
        {

            float4 x = float4(99.99f, 100.99f, 88.88f, 0.99f);
            float4 y = float4(100.99f, 99.99f, 88.88f, 0.999f);

            Assert.AreEqual(float4(100.99f, 100.99f, 88.88f, 0.999f), Math.Max(x, y));

            float4 z = float4(0.001f, 0.002f, 0.15f, 0.003f);
            float w = 0.0015f;

            Assert.AreEqual(float4(0.0015f, 0.002f, 0.15f, 0.003f), Math.Max(z, w));

        }

        [Test]
        public void IntMax()
        {
            int x = 99;
            int y = 100;
            Assert.AreEqual(y, Math.Max(x, y));

            int z = -1;
            int w = -2;
            Assert.AreEqual(z, Math.Max(z, w));
        }

        [Test]
        public void Int2Max()
        {

            int2 x = int2(99, 100);
            int2 y = int2(100, 99);

            Assert.AreEqual(int2(100, 100), Math.Max(x, y));

            int2 z = int2(1, 3);
            int w = 2;

            Assert.AreEqual(int2(2, 3), Math.Max(z, w));

        }

        [Test]
        public void Int3Max()
        {

            int3 x = int3(99, 100, 88);
            int3 y = int3(100, 99, 88);

            Assert.AreEqual(int3(100, 100, 88), Math.Max(x, y));

            int3 z = int3(1, 2, 3);
            int w = 2;

            Assert.AreEqual(int3(2, 2, 3), Math.Max(z, w));

        }

        [Test]
        public void Int4Max()
        {

            int4 x = int4(99, 100, 88, 1);
            int4 y = int4(100, 99, 88, 1);

            Assert.AreEqual(int4(100, 100, 88, 1), Math.Max(x, y));

            int4 z = int4(1, 2, 3, 4);
            int w = 2;

            Assert.AreEqual(int4(2, 2, 3, 4), Math.Max(z, w));

        }

        [Test]
        public void DoubleMin()
        {
            double x = 99.999;
            double y = 99.9999;
            Assert.AreEqual(x, Math.Min(x, y));

            double z = -0.0001;
            double w = -0.0002;
            Assert.AreEqual(w, Math.Min(z, w));
        }


        [Test]
        public void FloatMin()
        {
            float x = 99.999f;
            float y = 99.9999f;
            Assert.AreEqual(x, Math.Min(x, y));

            float z = -0.0001f;
            float w = -0.0002f;
            Assert.AreEqual(w, Math.Min(z, w));
        }

        [Test]
        public void Float2Min()
        {

            float2 x = float2(99.99f, 100.99f);
            float2 y = float2(100.99f, 99.99f);

            Assert.AreEqual(float2(99.99f, 99.99f), Math.Min(x, y));

            float2 z = float2(0.001f, 0.002f);
            float w = 0.0015f;

            Assert.AreEqual(float2(0.001f, 0.0015f), Math.Min(z, w));

        }

        [Test]
        public void Float3Min()
        {

            float3 x = float3(99.99f, 100.99f, 88.88f);
            float3 y = float3(100.99f, 99.99f, 88.88f);

            Assert.AreEqual(float3(99.99f, 99.99f, 88.88f), Math.Min(x, y));

            float3 z = float3(0.001f, 0.002f, 0.15f);
            float w = 0.0015f;

            Assert.AreEqual(float3(0.001f, 0.0015f, 0.0015f), Math.Min(z, w));

        }

        [Test]
        public void Float4Min()
        {

            float4 x = float4(99.99f, 100.99f, 88.88f, 0.99f);
            float4 y = float4(100.99f, 99.99f, 88.88f, 0.999f);

            Assert.AreEqual(float4(99.99f, 99.99f, 88.88f, 0.99f), Math.Min(x, y));

            float4 z = float4(0.001f, 0.002f, 0.15f, 0.003f);
            float w = 0.0015f;

            Assert.AreEqual(float4(0.001f, 0.0015f, 0.0015f, 0.0015f), Math.Min(z, w));

        }

        [Test]
        public void IntMin()
        {
            int x = 99;
            int y = 100;
            Assert.AreEqual(x, Math.Min(x, y));

            int z = -1;
            int w = -2;
            Assert.AreEqual(w, Math.Min(z, w));
        }

        [Test]
        public void Int2Min()
        {

            int2 x = int2(99, 100);
            int2 y = int2(100, 99);

            Assert.AreEqual(int2(99, 99), Math.Min(x, y));

            int2 z = int2(1, 3);
            int w = 2;

            Assert.AreEqual(int2(1, 2), Math.Min(z, w));

        }

        [Test]
        public void Int3Min()
        {

            int3 x = int3(99, 100, 88);
            int3 y = int3(100, 99, 88);

            Assert.AreEqual(int3(99, 99, 88), Math.Min(x, y));

            int3 z = int3(1, 2, 3);
            int w = 2;

            Assert.AreEqual(int3(1, 2, 2), Math.Min(z, w));

        }

        [Test]
        public void Int4Min()
        {

            int4 x = int4(99, 100, 88, 1);
            int4 y = int4(100, 99, 88, 1);

            Assert.AreEqual(int4(99, 99, 88, 1), Math.Min(x, y));

            int4 z = int4(1, 2, 3, 4);
            int w = 2;

            Assert.AreEqual(int4(1, 2, 2, 2), Math.Min(z, w));

        }

        [Test]
        public void DoubleClamp()
        {
            double min = 1.0;
            double max = 2.0;

            double x = 2.11119;
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            double y = 0.999;
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            double z = 1.5;
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void FloatClamp()
        {
            float min = 1.0f;
            float max = 2.0f;

            float x = 2.11119f;
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            float y = 0.999f;
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            float z = 1.5f;
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Float2Clamp()
        {
            float2 min = float2(1.0f);
            float2 max = float2(2.0f);

            float2 x = float2(2.11119f);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            float2 y = float2(0.999f);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            float2 z = float2(1.5f);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Float3Clamp()
        {
            float3 min = float3(1.0f);
            float3 max = float3(2.0f);

            float3 x = float3(2.11119f);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            float3 y = float3(0.999f);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            float3 z = float3(1.5f);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Float4Clamp()
        {
            float4 min = float4(1.0f);
            float4 max = float4(2.0f);

            float4 x = float4(2.11119f);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            float4 y = float4(0.999f);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            float4 z = float4(1.5f);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }


        [Test]
        public void IntClamp()
        {
            int min = 1;
            int max = 3;

            int x = 4;
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            int y = 0;
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            int z = 2;
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Int2Clamp()
        {
            int2 min = int2(1);
            int2 max = int2(3);

            int2 x = int2(4);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            int2 y = int2(0);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            int2 z = int2(2);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Int3Clamp()
        {
            int3 min = int3(1);
            int3 max = int3(3);

            int3 x = int3(4);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            int3 y = int3(0);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            int3 z = int3(2);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Int4Clamp()
        {
            int4 min = int4(1);
            int4 max = int4(3);

            int4 x = int4(4);
            Assert.AreEqual(max, Math.Clamp(x, min, max));

            int4 y = int4(0);
            Assert.AreEqual(min, Math.Clamp(y, min, max));

            int4 z = int4(2);
            Assert.AreEqual(z, Math.Clamp(z, min, max));
        }

        [Test]
        public void Lerp()
        {
            Assert.AreEqual(0.5, Math.Lerp(0.0, 1.0, 0.5));
            Assert.AreEqual(0.5f, Math.Lerp(0.0f, 1.0f, 0.5f));

            var random = new Random(1337);
            for (int i = 0; i < 10000; ++i)
            {
                var a = random.NextDouble() * 100.0;
                var b = random.NextDouble() * 100.0;
                var x = random.NextDouble();

                // lerp(0, 1, x) = x
                Assert.AreEqual(x, Math.Lerp(0.0, 1.0, x));
                Assert.AreEqual((float)x, Math.Lerp(0.0f, 1.0f, (float)x));

                // lerp(a, b, 0) = a
                Assert.AreEqual(a, Math.Lerp(a, b, 0.0));
                Assert.AreEqual((float)a, Math.Lerp((float)a, (float)b, 0.0f));

                // lerp(a, b, 1) = b
                Assert.AreEqual(b, Math.Lerp(a, b, 1.0));
                Assert.AreEqual((float)b, Math.Lerp((float)a, (float)b, 1.0f));
            }

            // verify that vector versions work as expected
            for (int i = 0; i < 1000; ++i)
            {
                var a = (float)random.NextDouble();
                var b = (float)random.NextDouble();
                var x = (float)random.NextDouble();
                var expected = Math.Lerp(a, b, x);
                Assert.AreEqual(float2(expected), Math.Lerp(float2(a), float2(b), x));
                Assert.AreEqual(float3(expected), Math.Lerp(float3(a), float3(b), x));
                Assert.AreEqual(float4(expected), Math.Lerp(float4(a), float4(b), x));
            }
        }

        [Test]
        public void DoubleStep()
        {
            double x = 0.99;
            double y = 1.0;
            Assert.AreEqual(1.0, Math.Step(x, y));
            Assert.AreEqual(0.0, Math.Step(y, x));
        }

        [Test]
        public void FloatStep()
        {
            float x = 0.99f;
            float y = 1.0f;
            Assert.AreEqual(1.0f, Math.Step(x, y));
            Assert.AreEqual(0.0f, Math.Step(y, x));
        }

        [Test]
        public void Float2Step()
        {
            float2 x = float2(0.99f, 1.0f);
            float2 y = float2(1.0f, 0.99f);
            Assert.AreEqual(float2(1.0f, 0.0f), Math.Step(x, y));
            Assert.AreEqual(float2(0.0f, 1.0f), Math.Step(y, x));
        }

        [Test]
        public void Float3Step()
        {
            float3 x = float3(0.99f, 1.0f, 0.99f);
            float3 y = float3(1.0f, 0.99f, 1.0f);
            Assert.AreEqual(float3(1.0f, 0.0f, 1.0f), Math.Step(x, y));
            Assert.AreEqual(float3(0.0f, 1.0f, 0.0f), Math.Step(y, x));
        }

        [Test]
        public void Float4Step()
        {
            float4 x = float4(0.99f, 1.0f, 0.99f, 1.0f);
            float4 y = float4(1.0f, 0.99f, 1.0f, 0.99f);
            Assert.AreEqual(float4(1.0f, 0.0f, 1.0f, 0.0f), Math.Step(x, y));
            Assert.AreEqual(float4(0.0f, 1.0f, 0.0f, 1.0f), Math.Step(y, x));
        }

        [Test]
        public void DoubleSaturate()
        {
            double x = 1.1;
            Assert.AreEqual(1.0, Math.Saturate(x));

            double y = 0.5;
            Assert.AreEqual(y, Math.Saturate(y));

            double z = -1.0;
            Assert.AreEqual(0.0, Math.Saturate(z));
        }

        [Test]
        public void FloatSaturate()
        {
            float x = 1.1f;
            Assert.AreEqual(1.0f, Math.Saturate(x));

            float y = 0.5f;
            Assert.AreEqual(y, Math.Saturate(y));

            float z = -1.0f;
            Assert.AreEqual(0f, Math.Saturate(z));
        }

        [Test]
        public void Float2Saturate()
        {
            float2 x = float2(1.1f);
            Assert.AreEqual(float2(1.0f), Math.Saturate(x));

            float2 y = float2(0.5f);
            Assert.AreEqual(y, Math.Saturate(y));

            float2 z = float2(-1.0f);
            Assert.AreEqual(float2(0f), Math.Saturate(z));
        }

        [Test]
        public void Float3Saturate()
        {
            float3 x = float3(1.1f);
            Assert.AreEqual(float3(1.0f), Math.Saturate(x));

            float3 y = float3(0.5f);
            Assert.AreEqual(y, Math.Saturate(y));

            float3 z = float3(-1.0f);
            Assert.AreEqual(float3(0f), Math.Saturate(z));
        }


        [Test]
        public void Float4Saturate()
        {
            float4 x = float4(1.1f);
            Assert.AreEqual(float4(1.0f), Math.Saturate(x));

            float4 y = float4(0.5f);
            Assert.AreEqual(y, Math.Saturate(y));

            float4 z = float4(-1.0f);
            Assert.AreEqual(float4(0f), Math.Saturate(z));
        }

        [Test]
        public void Float2ComponentMax()
        {
            float2 x = float2(100.0f, 200.0f);
            Assert.AreEqual(200.0f, Math.ComponentMax(x));

            float2 y = float2(100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMax(y));
        }

        [Test]
        public void Float3ComponentMax()
        {
            float3 x = float3(100.0f, 200.0f, 300.0f);
            Assert.AreEqual(300.0f, Math.ComponentMax(x));

            float3 y = float3(100.0f, 100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMax(y));
        }

        [Test]
        public void Float4ComponentMax()
        {
            float4 x = float4(100.0f, 200.0f, 300.0f, 400.0f);
            Assert.AreEqual(400.0f, Math.ComponentMax(x));

            float4 y = float4(100.0f, 100.0f, 100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMax(y));
        }

        [Test]
        public void Int2ComponentMax()
        {
            int2 x = int2(100, 200);
            Assert.AreEqual(200, Math.ComponentMax(x));

            int2 y = int2(100, 100);
            Assert.AreEqual(100, Math.ComponentMax(y));
        }

        [Test]
        public void Int3ComponentMax()
        {
            int3 x = int3(100, 200, 300);
            Assert.AreEqual(300, Math.ComponentMax(x));

            int3 y = int3(100, 100, 100);
            Assert.AreEqual(100, Math.ComponentMax(y));
        }

        [Test]
        public void Int4ComponentMax()
        {
            int4 x = int4(100, 200, 300, 400);
            Assert.AreEqual(400, Math.ComponentMax(x));

            int4 y = int4(100, 100, 100, 100);
            Assert.AreEqual(100, Math.ComponentMax(y));
        }

        [Test]
        public void Float2ComponentMin()
        {
            float2 x = float2(100.0f, 200.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(x));

            float2 y = float2(100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(y));
        }

        [Test]
        public void Float3ComponentMin()
        {
            float3 x = float3(100.0f, 200.0f, 300.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(x));

            float3 y = float3(100.0f, 100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(y));
        }

        [Test]
        public void Float4ComponentMin()
        {
            float4 x = float4(100.0f, 200.0f, 300.0f, 400.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(x));

            float4 y = float4(100.0f, 100.0f, 100.0f, 100.0f);
            Assert.AreEqual(100.0f, Math.ComponentMin(y));
        }

        [Test]
        public void Int2ComponentMin()
        {
            int2 x = int2(100, 200);
            Assert.AreEqual(100, Math.ComponentMin(x));

            int2 y = int2(100, 100);
            Assert.AreEqual(100, Math.ComponentMin(y));
        }

        [Test]
        public void Int3ComponentMin()
        {
            int3 x = int3(100, 200, 300);
            Assert.AreEqual(100, Math.ComponentMin(x));

            int3 y = int3(100, 100, 100);
            Assert.AreEqual(100, Math.ComponentMin(y));
        }

        [Test]
        public void Int4ComponentMin()
        {
            int4 x = int4(100, 200, 300, 400);
            Assert.AreEqual(100, Math.ComponentMin(x));

            int4 y = int4(100, 100, 100, 100);
            Assert.AreEqual(100, Math.ComponentMin(y));
        }

        [Test]
        public void Float2ComponentSum()
        {
            float2 x = float2(100.0f, 200.0f);
            Assert.AreEqual(300.0f, Math.ComponentSum(x));

            float2 y = float2(-100.0f, 100.0f);
            Assert.AreEqual(0.0f, Math.ComponentSum(y));
        }

        [Test]
        public void Float3ComponentSum()
        {
            float3 x = float3(100.0f, 200.0f, 300.0f);
            Assert.AreEqual(600.0f, Math.ComponentSum(x));

            float3 y = float3(-100.0f, 0.0f, 100.0f);
            Assert.AreEqual(0.0f, Math.ComponentSum(y));
        }

        [Test]
        public void Float4ComponentSum()
        {
            float4 x = float4(100.0f, 200.0f, 300.0f, 400.0f);
            Assert.AreEqual(1000.0f, Math.ComponentSum(x));

            float4 y = float4(-50.0f, 50.0f, -50.0f, 50.0f);
            Assert.AreEqual(0.0f, Math.ComponentSum(y));
        }

        [Test]
        public void Int2ComponentSum()
        {
            int2 x = int2(100, 200);
            Assert.AreEqual(300, Math.ComponentSum(x));

            int2 y = int2(-100, 100);
            Assert.AreEqual(0, Math.ComponentSum(y));
        }

        [Test]
        public void Int3ComponentSum()
        {
            int3 x = int3(100, 200, 300);
            Assert.AreEqual(600, Math.ComponentSum(x));

            int3 y = int3(-100, 0, 100);
            Assert.AreEqual(0, Math.ComponentSum(y));
        }

        [Test]
        public void Int4ComponentSum()
        {
            int4 x = int4(100, 200, 300, 400);
            Assert.AreEqual(1000, Math.ComponentSum(x));

            int4 y = int4(-50, 50, -50, 50);
            Assert.AreEqual(0, Math.ComponentSum(y));
        }


    }

}
