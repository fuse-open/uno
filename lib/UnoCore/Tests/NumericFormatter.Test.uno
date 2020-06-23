using Uno;
using Uno.Testing;
using Uno.Testing.Assert;
using Uno.Internal;

namespace Uno.Test
{
    public class NumericFormatterTest
    {
        [Test]
        public void DecimalWithSameNumberOfDigits()
        {
            Assert.AreEqual( "43" , NumericFormatter.Format("D2", 43));
        }

        [Test]
        public void DecimalWithMoreDigits()
        {
            Assert.AreEqual( "1338" , NumericFormatter.Format("D2", 1338));
        }

        [Test]
        public void DecimalWithLessDigits()
        {
            Assert.AreEqual( "007" , NumericFormatter.Format("D3", 7));
        }

        [Test]
        public void DecimalWithOtherIntegralTypes()
        {
            Assert.AreEqual( "002" , NumericFormatter.Format("D3", (char)2));
            Assert.AreEqual( "003" , NumericFormatter.Format("D3", (sbyte)3));
            Assert.AreEqual( "004" , NumericFormatter.Format("D3", (byte)4));
            Assert.AreEqual( "005" , NumericFormatter.Format("D3", (short)5));
            Assert.AreEqual( "006" , NumericFormatter.Format("D3", (ushort)6));
            Assert.AreEqual( "007" , NumericFormatter.Format("D3", (int)7));
            Assert.AreEqual( "008" , NumericFormatter.Format("D3", (uint)8));
            Assert.AreEqual( "009" , NumericFormatter.Format("D3", (long)9));
            Assert.AreEqual( "010" , NumericFormatter.Format("D3", (ulong)10));
        }

        [Test]
        public void DecimalWithNegativeNumbers()
        {
            Assert.AreEqual( "-1" , NumericFormatter.Format("D1", -1));
            Assert.AreEqual( "-01" , NumericFormatter.Format("D2", -1));
        }

        [Test]
        public void DecimalWithBool()
        {
            Assert.AreEqual( "True" , NumericFormatter.Format("D", true));
            Assert.AreEqual( "False" , NumericFormatter.Format("D3", false));
        }

        [Test]
        public void DecimalWithRealNumbers()
        {
            Assert.Throws<FormatException>(() => NumericFormatter.Format("D1", 0.0f));
            Assert.Throws<FormatException>(() => NumericFormatter.Format("D1", 0.0));
        }

        [Test]
        public void FixedPointWithDouble()
        {
            Assert.AreEqual("0.00" , NumericFormatter.Format("F", 0.0));
            Assert.AreEqual("0.00" , NumericFormatter.Format("F", -0.0));
            Assert.AreEqual("0.0" , NumericFormatter.Format("F1", 0.0));
            Assert.AreEqual("1.23" , NumericFormatter.Format("F", 1.234));
            Assert.AreEqual("43.13" , NumericFormatter.Format("F2", 43.13));
            Assert.AreEqual("1.34" , NumericFormatter.Format("F2", 1.338));
            Assert.AreEqual("1.100" , NumericFormatter.Format("F3", 1.1));
            Assert.AreEqual("1" , NumericFormatter.Format("F0", 1.1));
            Assert.AreEqual("1.00", NumericFormatter.Format("F2", 1f));

            var doubleMaxValueString = NumericFormatter.Format("F", double.MaxValue);
            Assert.AreEqual(312, doubleMaxValueString.Length);
            Assert.IsTrue(doubleMaxValueString.StartsWith("17976931348623"));

            Assert.AreEqual("3.14", NumericFormatter.Format("F", Math.PI));
            Assert.AreEqual("3",    NumericFormatter.Format("F0", Math.PI));
            Assert.AreEqual("3.1",  NumericFormatter.Format("F1", Math.PI));
            Assert.AreEqual("3.14", NumericFormatter.Format("F2", Math.PI));
        }

        [Test]
        public void FixedPointWithIntegralTypes()
        {
            Assert.AreEqual( "2.000" , NumericFormatter.Format("F3", (char)2)); // Should give another result in DotNet
            Assert.AreEqual( "3.000" , NumericFormatter.Format("F3", (sbyte)3));
            Assert.AreEqual( "4.000" , NumericFormatter.Format("F3", (byte)4));
            Assert.AreEqual( "5.000" , NumericFormatter.Format("F3", (short)5));
            Assert.AreEqual( "6.000" , NumericFormatter.Format("F3", (ushort)6));
            Assert.AreEqual( "7.000" , NumericFormatter.Format("F3", (int)7));
            Assert.AreEqual( "8.000" , NumericFormatter.Format("F3", (uint)8));
            Assert.AreEqual( "9.000" , NumericFormatter.Format("F3", (long)9));
            Assert.AreEqual( "10.000" , NumericFormatter.Format("F3", (ulong)10));
        }

        [Test]
        public void FixedPointWithBool()
        {
            Assert.AreEqual( "True" , NumericFormatter.Format("F", true));
            Assert.AreEqual( "False" , NumericFormatter.Format("F3", false));
        }

        [Test]
        public void FixedPointWithNegativeNumbers()
        {
            Assert.AreEqual( "-1.0" , NumericFormatter.Format("F1", -1));
            Assert.AreEqual( "-1.00" , NumericFormatter.Format("F2", -1));
            Assert.AreEqual( "-1.40" , NumericFormatter.Format("F2", -1.4));
        }

        [Test]
        public void DecimalWithNoDigitSpecifier()
        {
            Assert.AreEqual("1337", NumericFormatter.Format("D", 1337));
        }

        [Test]
        public void FixedPointWithNoDigitSpecifier()
        {
            Assert.AreEqual("3.14", NumericFormatter.Format("F", Math.PI));
        }

        [Test]
        public void HexWithRealNumbers()
        {
            Assert.Throws<FormatException>(() => NumericFormatter.Format("X2", 0.20f));
            Assert.Throws<FormatException>(() => NumericFormatter.Format("X", 0.0));
        }

        [Test]
        public void HexWithNegativeNumber()
        {
            Assert.AreEqual("FFFFFFFF", NumericFormatter.Format("X", -1));
            Assert.AreEqual("FFFFFFFE", NumericFormatter.Format("X", -2));
            Assert.AreEqual("FFFFF822", NumericFormatter.Format("X", -2014));
            Assert.AreEqual("80", NumericFormatter.Format("X", sbyte.MinValue));
            Assert.AreEqual("8000", NumericFormatter.Format("X", short.MinValue));
            Assert.AreEqual("80000000", NumericFormatter.Format("X", int.MinValue));
            Assert.AreEqual("8000000000000001", NumericFormatter.Format("X", long.MinValue + 1));
            Assert.AreEqual("8000000000000000", NumericFormatter.Format("X", long.MinValue));
        }

        [Test]
        public void HexWithNormalDecimal()
        {
            Assert.AreEqual("0", NumericFormatter.Format("X", 0));
            Assert.AreEqual("1", NumericFormatter.Format("X", 1));
            Assert.AreEqual("9", NumericFormatter.Format("X", 9));
            Assert.AreEqual("A", NumericFormatter.Format("X", 10));
            Assert.AreEqual("539", NumericFormatter.Format("X", 1337));
            Assert.AreEqual("7DE", NumericFormatter.Format("X", 2014));
            Assert.AreEqual("7FFFFFFF", NumericFormatter.Format("X", int.MaxValue));
            Assert.AreEqual("FFFFFFFF", NumericFormatter.Format("X", uint.MaxValue));
            Assert.AreEqual("7FFFFFFFFFFFFFFF", NumericFormatter.Format("X", long.MaxValue));
            Assert.AreEqual("FFFFFFFFFFFFFFFF", string.Format("{0:X}", ulong.MaxValue));
            Assert.AreEqual("FFFFFFFFFFFFFFFF", NumericFormatter.Format("X", ulong.MaxValue));
        }

        [Test]
        public void HexLowercase()
        {
            Assert.AreEqual("539", NumericFormatter.Format("x", 1337));
            Assert.AreEqual("7de", NumericFormatter.Format("x", 2014));
        }

        [Test]
        public void HexWithWidth()
        {
            Assert.AreEqual("7DE", NumericFormatter.Format("X0", 2014));
            Assert.AreEqual("7DE", NumericFormatter.Format("X1", 2014));
            Assert.AreEqual("07DE", NumericFormatter.Format("X4", 2014));
            Assert.AreEqual("000007DE", NumericFormatter.Format("X8", 2014));
        }

        [Test]
        public void ExponentialWithBool()
        {
            Assert.AreEqual("True", NumericFormatter.Format("E", true));
            Assert.AreEqual("False", NumericFormatter.Format("E2", false));
        }

        [Test]
        public void ExponentialWithFloat()
        {
            Assert.AreEqual("0.000000E+000", NumericFormatter.Format("E", 0.0f));
            Assert.AreEqual("2.412000E+001", NumericFormatter.Format("E", 24.12f));
            Assert.AreEqual("2.41E+001", NumericFormatter.Format("E2", 24.12f));
            Assert.AreEqual("2.4120E+001", NumericFormatter.Format("E4", 24.12f));
        }

        [Test]
        public void ExponentialWithDouble()
        {
            Assert.AreEqual("0.000000E+000", NumericFormatter.Format("E", 0.0));
            Assert.AreEqual("0.000000E+000", NumericFormatter.Format("E", -0.0));
            Assert.AreEqual("0.000000e+000", NumericFormatter.Format("e", 0.0));

            Assert.AreEqual("4.940656E-324", NumericFormatter.Format("E", 4.940656E-324));
            Assert.AreEqual("-4.940656E-324", NumericFormatter.Format("E", -4.940656E-324));
            Assert.AreEqual("1.797693E+308", NumericFormatter.Format("E", double.MaxValue));
            Assert.AreEqual("-1.797693E+308", NumericFormatter.Format("E", -double.MaxValue));

            var doubleMinValueString = NumericFormatter.Format("E99", 4.940656E-324);
            Assert.AreEqual(2 + 99 + 5, doubleMinValueString.Length);
            Assert.IsTrue(doubleMinValueString.StartsWith("4.9406564584124"));
            Assert.IsTrue(doubleMinValueString.EndsWith("E-324"));

            var doubleMaxValueString = NumericFormatter.Format("E99", double.MaxValue);
            Assert.AreEqual(2 + 99 + 5, doubleMaxValueString.Length);
            Assert.IsTrue(doubleMaxValueString.StartsWith("1.7976931348623"));
            Assert.IsTrue(doubleMaxValueString.EndsWith("E+308"));
        }

        [Test]
        public void ExponentialWithIntegralTypes()
        {
            Assert.AreEqual("0.000e+000", NumericFormatter.Format("e3", (char)0));
            Assert.AreEqual("5.600e+001", NumericFormatter.Format("e3", (char)56)); // Should give another result in DotNet
            Assert.AreEqual("5.800e+001", NumericFormatter.Format("e3", (sbyte)58));
            Assert.AreEqual("2.500e+001", NumericFormatter.Format("e3", (byte)25));
            Assert.AreEqual("5.200e+001", NumericFormatter.Format("e3", (short)52));
            Assert.AreEqual("6.000e+000", NumericFormatter.Format("e3", (ushort)6));
            Assert.AreEqual("7.200e+001", NumericFormatter.Format("e3", (int)72));
            Assert.AreEqual("8.100E+001", NumericFormatter.Format("E3", (uint)81));
            Assert.AreEqual("2.900E+001", NumericFormatter.Format("E3", (long)29));
            Assert.AreEqual("1.000E+001", NumericFormatter.Format("E3", (ulong)10));
        }

        [Test]
        public void GeneralWithBool()
        {
            Assert.AreEqual("True", NumericFormatter.Format("G", true));
            Assert.AreEqual("False", NumericFormatter.Format("G2", false));
        }

        [Test]
        public void GeneralWithLongInt()
        {
            Assert.AreEqual("2.41E+04", NumericFormatter.Format("g3", 24123));
            Assert.AreEqual("6.5E+04", NumericFormatter.Format("g2", 65345));
            Assert.AreEqual("2.415E+06", NumericFormatter.Format("g4", 2415434));
            Assert.AreEqual("-1.8E+06", NumericFormatter.Format("G2", -1752142));
        }

        [Test]
        public void GeneralWithFloat()
        {
            // G and G0 should give the same result, pi with 6 digits
            Assert.AreEqual("3.141593", NumericFormatter.Format("G", (float)Math.PI));
            Assert.AreEqual("3.141593", NumericFormatter.Format("G0", (float)Math.PI));

            Assert.AreEqual("3",    NumericFormatter.Format("G1", (float)Math.PI));
            Assert.AreEqual("3.1",  NumericFormatter.Format("G2", (float)Math.PI));
            Assert.AreEqual("3.14", NumericFormatter.Format("G3", (float)Math.PI));

            Assert.AreEqual("24.1226", NumericFormatter.Format("g", 24.1226f));
            Assert.AreEqual("24", NumericFormatter.Format("g2", 24.12f));
            Assert.AreEqual("-24.1", NumericFormatter.Format("g4", -24.1f));
            Assert.AreEqual("1.8E+03", NumericFormatter.Format("G2", 1752.142f));
        }

        [Test]
        public void GeneralWithDouble()
        {
            Assert.AreEqual("0", NumericFormatter.Format("G", 0.0));
            Assert.AreEqual("0", NumericFormatter.Format("G", -0.0));

            Assert.AreEqual("99", NumericFormatter.Format("G2", 99.0));
            Assert.AreEqual("1E+02", NumericFormatter.Format("G2", 100.0));

            Assert.AreEqual("3.14159265358979", NumericFormatter.Format("G", Math.PI));
            Assert.AreEqual("100000000000000", NumericFormatter.Format("G", 1e14));
            Assert.AreEqual("1E+15", NumericFormatter.Format("G", 1e15));
            Assert.AreEqual("0.111111111111111", NumericFormatter.Format("G", 0.1111111111111111111111111111111));
            Assert.AreEqual("1E+99", NumericFormatter.Format("G", 1E99));
            Assert.AreEqual("3.14159265358979E+99", NumericFormatter.Format("G", 1E99 * Math.PI));
        }

        [Test]
        public void GeneralWithIntegralTypes()
        {
            Assert.AreEqual("56", NumericFormatter.Format("G3", (char)56)); // Should give another result in DotNet
            Assert.AreEqual("58", NumericFormatter.Format("G3", (sbyte)58));
            Assert.AreEqual("25", NumericFormatter.Format("G3", (byte)25));
            Assert.AreEqual("52", NumericFormatter.Format("G3", (short)52));
            Assert.AreEqual("6", NumericFormatter.Format("G3", (ushort)6));
            Assert.AreEqual("72", NumericFormatter.Format("G3", (int)72));
            Assert.AreEqual("81", NumericFormatter.Format("G3", (uint)81));
            Assert.AreEqual("29", NumericFormatter.Format("G3", (long)29));
            Assert.AreEqual("10", NumericFormatter.Format("G3", (ulong)10));
        }

        [Test]
        public void PercentsWithBool()
        {
            Assert.AreEqual("True", NumericFormatter.Format("P", true));
            Assert.AreEqual("False", NumericFormatter.Format("P2", false));
        }

        [Test]
        public void PercentsWithInteger()
        {
            Assert.AreEqual("2,400.00 %", NumericFormatter.Format("p", 24));
            Assert.AreEqual("12,300 %", NumericFormatter.Format("p0", 123));
            Assert.AreEqual("-65,400.0000 %", NumericFormatter.Format("p4", -654));
            Assert.AreEqual("600.000 %", NumericFormatter.Format("p3", 6));
        }

        [Test]
        public void PercentsWithFloat()
        {
            Assert.AreEqual("2,412.23 %", NumericFormatter.Format("p", 24.12226f));
            Assert.AreEqual("-2,412.00 %", NumericFormatter.Format("p2", -24.12f));
            Assert.AreEqual("2,410.0000 %", NumericFormatter.Format("p4", 24.1f));
            Assert.AreEqual("2,410 %", NumericFormatter.Format("p0", 24.1f));
        }

        [Test]
        public void PercentsWithIntegralTypes()
        {
            Assert.AreEqual("5,600.000 %", NumericFormatter.Format("P3", (char)56)); // Should give another result in DotNet
            Assert.AreEqual("5,800.000 %", NumericFormatter.Format("P3", (sbyte)58));
            Assert.AreEqual("2,500.000 %", NumericFormatter.Format("P3", (byte)25));
            Assert.AreEqual("5,200.000 %", NumericFormatter.Format("P3", (short)52));
            Assert.AreEqual("600.000 %", NumericFormatter.Format("P3", (ushort)6));
            Assert.AreEqual("7,200.000 %", NumericFormatter.Format("P3", (int)72));
            Assert.AreEqual("8,100.000 %", NumericFormatter.Format("P3", (uint)81));
            Assert.AreEqual("2,900.000 %", NumericFormatter.Format("P3", (long)29));
            Assert.AreEqual("1,000.000 %", NumericFormatter.Format("P3", (ulong)10));
        }

        [Test]
        public void NumberWithBool()
        {
            Assert.AreEqual("True", NumericFormatter.Format("N", true));
            Assert.AreEqual("False", NumericFormatter.Format("N2", false));
        }

        [Test]
        public void NumberWithFloat()
        {
            Assert.AreEqual("24.13", NumericFormatter.Format("n", 24.125f));
            Assert.AreEqual("24.12", NumericFormatter.Format("n2", 24.12f));
            Assert.AreEqual("24.1000", NumericFormatter.Format("n4", 24.1f));
        }

        [Test]
        public void NumberWithIntegralTypes()
        {
            Assert.AreEqual("56.000", NumericFormatter.Format("N3", (char)56)); // Should give another result in DotNet
            Assert.AreEqual("58.000", NumericFormatter.Format("N3", (sbyte)58));
            Assert.AreEqual("25.000", NumericFormatter.Format("N3", (byte)25));
            Assert.AreEqual("52.000", NumericFormatter.Format("N3", (short)52));
            Assert.AreEqual("6.000", NumericFormatter.Format("N3", (ushort)6));
            Assert.AreEqual("72.000", NumericFormatter.Format("N3", (int)72));
            Assert.AreEqual("81.000", NumericFormatter.Format("N3", (uint)81));
            Assert.AreEqual("29.000", NumericFormatter.Format("N3", (long)29));
            Assert.AreEqual("10.000", NumericFormatter.Format("N3", (ulong)10));
        }

        [Test]
        public void CustomFormatWithBool()
        {
            Assert.AreEqual("True", NumericFormatter.Format("0.##", true));
            Assert.AreEqual("False", NumericFormatter.Format("#", false));
        }

        [Test]
        public void CustomFormatWithFloat()
        {
            Assert.AreEqual("24.13", NumericFormatter.Format("0.00", 24.125f));
            Assert.AreEqual("24.2", NumericFormatter.Format("#.#", 24.151f));
            Assert.AreEqual("24", NumericFormatter.Format("0.##", 24f));
            Assert.AreEqual("-m3432.q5w2", NumericFormatter.Format("m0.q#w#", -3432.523f));
            Assert.AreEqual("24.1200", NumericFormatter.Format("##.###0#", 24.12f));
            Assert.AreEqual("24.12000", NumericFormatter.Format("0#.####0", 24.12f));
            Assert.AreEqual("024.1x2x", NumericFormatter.Format("#0##.#x##x#", 24.12f));
            Assert.AreEqual("24.1000", NumericFormatter.Format("#.0000", 24.1f));
            Assert.AreEqual("24.1000", NumericFormatter.Format("#.00.00", 24.1f));
            Assert.AreEqual("24.100L", NumericFormatter.Format("#.000L", 24.1f));
            Assert.AreEqual("24.1 usd", NumericFormatter.Format("#.## usd", 24.1f));
            Assert.AreEqual(".1", NumericFormatter.Format("#.0", 0.074f));
        }

        [Test]
        public void CustomFormatWithIntegralTypes()
        {
            Assert.AreEqual("56.000", NumericFormatter.Format("#.000", (char)56)); // Should give another result in DotNet
            Assert.AreEqual("58.000", NumericFormatter.Format("#.000", (sbyte)58));
            Assert.AreEqual("25.000", NumericFormatter.Format("#.000", (byte)25));
            Assert.AreEqual("52.000", NumericFormatter.Format("#.000", (short)52));
            Assert.AreEqual("6.000", NumericFormatter.Format("#.000", (ushort)6));
            Assert.AreEqual("72.000", NumericFormatter.Format("#.000", (int)72));
            Assert.AreEqual("81.000", NumericFormatter.Format("#.000", (uint)81));
            Assert.AreEqual("29.000", NumericFormatter.Format("#.000", (long)29));
            Assert.AreEqual("10.000", NumericFormatter.Format("#.000", (ulong)10));
        }

    }
}
