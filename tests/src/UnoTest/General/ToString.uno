using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class ToString
    {
        [Test]
        public void Run()
        {
            bool boolean = true;
            Assert.AreEqual("True", boolean.ToString());
            boolean = false;
            Assert.AreEqual("False", boolean.ToString());

            byte byteNumber = 23;
            Assert.AreEqual("23", byteNumber.ToString());
            byteNumber = 117;
            Assert.AreEqual("117", byteNumber.ToString());
            byteNumber = 255;
            Assert.AreEqual("255", byteNumber.ToString());

            char character = 'X';
            Assert.AreEqual("X", character.ToString());
            character = '\x0058';
            Assert.AreEqual("X", character.ToString());
            character = (char)88;
            Assert.AreEqual("X", character.ToString());
            character = '\u0058';
            Assert.AreEqual("X", character.ToString());

            double doubleNumber = 300.5;
            Assert.AreEqual("300.5", doubleNumber.ToString());
            doubleNumber = 0.7215e-12;
            //Issue #273
            //Assert.AreEqual("7.215E-13", doubleNumber.ToString());
            doubleNumber = 922.0;
            Assert.AreEqual("922", doubleNumber.ToString());

            float floatNumber = 7135.15f;
            Assert.AreEqual(floatNumber, float.Parse(floatNumber.ToString()));
            floatNumber = 2672.99f;
            Assert.AreEqual(floatNumber, float.Parse(floatNumber.ToString()));
            floatNumber = -122.0f;
            Assert.AreEqual(floatNumber, float.Parse(floatNumber.ToString()));

            int intNumber = (short)-153*2;
            Assert.AreEqual("-306", intNumber.ToString());
            intNumber = 2147483647;
            Assert.AreEqual("2147483647", intNumber.ToString());
            intNumber = Int.MaxValue;
            Assert.AreEqual("2147483647", intNumber.ToString());

            long longNumber = -15719387;
            Assert.AreEqual("-15719387", longNumber.ToString());
            longNumber = 2147163483647;
            Assert.AreEqual("2147163483647", longNumber.ToString());
            longNumber = Long.MaxValue;
            //Issue #275
            //Assert.AreEqual("9223372036854775807", longNumber.ToString());
            longNumber = 4611686014132420609;
            //Issue #275
            //Assert.AreEqual("4611686014132420609", longNumber.ToString());
            longNumber = (int.MaxValue * 2L);
            //Issue #272
            //Assert.AreEqual("4611686014132420609", longNumber.ToString());

            sbyte sbyteNumber = -12;
            Assert.AreEqual("-12", sbyteNumber.ToString());
            sbyteNumber = (sbyte)(32*3);
            Assert.AreEqual("96", sbyteNumber.ToString());
            sbyteNumber = SByte.MaxValue;
            Assert.AreEqual("127", sbyteNumber.ToString());

            short shortNumber = -32768;
            Assert.AreEqual("-32768", shortNumber.ToString());
            shortNumber = Short.MaxValue;
            Assert.AreEqual("32767", shortNumber.ToString());

            uint uintNumber = (ushort)64000 * 2u;
            Assert.AreEqual("128000", uintNumber.ToString());
            uintNumber = 3200000000;
            Assert.AreEqual("3200000000", uintNumber.ToString());
            uintNumber = UInt.MaxValue;
            Assert.AreEqual("4294967295", uintNumber.ToString());

            ulong ulongNumber = 15719387;
            Assert.AreEqual("15719387", ulongNumber.ToString());
            ulongNumber = 21471L;
            Assert.AreEqual("21471", ulongNumber.ToString());
            ulongNumber = ULong.MaxValue;
            //Issue #275
            //Assert.AreEqual("18446744073709551615", ulongNumber.ToString());
            //Issue #272
            //ulongNumber = 2140000000 * 2UL;
            //Assert.AreEqual("4280000000", ulongNumber.ToString());

            ushort ushortNumber = 41523;
            Assert.AreEqual("41523", ushortNumber.ToString());
            ushortNumber = UShort.MaxValue;
            Assert.AreEqual("65535", ushortNumber.ToString());

        }
    }
}
