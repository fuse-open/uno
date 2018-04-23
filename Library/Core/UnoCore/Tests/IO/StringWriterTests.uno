using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;
using Uno.Text;

namespace UnoCore_Uno_IO
{
    public class StringWriterTests
    {
        [Test]
        public void WriteCharTest()
        {
            var w = new StringWriter();
            w.Write('X');
            Assert.AreEqual("X", w.ToString());
        }

        [Test]
        public void WriteByteTest()
        {
            var w = new StringWriter();
            w.Write((byte)255);
            Assert.AreEqual("255", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteSByteTest()
        {
            var w = new StringWriter();
            w.Write((sbyte)-128);
            Assert.AreEqual("-128", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteIntTest()
        {
            var w = new StringWriter();
            w.Write(int.MinValue);
            Assert.AreEqual("-2147483648", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLongTest()
        {
            var w = new StringWriter();
            w.Write(873925L);
            Assert.AreEqual("873925", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteStringTest()
        {
            var w = new StringWriter();
            w.Write("foo\r\tbar");
            Assert.AreEqual("foo\r\tbar", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteCharBufferTest()
        {
            var w = new StringWriter();
            w.Write(new char[] { 'f', 'o', 'o' });
            Assert.AreEqual("foo", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteCharBufferFromIndexTest()
        {
            var w = new StringWriter();
            w.Write(new char[]{'f','o','o', 'b', 'a', 'r' }, 2, 3);
            Assert.AreEqual("oba", w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineByteTest()
        {
            var w = new StringWriter();
            w.WriteLine((byte)255);
            Assert.AreEqual("255" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineSByteTest()
        {
            var w = new StringWriter();
            w.WriteLine((sbyte)-128);
            Assert.AreEqual("-128" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineIntTest()
        {
            var w = new StringWriter();
            w.WriteLine(int.MinValue);
            Assert.AreEqual("-2147483648" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineLongTest()
        {
            var w = new StringWriter();
            w.WriteLine(5712838L);
            Assert.AreEqual("5712838" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineStringTest()
        {
            var w = new StringWriter();
            w.WriteLine("foo\r\tbar");
            Assert.AreEqual("foo\r\tbar" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineCharBufferTest()
        {
            var w = new StringWriter();
            w.WriteLine(new char[] { 'f', 'o', 'o' });
            Assert.AreEqual("foo" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteLineCharBufferFromIndexTest()
        {
            var w = new StringWriter();
            w.WriteLine(new char[]{'f','o','o', 'b', 'a', 'r' }, 2, 3);
            Assert.AreEqual("oba" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteTest()
        {
            var w = new StringWriter();
            w.WriteLine("foo");
            w.WriteLine(15);
            w.WriteLine(15.6f);
            w.WriteLine(222.4);
            Assert.AreEqual("foo" + Environment.NewLine + "15" + Environment.NewLine + "15.6" + Environment.NewLine + "222.4" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteFormatTest()
        {
            var w = new StringWriter();
            w.WriteLine("test - {0}", 123);
            Assert.AreEqual("test - 123" + Environment.NewLine, w.ToString());
            w.Dispose();
        }

        [Test]
        public void WriteBigString()
        {
            var str = GenerateString(7374);
            var w = new StringWriter();
            w.Write(str);
            var actual = w.ToString();
            Assert.AreEqual(str.Length, actual.Length);
            Assert.AreEqual(str, actual);
            w.Dispose();
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
