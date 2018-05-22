using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;

namespace UnoCore_Uno_IO
{
    public class StreamWriterTests
    {
        [Test]
        public void WriteByteTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write((byte)255);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("255", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteSByteTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write((sbyte)-128);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("-128", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteIntTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write(int.MinValue);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("-2147483648", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLongTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write(3235L);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("3235", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteStringTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write("foo\r\tbar");
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("foo\r\tbar", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteCharBufferTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write(new char[] { 'f', 'o', 'o' });
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("foo", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteCharBufferFromIndexTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write(new char[]{'f','o','o', 'b', 'a', 'r' }, 2, 3);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("oba", r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineByteTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine((byte)255);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("255" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineSByteTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine((sbyte)-128);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("-128" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineIntTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine(int.MinValue);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("-2147483648" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineLongTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine(134352L);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("134352" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineStringTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine("foo\r\tbar");
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("foo\r\tbar" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineCharBufferTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine(new char[] { 'f', 'o', 'o' });
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("foo" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteLineCharBufferFromIndexTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine(new char[]{'f','o','o', 'b', 'a', 'r' }, 2, 3);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("oba" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine("foo");
            w.Write(15);
            w.WriteLine(15.6f);
            w.WriteLine(222.4);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("foo" + Environment.NewLine + "1515.6" + Environment.NewLine + "222.4" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteFormatTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.WriteLine("test - {0}", "test");
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual("test - test" + Environment.NewLine, r.ReadToEnd());
            w.Dispose();
            r.Dispose();
        }

        [Test]
        public void WriteBigString()
        {
            var str = GenerateString(2853);
            StreamWriter w = new StreamWriter(new MemoryStream());
            w.Write(str);
            w.Flush();
            var r = new StreamReader(w.BaseStream);
            r.BaseStream.Position = 0;
            var actual = r.ReadToEnd();
            Assert.AreEqual(str.Length, actual.Length);
            Assert.AreEqual(str, actual);
            w.Dispose();
            r.Dispose();
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

        [Test]
        public void WriteUnicodeStringTest()
        {
            StreamWriter w = new StreamWriter(new MemoryStream());

            // Write three Unicode code-points, to make sure we test the three
            // different long-encodings in UTF-8:
            //
            // - U+00B1: PLUS-MINUS SIGN, two-byte UTF-8 encoding
            // - U+0950: DEVANAGARI OM, three-byte UTF-8 encoding
            // - U+01D11E: MUSICAL SYMBOL G CLEF, four-byte UTF-8 encoding
            //
            // The latter code-point needs to be specified as a UTF-16 surrogate
            // pair, due to C#-isms.

            w.Write("\u00B1\u0950\uD834\uDD1E");
            w.Flush();
            var r = new BinaryReader(w.BaseStream);
            r.BaseStream.Position = 0;
            Assert.AreEqual(new byte[]{ 0xC2, 0xB1, 0xE0, 0xA5, 0x90, 0xF0, 0x9D, 0x84, 0x9E }, r.ReadBytes(9));
            w.Dispose();
            r.Dispose();
        }
    }
}
