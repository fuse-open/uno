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
            Assert.That(element.Properties.Count(), Is.EqualTo(1));

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
            Assert.That(element.ElementType, Is.EqualTo(elementType));
            Assert.That(element.TypeName, Is.EqualTo(typeName));
            Assert.That(element.Source, Is.EqualTo(source));
            Assert.That(element.Generator.GetType(), Is.EqualTo(generatorType));
            Assert.That(element.Children.Count(), Is.EqualTo(childCount));
        }

        void AssertProperty(Property prop, string name, string value, string ns = "")
        {
            Assert.That(prop.Name, Is.EqualTo(name));
            Assert.That(prop.Value, Is.EqualTo(value));
            Assert.That(prop.Namespace, Is.EqualTo(ns));
        }
    }
}
