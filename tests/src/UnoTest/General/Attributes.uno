using Uno;
using Uno.Testing;
using UnoTest.General;

namespace UnoTest.General
{
    public class Attributes
    {
        public class FooAttribute : Attribute
        {
        }

        [Foo]
        [AttributeUsage(AttributeTargets.Class)]
        class BarAttribute : Attribute
        {
        }

        [Foo]
        [Bar]
        class GenericType<T>
        {
        }

        [Bar]
        abstract class AbstractType
        {
        }

        [Test]
        public void Run()
        {
        }
    }
}
