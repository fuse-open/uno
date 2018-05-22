using Uno;
using Uno.Text;
using Uno.Testing;

namespace Uno.Text.Test
{
    public class Base64Test
    {

        string chars = "dGVzdF9zdHJpbmc1Nn4hQCMkJV4mKigpXys9LS8uLHwgZw==";  //test_string56~!@#$%^&*()_+=-/.,| g
        byte[] bytes = new byte[] { 116, 101, 115, 116, 95, 115, 116, 114, 105, 110, 103, 53, 54, 126, 33, 64, 35, 36, 37, 94, 38, 42, 40, 41, 95, 43, 61, 45, 47, 46, 44, 124, 32, 103 }; //test_string56~!@#$%^&*()_+=-/.,| g

        [Test]
        public void Base64Encode()
        {
            var result = Base64.GetString(bytes);
            Assert.AreEqual(chars, result);
        }

        [Test]
        public void Base64Decode()
        {
            var result = Base64.GetBytes(chars);
            Assert.AreEqual(bytes, result);
        }
    }
}