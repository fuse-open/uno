using Uno;
using Uno.Testing;

namespace Uno.UX.Tests
{
    public class EncodingTests
    {
        public class EncodingTester
        {
            [UXContent]
            public string Content { get; set; }
            public string Attribute { get; set; }
        }

        [Test]
        public void Content()
        {
            var e = new UXHelpers.ContentEncoding();
            Assert.AreEqual("\uC544\uCE68\uAE00 \n \uC544\uCE68\uAE00", e.Content);
        }

        [Test]
        public void Attribute()
        {
            var e = new UXHelpers.AttributeEncoding();
            Assert.AreEqual("\uC544\uCE68\uAE00 \n \uC544\uCE68\uAE00 && < >", e.Attribute);
        }
    }
}
