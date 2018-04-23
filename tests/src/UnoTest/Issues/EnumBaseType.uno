using Uno;
using Uno.Testing;

namespace UnoTest.Issues
{
    enum MyByteEnum : byte
    {
        Value1,
        Value2,
    }

    public class EnumBaseType
    {
        [Test]
        public void Run()
        {
            Assert.AreEqual(MyByteEnum.Value1, MyByteEnum.Value1);
        }
    }
}
