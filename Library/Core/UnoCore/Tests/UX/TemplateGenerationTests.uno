using Uno.Testing;
using UXHelpers;

namespace Uno.UX.Tests
{
    public class TemplateGenerationTests
    {
        [Test]
        public void NestedTemplateRef()
        {
            var e = new NestedTemplateRef();
            e.Root();

            // Traverse the resulting object graph to make sure we got the right reference
            //  Remember that Children will contain both templates and children, but the order will be deterministic
            var expectedRef = ((RootObject)((RootObject)e.Children[0]).Children[1]).Children[0];
            var actualRef = ((RefObject)((RootObject)((RootObject)((RootObject)e.Children[0]).Children[1]).Children[1]).Children[1]).Object;
            Assert.AreNotEqual(expectedRef, null);
            Assert.AreNotEqual(actualRef, null);
            Assert.AreEqual(expectedRef, actualRef);
        }

        [Test]
        public void SealedClassTemplate()
        {
            // SealedClassTemplate will fail to compile if we're unable to create templates with sealed base classes
            var instance = new SealedClassTemplate();
        }

        [Test]
        public void InnerClassTemplate()
        {
            // InnerClassTemplate will fail to compile if we're unable to create templates with inner classes as base classes
            var instance = new InnerClassTemplate();
        }
    }
}