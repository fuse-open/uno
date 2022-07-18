using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;

namespace UnoCore_Uno_IO
{
    public class StreamReaderTests
    {
        public StreamReader GetReaderFromString(string text)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return new StreamReader(stream);
        }

        public StreamReader GetReaderFromBytes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes);
            stream.Position = 0;
            return new StreamReader(stream);
        }

        [Test]
        public void Read()
        {
            var reader = GetReaderFromString("test");
            Assert.AreEqual((int)'t', reader.Read());
            Assert.AreEqual((int)'e', reader.Read());
            Assert.AreEqual((int)'s', reader.Read());
            Assert.AreEqual((int)'t', reader.Read());
            Assert.AreEqual(-1, reader.Read());
        }

        [Test]
        public void Peek()
        {
            var reader = GetReaderFromString("test");
            reader.Read();
            reader.Read();
            Assert.AreEqual((int)'s', reader.Peek());
            Assert.AreEqual((int)'s', reader.Peek());
            Assert.AreEqual((int)'s', reader.Peek());
        }

        [Test]
        public void ReadToArray()
        {
            var reader = GetReaderFromString("string");
            var buffer = new char[4];
            Assert.AreEqual(4, reader.ReadBlock(buffer, 0, 4));
            Assert.AreEqual("stri", new string(buffer));
            Assert.AreEqual(2, reader.ReadBlock(buffer, 1, 3));
            Assert.AreEqual("sngi", new string(buffer));
        }

        [Test]
        public void ReadToArrayOutOfIndex()
        {
            Assert.Throws<ArgumentException>(ReadToArrayOutOfIndexFunc);
        }

        private void ReadToArrayOutOfIndexFunc()
        {
            var reader = GetReaderFromString("string");
            var buffer = new char[4];
            Assert.AreEqual(4, reader.ReadBlock(buffer, 1, 4));
        }

        [Test]
        public void ReadLine()
        {
            var reader = GetReaderFromString("string1\n\rstring2\r\nstring3\nstring4");
            Assert.AreEqual("string1", reader.ReadLine());
            Assert.AreEqual(0, reader.ReadLine().Length);
            Assert.AreEqual("string2", reader.ReadLine());
            Assert.AreEqual("string3", reader.ReadLine());
            Assert.AreEqual("string4", reader.ReadLine());
            Assert.AreEqual(null, reader.ReadLine());
        }

        [Test]
        public void ReadToEnd()
        {
            var reader = GetReaderFromString("string1\n\rstring2\r\nstring3\nstring4");
            Assert.AreEqual("string1\n\rstring2\r\nstring3\nstring4", reader.ReadToEnd());
        }

        [Test]
        public void ReadToEndUnicode()
        {
            // Write the UTF-8 encoding of three Unicode code-points, to make
            // sure we test the three different long-encodings in UTF-8:
            //
            // - U+00B1: PLUS-MINUS SIGN, two-byte UTF-8 encoding
            // - U+0950: DEVANAGARI OM, three-byte UTF-8 encoding
            // - U+01D11E: MUSICAL SYMBOL G CLEF, four-byte UTF-8 encoding
            //
            // When checked, the latter code-point needs to be specified as a
            // UTF-16 surrogate pair, due to C#-isms.

            var reader = GetReaderFromBytes(new byte[]{ 0xC2, 0xB1, 0xE0, 0xA5, 0x90, 0xF0, 0x9D, 0x84, 0x9E });
            Assert.AreEqual("\u00B1\u0950\uD834\uDD1E", reader.ReadToEnd());
        }

        [Test]
        public void UnicodeNasty()
        {
            // prepare one-, two- and three-byte UTF-8 encoding code-point
            // variants, repeated 1024 times
            var variants = new string[]{ "\u00B1", "\u0950", "\uD834\uDD1E" };
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < variants.Length; ++j)
                    variants[j] += " " + variants[j];

            // prepare shift-patterns for the variants
            var shifts = new string[10];
            shifts[0] = string.Empty + ' ';
            for (int i = 1; i < shifts.Length; ++i)
                shifts[i] = shifts[i - 1] + shifts[0];

            foreach (var shift in shifts)
                foreach (var variant in variants)
                {
                    var data = shift + variant;
                    var result = GetReaderFromString(data).ReadToEnd();
                    if (data.Length != result.Length)
                        debug_log "data.Length: " + data.Length + ", result.Length: " + result.Length;
                    Assert.AreEqual(data, result);
                }
        }

        [Test]
        public void ReadBigString()
        {
            var str = GenerateString(5345);
            var reader = GetReaderFromString(str);
            var actual = reader.ReadToEnd();
            Assert.AreEqual(str.Length, actual.Length);
            Assert.AreEqual(str, actual);
        }

        [Test]
        public void ReadBigStringToArray()
        {
            var str = GenerateString(5345);
            var reader = GetReaderFromString(str);
            var buffer = new char[1000];
            Assert.AreEqual(1000, reader.ReadBlock(buffer, 0, 1000));
            Assert.AreEqual(str.Substring(0, 1000), new string(buffer));
            Assert.AreEqual(500, reader.ReadBlock(buffer, 500, 500));
            Assert.AreEqual(str.Substring(0, 500) + str.Substring(1000, 500), new string(buffer));
        }

        private string GenerateString(int count)
        {
            var array = new char[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = (char)(100 + i % 10);
            }
            return new string(array);
        }
    }
}
