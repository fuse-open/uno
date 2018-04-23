using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class TypeOf
    {
        [Test]
        public void Run()
        {
            Assert.IsTrue(new TypeOf().GetType() == typeof(TypeOf));
            // #268
            //Assert.IsTrue(new TypeOf().GetType().GetType() == typeof(Type));
        }
    }
}
