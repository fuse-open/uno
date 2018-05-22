using Uno;
using Uno.Testing;

namespace Uno.UX.Tests
{
    public class UXTestFoo
    {
        [UXContent]
        public UXTestBar Bar { get; set; }
    }

    [UXTestBootstrapperFor("Uno.UX.Tests.UXTestFoo")]
    public class UXTestBar
    {
        [UXContent]
        public UXTestFoo Foo { get; set; }

        public bool TestWasRun = false;
        public void RunTest() 
        {
            TestWasRun = true;
        }
    }

    public class UXTestTests
    {
        [Test]
        public void Test()
        {
            var e = new UXHelpers.UXTestTest();

            Assert.IsTrue(e is UXTestBar);
            Assert.IsTrue(e.Foo is UXTestFoo);
            Assert.IsTrue(e.innerObj is UXTestBar);
            Assert.IsTrue(((UXTestFoo)e.Foo).Bar is UXTestBar);
            
            Assert.IsFalse(((UXTestBar)e).TestWasRun);
            e.Run();
            Assert.IsTrue(((UXTestBar)e).TestWasRun);
        }

    }
}

