using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.Text;

namespace Uno.Text.Test
{
    public class EncodingTest
    {
        static readonly byte[] Base64Data = new byte[] { (byte)'T', (byte)'3', (byte)'V', (byte)'0', (byte)'c', (byte)'m', (byte)'F', (byte)'j', (byte)'a', (byte)'3', (byte)'N', (byte)'G', (byte)'d', (byte)'X', (byte)'N', (byte)'l' };
        static readonly string DecodedBase64Data = "OutracksFuse";

        /*[Test]
        public void DecodeBase64()
        {
            var result = Encoding.DecodeBase64(Base64Data);
            Assert.AreEqual(DecodedBase64Data, result);
        }*/
    }
}