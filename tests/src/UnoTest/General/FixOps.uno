using Uno;
using Uno.Collections;
using Uno.Testing;

namespace UnoTest.General
{
    public class IncOps
    {
        int _field = 0;
        int field { get { return _field; } set { _field = value; } }

        public class derp
        {
            public int derk = 0;

            [Test]
            public void Test()
            {
                Assert.AreEqual(0, derk);
                derk--;
                Assert.AreEqual(-1, derk);
                derk++;
                Assert.AreEqual(0, derk);
                derk++;
                Assert.AreEqual(1, derk);
                derk--;
                Assert.AreEqual(0, derk);
            }
        }
        static class berp
        {
            public static int berk = 0;
        }
        int returnInt(int a)
        {
            return a;
        }
        delegate int intDelegate(int i);

        int someMethod(int a)
        {
            Assert.AreEqual(5, a);
            a++;
            Assert.AreEqual(6, a);
            a--;
            return a;
        }

        [Test]
        //[Ignore()]
        [Ignore("#262", "webgl")]
        public void Run()
        {
            int a = 255;
            a++;
            Assert.AreEqual(256, a);
            a--;
            Assert.AreEqual(255, a);

            float b = 1.0f;
            b++;
            Assert.AreEqual(2.0f, b);
            b--;
            Assert.AreEqual(1.0f, b);

            short c = -1;
            c++;
            Assert.AreEqual(0, c);
            c--;
            Assert.AreEqual(-1, c);

            /*double d = 2.0;
            d++;
            Assert.AreEqual(3.0, d);
            d--;
            Assert.AreEqual(2.0, d));

            /*float2 e = float2(-1.0f, 2.0f);
            e++;
            Assert.AreEqual(float2(0.0f, 3.0f), e);
            e--;
            Assert.AreEqual(float2(-1.0f, 2.0f), e);*/

            byte f = 254;
            f++;
            Assert.AreEqual(255, f);
            f--;
            Assert.AreEqual(254, f);

            int g = -5;
            g++;
            Assert.AreEqual(-4, g);
            g--;
            Assert.AreEqual(-5, g);


            long h = -23;
            h++;
            Assert.AreEqual(-22, h);
            h--;
            Assert.AreEqual(-23, h);

            sbyte i = 127;
            i++;
            Assert.AreEqual(-128, i);
            i--;
            Assert.AreEqual(127, i);
            i--;
            Assert.AreEqual(126, i);

            short j = 45;
            j++;
            Assert.AreEqual(46, j);
            j--;
            Assert.AreEqual(45, j);

            uint k = 35;
            k++;
            Assert.AreEqual(36, k);
            k--;
            Assert.AreEqual(35, k);

            ulong l = 355;
            l++;
            Assert.AreEqual(356, l);
            l--;
            Assert.AreEqual(355, l);

            ushort m = 0;
            m++;
            Assert.AreEqual(1, m);
            m--;
            Assert.AreEqual(0, m);


            Assert.AreEqual(0, field);

            //field++; // causes error (#265)
            //Assert.AreEqual(1, field);
            //field--; // causes error (#265)
            //Assert.AreEqual(0, field);

            int[] aa = new int[3];
            aa[0] = 1;
            aa[1] = 2;
            aa[2] = 3;

            for(int ii=0; ii <= 2; ii++)
            {
                int bb = aa[ii]--;  // 0
                Assert.IsTrue(bb == ii + 1);
                bb = aa[ii]++;      // 1
                Assert.IsTrue(bb == ii + 0);
                bb = --aa[ii];      // 0
                Assert.IsTrue(bb == ii + 0);
                bb = ++aa[ii];      // 1
                Assert.IsTrue(bb == ii + 1);
            }
            var derp = new List<float2>();
            derp.Add(float2(1.0f,1.0f));
            derp.Add(float2(2.0f,2.0f));
            derp.Add(float2(3.0f,3.0f));
            derp.Add(float2(4.0f,4.0f));
            /*for(float ii = 0.0f; ii < 3.0f ; ii++)
            {
                float2 cc = derp[(int)ii++];
                Assert.IsTrue(cc == float2(++ii,++ii));
                cc = derp[(int)++ii];
                Assert.IsTrue(cc == float2(ii+2.0f,ii+2.0f));
                cc = derp[(int)ii++]--;
                Assert.IsTrue(cc == float2(++ii,++ii));
                cc = derp[(int)ii++]--;
                Assert.IsTrue(cc == float2(++ii,++ii));
                cc = --derp[(int)ii++];
                Assert.IsTrue(cc == float2(ii,ii));
                cc = ++derp[(int)ii];
                Assert.IsTrue(cc == float2(ii+2,ii+2));
                cc = derp[(int)ii]++;
                Assert.IsTrue(cc == float2(ii++,ii++));
            }
            derp[0].X++;
            Assert.AreEqual(2.0f, derp[0].X);
            Assert.AreEqual(1.0f, derp[0].Y);
            derp[0].X--;
            Assert.AreEqual(1.0f, derp[0].X);
            Assert.AreEqual(1.0f, derp[0].Y);
            */
            var dd = float3(1.0f,2.0f,3.0f);
            /*dd++;
            Assert.AreEqual(2.0f, dd.X);
            Assert.AreEqual(3.0f, dd.Y);
            Assert.AreEqual(4.0f, dd.Z);
            dd--;
            Assert.AreEqual(1.0f, dd.X);
            Assert.AreEqual(2.0f, dd.Y);
            Assert.AreEqual(3.0f, dd.Z);*/

            var ee = float4(1.0f,2.0f,3.0f,4.0f);
            /*ee++;
            Assert.AreEqual(2.0f, ee.X);
            Assert.AreEqual(ee.Y ,3.0f));
            Assert.AreEqual(ee.Z ,4.0f));
            Assert.AreEqual(ee.W ,5.0f));
            ee--;
            Assert.AreEqual(1.0f, ee.X);
            Assert.AreEqual(2.0f, ee.Y);
            Assert.AreEqual(3.0f, ee.Z);
            Assert.AreEqual(4.0f, ee.W);*/

            var ff = float3x3(float3(0.0f),float3(1.0f),float3(2.0f));
            /*ff++;
            Assert.AreEqual(1.0f, ff.M11);
            Assert.AreEqual(1.0f, ff.M12);
            Assert.AreEqual(1.0f, ff.M13);
            Assert.AreEqual(2.0f, ff.M21);
            Assert.AreEqual(2.0f, ff.M22);
            Assert.AreEqual(2.0f, ff.M23);
            Assert.AreEqual(3.0f, ff.M31);
            Assert.AreEqual(3.0f, ff.M32);
            Assert.AreEqual(3.0f, ff.M33);*/

            char gg = (char)65;
            gg++;
            Assert.AreEqual('B', gg);

            var hh = int2(1,2);
            //hh[0]++;
            //Assert.AreEqual(2, hh.X);
            hh.Y++;
            Assert.AreEqual(3, hh[1]);
            hh.Y--;
            Assert.AreEqual(2, hh[1]);

            var jj = new derp();
            jj.derk++;
            Assert.AreEqual(1, jj.derk);
            jj.derk--;
            Assert.AreEqual(0, jj.derk);
            jj.Test();

            int ab = 5;
            ab = returnInt(ab++);
            Assert.AreEqual(5, ab);
            ab = returnInt(++ab);
            Assert.AreEqual(6, ab);
            ab = returnInt(ab--);
            Assert.AreEqual(6, ab);
            ab = returnInt(--ab);
            Assert.AreEqual(5, ab);

            int[] ac = new int[2];
            ac[0] = 1;
            ab = returnInt(--ac[0]);
            Assert.AreEqual(0, ab);

            intDelegate intDel;
            intDel = returnInt;
            ab = 5;
            ab = intDel(++ab);
            Assert.AreEqual(6, ab);
            ac[0] = 1;
            ab = intDel(++ac[0]);
            Assert.AreEqual(2, ab);
            ab = 5;
            ab = intDel(--ab);
            Assert.AreEqual(4, ab);
            ac[0] = 1;
            ab = intDel(--ac[0]);
            Assert.AreEqual(0, ab);

            Assert.AreEqual(5, someMethod(5));
        }
    }
}
