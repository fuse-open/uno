using Uno;
using Uno.Text;
using Uno.Testing;

namespace Uno.Text.Test
{
    public class AsciiEncodingTest
    {
        byte[] bytes = new byte[] { 0x24, 0x40, 0x56, 0x3c, 0x3e, 0x35, 0x0A }; //$@V<>5\n
        string chars = "$@V<>5\n";

        [Test]
        public void AsciiDecodeTest()
        {
            var result = Ascii.GetString(bytes);
            Assert.AreEqual(chars, result);
        }

        [Test]
        public void AsciiEncodeTest()
        {
            var result = Ascii.GetBytes(chars);
            Assert.AreEqual(bytes, result);
        }

        [Test]
        public void AsciiInvalidCharDecodeTest()
        {
            var result = Ascii.GetString(new byte[] { 0x88  });
            Assert.AreEqual("?", result);
        }

        [Test]
        public void AsciiInvalidCharEncodeTest()
        {
            var result = Ascii.GetBytes("ш");
            Assert.AreEqual(new byte[] { 0x3f }, result);
        }

        [Test]
        public void AsciiEmptyDecodeTest()
        {
            var result = Ascii.GetString(new byte[0]);
            Assert.AreEqual("", result);
        }

        [Test]
        public void AsciiEmptyEncodeTest()
        {
            var result = Ascii.GetBytes("");
            Assert.AreEqual(new byte[0], result);
        }
    }
}