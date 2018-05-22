using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;
using Uno.Testing.Assert;

namespace Uno.Test
{
    //See also NumericFormatter.Test for more specific tests
    public class StringFormatTest
    {

        [Test]
        public void FormattingWith1Index()
        {
            Assert.AreEqual("foo 1",  String.Format("foo {0}", 1));
        }

        [Test]
        public void FormattingWith2Indicies()
        {
            Assert.AreEqual("foo 1 2",  String.Format("foo {0} {1}", 1, '2'));
        }

        [Test]
        public void FormattingWith3Indicies()
        {
            Assert.AreEqual("foo 1 2 3",  String.Format("foo {0} {1} {2}", 1, '2', "3"));
        }

        [Test]
        public void FormattingWith4Indicies()
        {
            Assert.AreEqual("foo 1 2 3 4",  String.Format("foo {0} {1} {2} {3}", 1, '2', "3", 4));
        }

        [Test]
        public void UsesEmptyFormatString()
        {
            Assert.AreEqual("test 23.15", String.Format("test {0:}", 23.15f));
            Assert.AreEqual("test 777", String.Format("test {0:}", 777));
        }

        [Test]
        public void UsesDecimalFormatString()
        {
            Assert.AreEqual("007 says 42",  String.Format("{0:D3} says {1:D1}", 7, 42));
            Assert.AreEqual("-01337",  String.Format("{0:D5}", -1337));
        }

        [Test]
        public void UsesFixedPointFormatString()
        {
            Assert.AreEqual("42.0 foo 13.37000",  String.Format("{0:F1} foo {1:F5}", 42, 13.37));
        }

        [Test]
        public void FixedPointWorksWithZeroDigits()
        {
            Assert.AreEqual("1", String.Format("{0:F0}", 1f));
        }

        [Test]
        public void UsesHexadecimalFormatString()
        {
            Assert.AreEqual("10", String.Format("{0:X}", 16));
        }

        [Test]
        public void HexadecimalZeroPad()
        {
            Assert.AreEqual("00", String.Format( "{0:X2}", 0 ));
        }

        [Test]
        public void UsesExponentialFormatString()
        {
            Assert.AreEqual("1.0E-004", String.Format("{0:E1}", 0.0001));
            Assert.AreEqual("1.000000E-004", String.Format("{0:E}", 0.0001));
            Assert.AreEqual("3.95322E+003", String.Format("{0:E5}", 3953.22));
        }

        [Test]
        public void UsesGeneralFormatString()
        {
            Assert.AreEqual("122.4", String.Format("{0:G}", 122.4));
            Assert.AreEqual("39522223.22422", String.Format("{0:G}", 39522223.22422));
            Assert.AreEqual("3.9522E+07", String.Format("{0:G5}", 39522223.224223423));
        }

        [Test]
        public void UsesPercentFormatString()
        {
            Assert.AreEqual("8,324.10000 %", String.Format("{0:P5}", 83.241));
            Assert.AreEqual("2.24 %", String.Format("{0:P}", 0.02241));
            Assert.AreEqual("24.000 %", String.Format("{0:P3}", 0.24));
        }

        [Test]
        public void UsesNumberFormatString()
        {
            Assert.AreEqual("1,234.57", String.Format("{0:N}", 1234.567));
            Assert.AreEqual("1,234.0", String.Format("{0:N1}", 1234));
            Assert.AreEqual("-1,234.560", String.Format("{0:N3}", -1234.56));
        }

        [Test]
        public void UsesCustomFormatString()
        {
            Assert.AreEqual("1234.57", String.Format("{0:0.##}", 1234.567));
            Assert.AreEqual(".8510", String.Format("{0:#.0000}", 0.851));
            Assert.AreEqual("-1234.6", String.Format("{0:0.0}", -1234.56));
            Assert.AreEqual("012", String.Format("{0:000}", 12));
        }

        [Test]
        public void IndexingOutsideItemsThrow()
        {
            Assert.Throws<FormatException>(IndexOutsideItems);
        }

        private void IndexOutsideItems()
        {
            String.Format("{1}", 0);
        }

        [Test]
        public void NegativeIndexThrows()
        {
            Assert.Throws<FormatException>(NegativeIndex);
        }

        private void NegativeIndex()
        {
            String.Format("{-1}", 0);
        }

        [Test]
        public void FormatStringItemThrowsForCharactersWeDontSupportYet()
        {
            foreach (var c in "CcRr")
            {
                try
                {
                    String.Format("{0:" + c + "}", 0);
                    Assert.Fail("Expected ArgumentException for " + c);
                    //throw new Exception("Expected ArgumentException for " + c);
                }
                catch (FormatException e)
                {
                }
            }
        }
    }
}
