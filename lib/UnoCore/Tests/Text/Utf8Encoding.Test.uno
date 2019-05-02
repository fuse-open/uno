using Uno;
using Uno.Text;
using Uno.Testing;

namespace Uno.Text.Test
{
    public class Utf8EncodingTest
    {
        byte[] bytes = new byte[] { 0x7e, 0x24, 0x40, 0x26, 0xC2, 0xB1, 0xE0, 0xA5, 0x90, 0xF0, 0x9D, 0x84, 0x9E, 0xF4, 0x8F, 0xBF, 0xBF }; // '~', '$', '@', '&', U+00B1, U+0950, U+01D11E, U+10FFFF
        string chars = "\u007e\u0024\u0040\u0026\u00B1\u0950\uD834\uDD1E\uDBFF\uDFFF"; //~$@&

        [Test]
        public void Utf8DecodeTest()
        {
            var result = Utf8.GetString(bytes);
            Assert.AreEqual(chars, result);
        }

        [Test]
        public void Utf8DecodeTest2()
        {
            var result = Utf8.GetString(bytes, 1, 2);
            Assert.AreEqual(chars.Substring(1, 2), result);
        }

        [Test]
        public void Utf8NullDecodeTest()
        {
            Assert.Throws<ArgumentNullException>(() => Utf8.GetString(null));
        }

        [Test]
        public void Utf8NullEncodeTest()
        {
            Assert.Throws<ArgumentNullException>(() => Utf8.GetBytes(null));
        }

        [Test]
        public void Utf8EncodeTest()
        {
            var result = Utf8.GetBytes(chars);
            Assert.AreEqual(bytes, result);
        }

        [Test]
        public void UTF8DecoderBasic()
        {
            var decoder = Encoding.UTF8.GetDecoder();
            var buf = new char[chars.Length];
            var charsUsed = decoder.GetChars(bytes, 0, bytes.Length, buf, 0);
            Assert.AreEqual(chars.Length, charsUsed);
            Assert.AreEqual(chars, new string(buf));
        }

        byte[] PrepareInput(byte[] input)
        {
            // glue together 'A' + input + 'B'
            var output = new byte[1 + input.Length + 1];
            output[0] = (byte)'A';
            Array.Copy(input, 0, output, 1, input.Length);
            output[output.Length - 1] = (byte)'B';
            return output;
        }

        char[] PrepareExpected(char[] input)
        {
            // glue together 'A' + input + 'B'
            var output = new char[1 + input.Length + 1];
            output[0] = 'A';
            Array.Copy(input, 0, output, 1, input.Length);
            output[output.Length - 1] = 'B';
            return output;
        }

        void VerifyDecoding(byte[] input, char[] expected)
        {
            var decoder = Encoding.UTF8.GetDecoder();
            Assert.AreEqual(expected.Length, decoder.GetCharCount(input, 0, input.Length));
            var buf = new char[expected.Length];
            var charsUsed = decoder.GetChars(input, 0, input.Length, buf, 0);
            Assert.AreEqual(expected.Length, charsUsed);
            Assert.AreEqual(expected, buf);
        }

        void VerifySplitDecoding(byte[] input, char[] expected)
        {
            // try all positions that splits the input
            for (int j = 1; j < input.Length - 1; ++j)
            {
                // pick the first j characters of input
                var headInput = new byte[j];
                Array.Copy(input, 0, headInput, 0, j);

                // pick everything but the first j characters of input
                var tailInput = new byte[input.Length - j];
                Array.Copy(input, j, tailInput, 0, input.Length - j);

                // verify that the different UTF-8 sequences decode to the same no matter where they got split
                var buf = new char[expected.Length];
                var decoder = Encoding.UTF8.GetDecoder();
                var charsUsed = decoder.GetChars(headInput, 0, headInput.Length, buf, 0);
                charsUsed += decoder.GetChars(tailInput, 0, tailInput.Length, buf, charsUsed);
                Assert.AreEqual(expected.Length, charsUsed);
                Assert.AreEqual(expected, buf);
            }
        }

        byte[] EncodeOverlongUTF8(int codePoint, int bytes)
        {
            if (bytes < 2 || bytes > 6)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            int trailingBytes = bytes - 1;
            int mask = 0x3F >> trailingBytes;
            int byteMark = 0xFC << (5 - trailingBytes);

            var ret = new byte[1 + trailingBytes];
            ret[0] = (byte)(byteMark | (codePoint >> (6 * trailingBytes)) & 0x7f);
            for (int i = 0; i < trailingBytes; ++i)
                ret[1 + i] = ((byte)(0x80 | (codePoint >> (6 * (trailingBytes - 1 - i))) & 0xBF));
            return ret;
        }

        [Test]
        public void UTF8DecoderBrokenEncoding()
        {
            var inputs = new byte[][]
            {
                new byte[] { 0xC2 }, // broken U+00B1

                new byte[] { 0xE0 }, // broken U+0950
                new byte[] { 0xE0, 0xA5 },

                new byte[] { 0xF0 }, // broken U+01D11E
                new byte[] { 0xF0, 0x9D },
                new byte[] { 0xF0, 0x9D, 0x84 },
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifyDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderSurrogateEncoding()
        {
            var inputs = new byte[][]
            {
                new byte[] { 0xED, 0xA0, 0x80 }, // U+D800
                new byte[] { 0xED, 0xAD, 0xBF }, // U+DB7F
                new byte[] { 0xED, 0xAE, 0x80 }, // U+DB80
                new byte[] { 0xED, 0xAF, 0xBF }, // U+DBFF
                new byte[] { 0xED, 0xBE, 0x80 }, // U+DF80

                new byte[] { 0xED, 0xB0, 0x80 }, // U+DC00
                new byte[] { 0xED, 0xBF, 0xBF }, // U+DFFF

                new byte[] { 0xED, 0xA0, 0x80, 0xED, 0xB0, 0x80 }, // U+D800 U+DC00
                new byte[] { 0xED, 0xA0, 0x80, 0xED, 0xBF, 0x8F }, // U+D800 U+DFFF
                new byte[] { 0xED, 0xAD, 0xBF, 0xED, 0xB0, 0x80 }, // U+DB7F U+DC00
                new byte[] { 0xED, 0xAD, 0xBF, 0xED, 0xBF, 0x8F }, // U+DB7F U+DFFF
                new byte[] { 0xED, 0xAF, 0xBF, 0xED, 0xB0, 0x80 }, // U+DB80 U+DC00
                new byte[] { 0xED, 0xAF, 0xBF, 0xED, 0xBF, 0x8F }, // U+DB80 U+DFFF
                new byte[] { 0xED, 0xAF, 0xBF, 0xED, 0xB0, 0x80 }, // U+DBFF U+DC00
                new byte[] { 0xED, 0xAF, 0xBF, 0xED, 0xBF, 0x8F }, // U+DBFF U+DFFF
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },

                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },

                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifyDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderBrokenSurrogateEncoding()
        {
            var inputs = new byte[][]
            {
                // single broken surrogates
                new byte[] { 0xED }, // broken U+D800
                new byte[] { 0xED }, // broken U+DB7F
                new byte[] { 0xED }, // broken U+DC00
                new byte[] { 0xED }, // broken U+DFFF

                new byte[] { 0xED, 0xA0 }, // broken U+D800
                new byte[] { 0xED, 0xAD }, // broken U+DB7F
                new byte[] { 0xED, 0xB0 }, // broken U+DC00
                new byte[] { 0xED, 0xBF }, // broken U+DFFF

                new byte[] { 0xED, 0xED }, // broken U+D800, broken U+DC00
                new byte[] { 0xED, 0xED }, // broken U+D800, broken U+DFFF
                new byte[] { 0xED, 0xED }, // broken U+DB7F, broken U+DC00
                new byte[] { 0xED, 0xED }, // broken U+DB7F, broken U+DFFF

                new byte[] { 0xED, 0xA0, 0xED, 0xB0 }, // broken U+D800, broken U+DC00
                new byte[] { 0xED, 0xA0, 0xED, 0xBF }, // broken U+D800, broken U+DFFF
                new byte[] { 0xED, 0xAD, 0xED, 0xB0 }, // broken U+DB7F, broken U+DC00
                new byte[] { 0xED, 0xAD, 0xED, 0xBF }, // broken U+DB7F, broken U+DFFF
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },

                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifyDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderMidCharacterSplit()
        {
            var inputs = new byte[][]
            {
                new byte[] { 0xC2, 0xB1 }, // UTF-8 encoded U+00B1
                new byte[] { 0xE0, 0xA5, 0x90 }, // UTF-8 encoded U+0950
                new byte[] { 0xF0, 0x9D, 0x84, 0x9E }, // UTF-8 encoded U+01D11E
            };

            var expecteds = new char[][]
            {
                new char[] { '\u00B1' },
                new char[] { '\u0950' },
                new char[] { '\uD834', '\uDD1E' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifySplitDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderOverlongEncoding()
        {
            var inputs = new byte[][]
            {
                EncodeOverlongUTF8(0x0000, 2),
                EncodeOverlongUTF8(0x007F, 2),
                EncodeOverlongUTF8(0x0000, 3),
                EncodeOverlongUTF8(0x07FF, 3),
                EncodeOverlongUTF8(0x0000, 4),
                EncodeOverlongUTF8(0xFFFF, 4),
                EncodeOverlongUTF8(0x110000, 4),
                EncodeOverlongUTF8(0x1FFFFF, 4),
                EncodeOverlongUTF8(0x000000, 5),
                EncodeOverlongUTF8(0x1FFFFF, 5),
                EncodeOverlongUTF8(0x000000, 6),
                EncodeOverlongUTF8(0x1FFFFF, 6),
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifyDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderBrokenOverlongEncoding()
        {
            var inputs = new byte[][]
            {
                new byte[] { 0xC0 }, // broken 2-byte overlong U+0000
                new byte[] { 0xC1 }, // broken 2-byte overlong U+007F

                new byte[] { 0xE0 }, // broken 3-byte overlong U+0000
                new byte[] { 0xE0 }, // broken 3-byte overlong U+07FF

                new byte[] { 0xF0 }, // broken 4-byte overlong U+0000
                new byte[] { 0xF0 }, // broken 4-byte overlong U+FFFF

                new byte[] { 0xE0, 0x80 }, // broken 3-byte overlong U+0000
                new byte[] { 0xE0, 0x9F }, // broken 3-byte overlong U+07FF

                new byte[] { 0xF0, 0x80 }, // broken 4-byte overlong U+0000
                new byte[] { 0xF0, 0x8F }, // broken 4-byte overlong U+FFFF

                new byte[] { 0xF0, 0x80, 0x80 }, // broken 4-byte overlong U+0000
                new byte[] { 0xF0, 0x8F, 0xBF }, // broken 4-byte overlong U+FFFF
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD' },
                new char[] { '\uFFFD' },

                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifyDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

        [Test]
        public void UTF8DecoderOverlongMidCharacterSplit()
        {
            var inputs = new byte[][]
            {
                EncodeOverlongUTF8(0x0000, 2),
                EncodeOverlongUTF8(0x007F, 2),
                EncodeOverlongUTF8(0x0000, 3),
                EncodeOverlongUTF8(0x07FF, 3),
                EncodeOverlongUTF8(0x0000, 4),
                EncodeOverlongUTF8(0xFFFF, 4),
                EncodeOverlongUTF8(0x110000, 4),
                EncodeOverlongUTF8(0x1FFFFF, 4),
                EncodeOverlongUTF8(0x000000, 5),
                EncodeOverlongUTF8(0x1FFFFF, 5),
                EncodeOverlongUTF8(0x000000, 6),
                EncodeOverlongUTF8(0x1FFFFF, 6),
            };

            var expecteds = new char[][]
            {
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
                new char[] { '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD', '\uFFFD' },
            };

            for (int i = 0; i < inputs.Length; ++i)
                VerifySplitDecoding(PrepareInput(inputs[i]), PrepareExpected(expecteds[i]));
        }

    }
}
