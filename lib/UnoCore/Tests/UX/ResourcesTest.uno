using Uno.Testing;

namespace Uno.UX.Tests
{
	public class TestPanel
	{
		public float4 Margin { get; set; }
	}

    public class ResourcesTest
    {

        [Test]
        public void ResourcesBasics()
        {
            var e1 = new UXResourcesTest.FooPanel();
            var e2 = new UXResourcesTest2.FooPanel();
            var m = (float4)UnoCoreTest_Resources1_res.UXResourcesTestFooMargin;

            Assert.AreEqual(float4(10, 8, 9, 3), m);
            Assert.AreEqual(float4(10, 8, 9, 3), e1.Margin);
            Assert.AreEqual(float4(10, 4, 9, 3), e2.Margin);
        }
    }
}
