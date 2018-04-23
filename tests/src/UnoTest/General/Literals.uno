using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Literals
    {
        [Test]
        public void Run()
        {
            // Hex lits

            const byte byteMin = 0x00;
            const byte byteMax = 0xff;

            const sbyte sbyteMin = -0x80;
            const sbyte sbyteMax =  0x7F;

            const short shortMin = -0x8000;
            const short shortMax =  0x7Fff;

            const ushort ushortMin = 0x0000;
            const ushort ushortMax = 0xffFF;

            const char charMinHex = (char)0x0000;
            const char charMaxHex = (char)0xFFFF;

            const int intMin = (int)-0x80000000;
            const int intMax = 0x7FFFFFFF;

            const uint uintMin = 0x00000000;
            const uint uintMax = 0xFFFFFFFF;

            const long longMin = -(long)(0x8000000000000000 - 1) - 1;
            const long longMax = 0x7FFFFFFFFFFFFFFF;

            const ulong ulongMin = 0x0000000000000000;
            const ulong ulongMax = 0xFFFFfFfffFFFffFF;


            // Invalid casts

            unchecked
            {
                const int intInvalid = (int)-0x80000001;
                const long longInvalid1 = (long)(1./0.);
                const long longInvalid2 = (long)(0./0.);
                const long longInvalid3 = (long)(0.0f / 0.0f);
                const long longInvalid4 = (long)(-1.0 / 0.0);
            }


            // Char lits

            const char charMinLit = '\0';
            const char charMinLit2 = '\x0';
            const char charMinLit3 = '\x00';
            const char charMinLit4 = '\x000';
            const char charMinLit5 = '\x0000';
            const char charMinLit6 = '\u0000';
            const char charMinLit7 = '\U00000000';
            const char charMaxLit = '\xffff';
            const char charMaxLit2 = '\uffff';
            const char charMaxLit3 = '\U0000ffff';

            var c = '\uff73';
            Assert.AreEqual('ｳ', c);
            var d = '\u8db3';
            Assert.AreEqual('足', d);
            var e = '\x05c';
            Assert.AreEqual('\\', e);
            var f = '\x65';
            Assert.AreEqual('e', f);

            // String literals
            const string strLit1 = "";
            Assert.AreEqual(0, strLit1.Length);
            const string strLit2 = "\x0";
            Assert.AreEqual(1, strLit2.Length);
            const string strLit3 = "\x00";
            Assert.AreEqual(1, strLit3.Length);
            const string strLit4 = "\x000";
            Assert.AreEqual(1, strLit4.Length);
            const string strLit5 = "\x0000";
            Assert.AreEqual(1, strLit5.Length);
            const string strLit6 = "\x0000f";
            Assert.AreEqual(2, strLit6.Length);
            const string strLit7 = "\uffff";
            Assert.AreEqual(1, strLit7.Length);
            const string strLit8 = "\U0010ffff";
            Assert.AreEqual(2, strLit8.Length);


            // Integral

            uint i1 = 34u;
            uint i2 = 399995U;
            ulong i3 = 87ul;
            ulong i4 = 8888555lu;
            ulong i5 = 84499Lu;
            ulong i6 = 93939944lU;
            ulong i7 = 939494991Ul;
            ulong i8 = 90234932uL;
            long i9 = 46666l;
            long i10 = 213444L;

            int i11 = -2147483648;
            long i12 = 2147483648;
            long i13 = -9223372036854775808;
            ulong i14 = 9223372036854775808;

            uint i15 = 0x34u;
            uint i16 = 0x39bba95U;
            ulong i17 = 0x87ul;
            ulong i18 = 0x8c88555lu;
            ulong i19 = 0x84499Lu;
            ulong i20 = 0x949aaf44lU;
            ulong i21 = 0x952423491Ul;
            ulong i22 = 0x90637bf32uL;
            long i23 = 0x46ee666l;
            long i24 = 0x2134f44L;


            // Floating point

            const float f1 = 0.157f;
            const float f2 = 0.157F;
            const float f3 = .157f;
            const float f4 = 157.f;
            const float f5 = 00157.f;
            const float f6 = 157f / 1000f;

            Assert.IsTrue(f1 == f6);

            const double d1 = 0.157;
            const double d2 = 0.157F;
            const double d3 = .157;
            const double d4 = 157.;
            const double d5 = 00157.;
            const double d6 = f5 * f3 - f1; // Evaluated compile time


            // NaN

            const float fNan1 = .0f/.0f;
            const float fNan2 = 0.f/.0f;

            const double dNan1 = .0/0.;
            const double dNan2 = .0/.0;


            // Infinity

            const float fInf1 = 1.f/0.f;
            const float fInf2 = -1.f/.0f;
            const float fInf3 = -1.f/-.0f;

            const double dInf1 = 1./0.;
            const double dInf2 = -1./.0;
            const double dInf3 = -1./-.0;

            float fNonConstNegInf = +1.0f / -0.0f;

            Assert.IsTrue(Math.PIf / 0 == fInf1);
            Assert.IsTrue(Math.PI / 0 == -fInf2);
            Assert.IsTrue(-d4 / -0x0 == fNonConstNegInf); // Note: (double)-0x0 == 0.0 (integers does not store sign bit)


            // Scientific notation

            const double dSn1 = -12e3;
            const double dSn2 = -12.0e3;
            const double dSn3 = +12.0e3;
            const double dSn4 = -0.12e3;
            const double dSn5 = -.12e3;
            const double dSn6 = 12e3;
            const double dSn7 = 12.0e3;
            const double dSn8 = +12.0e3;
            const double dSn9 = 0.12e3;
            const double dSn10 = .12e3;
            const double dSn11 = 12e+3;
            const double dSn12 = 12.0e+3;
            const double dSn13 = +12.0e+3;
            const double dSn14 = 0.12e+003;
            const double dSn15 = .12e+0003;
            const double dSn16 = 12e-3;
            const double dSn17 = 12.0e-3;
            const double dSn18 = +12.0e-3;
            const double dSn19 = 0.12e-003;
            const double dSn20 = .12e-0003;

            const float fSn1 = -12e3f;
            const float fSn2 = -12.0e3f;
            const float fSn3 = +12.0e3f;
            const float fSn4 = -0.12e3f;
            const float fSn5 = -.12e3f;
            const float fSn6 = 12e3f;
            const float fSn7 = 12.0e3F;
            const float fSn8 = +12.0e3F;
            const float fSn9 = 0.12e3f;
            const float fSn10 = .12e3F;
            const float fSn11 = 12e+3f;
            const float fSn12 = 12.0e+3F;
            const float fSn13 = +12.0e+3F;
            const float fSn14 = 0.12e+003f;
            const float fSn16 = 12e-3f;
            const float fSn17 = 12.0e-3f;
            const float fSn18 = +12.0e-3f;
            const float fSn19 = 0.12e-003f;
            const float fSn20 = .12e-0003f;


            // Variable dependent constant

            const int max_lights = defined(Android) ? 1 : 3;
        }

        [Test]
        public void UintConstantSpecialCases()
        {
            // NOTE: this test uses uint.Parse() to avoid constant-folding. Please do not try to be clever ;)

            // These use Ldc_I4_[0..8] for EmitConstant
            Assert.IsTrue(0x00000000U == uint.Parse("0"));
            Assert.AreEqual(uint.Parse("1"), 0x00000001U);
            Assert.IsTrue(0x00000001U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("2"), 0x00000002U);
            Assert.IsTrue(0x00000002U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("3"), 0x00000003U);
            Assert.IsTrue(0x00000003U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("4"), 0x00000004U);
            Assert.IsTrue(0x00000004U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("5"), 0x00000005U);
            Assert.IsTrue(0x00000005U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("6"), 0x00000006U);
            Assert.IsTrue(0x00000006U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("7"), 0x00000007U);
            Assert.IsTrue(0x00000007U > uint.Parse("0"));
            Assert.AreEqual(uint.Parse("8"), 0x00000008U);
            Assert.IsTrue(0x00000008U > uint.Parse("0"));

            // This uses Ldc_I4_M1 for EmitConstant
            Assert.IsTrue(0xFFFFFFFFU == uint.Parse("4294967295"));
            Assert.IsTrue(0xFFFFFFFFU > uint.Parse("0"));

            // These use Ldc_I4_S for EmitConstant:
            Assert.IsTrue(0xFFFFFFFEU == uint.Parse("4294967294"));
            Assert.IsTrue(0xFFFFFFFEU > uint.Parse("0"));
            Assert.IsTrue(0xFFFFFF80U == uint.Parse("4294967168"));
            Assert.IsTrue(0xFFFFFF80U > uint.Parse("0"));
        }

        [Test]
        public void LongConstantSpecialCases()
        {
            // NOTE: these use long.Parse() to avoid constant-folding. Please do not try to be clever ;)

            Assert.AreEqual(long.Parse("2147483648"), 0x80000000L);
            Assert.IsTrue(0x80000000L > long.Parse("0"));
            Assert.AreEqual(long.Parse("4294967295"), 0xFFFFFFFFL);
            Assert.IsTrue(0xFFFFFFFFL > long.Parse("0"));
        }

        [Test]
        public void NegativeZero()
        {
            Assert.IsTrue(double.IsNegativeInfinity(1.0 / -0.0));
            var tmp1 = -0.0;
            Assert.IsTrue(double.IsNegativeInfinity(1.0 / tmp1));

            Assert.IsTrue(float.IsNegativeInfinity(1.0f / -0.0f));
            var tmp2 = -0.0f;
            Assert.IsTrue(float.IsNegativeInfinity(1.0f / tmp2));
        }
    }
}
