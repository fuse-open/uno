using Uno;
using Uno.UX;
using Uno.Testing;

namespace UXTests
{
    public class CustomApp : Uno.Application
    {
        [UXContent]
        public InnerClass SomeContent { get; set; }
    }

    public class Tests
    {
        [Test]
        public void TestContent()
        {
            var app = new BasicApp();
            Assert.IsTrue(app.SomeContent != null);
            Assert.AreEqual("Foo", app.SomeContent.GetType().ToString());
        }
    }

    public class InnerClass
    {

    }
}
