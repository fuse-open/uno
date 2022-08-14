using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;
using Uno.Text;

namespace UnoCore_Uno_IO
{
    public class StringReaderTests
    {
        [Test]
        public void Read()
        {
            var reader = new StringReader("test");
            Assert.AreEqual((int)'t', reader.Read());
            Assert.AreEqual((int)'e', reader.Read());
            Assert.AreEqual((int)'s', reader.Read());
            Assert.AreEqual((int)'t', reader.Read());
            Assert.AreEqual(-1, reader.Read());
        }

        [Test]
        public void Peek()
        {
            var reader = new StringReader("test");
            reader.Read();
            reader.Read();
            Assert.AreEqual((int)'s', reader.Peek());
            Assert.AreEqual((int)'s', reader.Peek());
            Assert.AreEqual((int)'s', reader.Peek());
        }

        [Test]
        public void ReadToArray()
        {
            var reader = new StringReader("string");
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
            var reader = new StringReader("string");
            var buffer = new char[4];
            Assert.AreEqual(4, reader.ReadBlock(buffer, 1, 4));
        }

        [Test]
        public void ReadLine()
        {
            var reader = new StringReader("string1\n\rstring2\r\nstring3\nstring4");
            Assert.AreEqual("string1", reader.ReadLine());
            Assert.AreEqual("''", "'" + reader.ReadLine() + "'");
            Assert.AreEqual("string2", reader.ReadLine());
            Assert.AreEqual("string3", reader.ReadLine());
            Assert.AreEqual("string4", reader.ReadLine());
        }

        [Test]
        public void ReadToEnd()
        {
            var reader = new StringReader("string1\n\rstring2\r\nstring3\nstring4");
            Assert.AreEqual("string1\n\rstring2\r\nstring3\nstring4", reader.ReadToEnd());
        }

        [Test]
        public void ReadBigString()
        {
            var str = GenerateString(12345);
            var reader = new StringReader(str);
            var actual = reader.ReadToEnd();
            Assert.AreEqual(str.Length, actual.Length);
            Assert.AreEqual(str, actual);
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
