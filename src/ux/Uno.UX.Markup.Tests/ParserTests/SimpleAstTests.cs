using System;
using System.Linq;
using NUnit.Framework;
using Uno.UX.Markup.AST;
using Uno.UX.Markup.Tests.Helpers;

namespace Uno.UX.Markup.Tests.ParserTests
{
    [TestFixture]
    public class SimpleAstTests
    {
        [Test]
        public void SingleElement()
        {
            const string docName = "a.ux";
            const string docCode = "<Something />";

            var element = TestHelpers.Parse(docName, docCode);

            AssertElement(element, ElementType.Object, "Something", new FileSourceInfo(docName, 1), typeof(UnspecifiedGenerator), 0);
        }

        [Test]
        public void SingleElementWithSingleProperty()
        {
            const string docName = "b.ux";
            const string docCode = "<TheElement SomeProp=\"SomeValue\" />";

            var element = TestHelpers.Parse(docName, docCode);
            AssertElement(element, ElementType.Object, "TheElement", new FileSourceInfo(docName, 1), typeof(UnspecifiedGenerator), 0);
            Assert.AreEqual(1, element.Properties.Count());

            var prop = element.Properties.ElementAt(0);
            AssertProperty(prop, "SomeProp", "SomeValue");
        }

        [Test]
        public void ShallowTree()
        {
            const string docName = "c.ux";
            const string docCode = @"

<TheParent>
    <FirstChild />
    <SecondChild />
</TheParent>

";

            var element = TestHelpers.Parse(docName, docCode);
            AssertElement(element, ElementType.Object, "TheParent", new FileSourceInfo(docName, 3), typeof(UnspecifiedGenerator), 2);

            var firstChild = (Element)element.Children.ElementAt(0);
            AssertElement(firstChild, ElementType.Object, "FirstChild", new FileSourceInfo(docName, 4), typeof(UnspecifiedGenerator), 0);

            var secondChild = (Element)element.Children.ElementAt(1);
            AssertElement(secondChild, ElementType.Object, "SecondChild", new FileSourceInfo(docName, 5), typeof(UnspecifiedGenerator), 0);
        }

        [Test]
        public void SingleElementWithSingleDependency()
        {
            const string docName = "d.ux";
            const string docCode = @"

<TheParent>
    <TheType ux:Dependency=""The name"" />
</TheParent>

";

            var element = TestHelpers.Parse(docName, docCode);
            AssertElement(element, ElementType.Object, "TheParent", new FileSourceInfo(docName, 3), typeof(UnspecifiedGenerator), 1);

            var dep = (Element)element.Children.ElementAt(0);
            AssertElement(dep, ElementType.Dependency, "TheType", new FileSourceInfo(docName, 4), typeof(UnspecifiedGenerator), 0);
        }

        void AssertElement(Element element, ElementType elementType, string typeName, FileSourceInfo source, Type generatorType, int childCount)
        {
            Assert.AreEqual(elementType, element.ElementType);
            Assert.AreEqual(typeName, element.TypeName);
            Assert.AreEqual(source, element.Source);
            Assert.AreEqual(generatorType, element.Generator.GetType());
            Assert.AreEqual(childCount, element.Children.Count());
        }

        void AssertProperty(Property prop, string name, string value, string ns = "")
        {
            Assert.AreEqual(name, prop.Name);
            Assert.AreEqual(value, prop.Value);
            Assert.AreEqual(ns, prop.Namespace);
        }
    }
}
