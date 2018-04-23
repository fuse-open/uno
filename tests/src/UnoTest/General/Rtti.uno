using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Rtti
    {
        class Foo
        {
        }

        class Bar
        {
        }

        class FooChild : Foo
        {
        }

        class FooChildChild : FooChild
        {
        }

        class BarChild : Bar
        {
        }

        [Test]
        public void Run()
        {
            var foo = new Foo();
            Assert.IsTrue(foo is Foo);
            Assert.IsFalse(foo is FooChild);
            Assert.IsFalse(foo is FooChildChild);
            Assert.IsFalse(foo is Bar);

            var bar = new Bar();
            Assert.IsTrue(bar is Bar);
            Assert.IsFalse(bar is BarChild);
            Assert.IsFalse(bar is Foo);
            Assert.IsFalse(bar is FooChild);
            Assert.IsFalse(bar is FooChildChild);

            var fooChild = new FooChild();
            Assert.IsTrue(fooChild is FooChild);
            Assert.IsTrue(fooChild is Foo);
            Assert.IsFalse(fooChild is FooChildChild);
            Assert.IsFalse(fooChild is Bar);
            Assert.IsFalse(fooChild is BarChild);

            var fooChildChild = new FooChildChild();
            Assert.IsTrue(fooChildChild is FooChildChild);
            Assert.IsTrue(fooChildChild is FooChild);
            Assert.IsTrue(fooChildChild is Foo);
            Assert.IsFalse(fooChildChild is Bar);
            Assert.IsFalse(fooChildChild is BarChild);

            var barChild = new BarChild();
            Assert.IsTrue(barChild is BarChild);
            Assert.IsTrue(barChild is Bar);
            Assert.IsFalse(barChild is Foo);
            Assert.IsFalse(barChild is FooChild);
            Assert.IsFalse(barChild is FooChildChild);
        }
    }
}
