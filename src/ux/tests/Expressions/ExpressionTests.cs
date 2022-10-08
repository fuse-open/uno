using System;
using NUnit.Framework;
using Uno.UX.Markup.UXIL.Expressions;
using Uno.UX.Markup.Tests.Helpers;

namespace Uno.UX.Markup.Tests.Expressions
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void ThisExpression()
        {
            var e = new ThisExpression();

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("this"));
        }

        [Test]
        public void Literal()
        {
            var e = new Literal("abcd");

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("abcd"));
        }

        [Test]
        public void StringLiteral()
        {
            var e = new StringLiteral("hi");

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("\"hi\""));
        }

        [Test]
        public void Identifier()
        {
            var e = new Identifier("hello");

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("hello"));
        }

        [Test]
        public void Binding1()
        {
            var e = new Binding(new Identifier("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("{a}"));
        }

        [Test]
        public void Binding2()
        {
            var e = new Binding(new Literal("17"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("{17}"));
        }

        [Test]
        public void ModeExpression1()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Clear);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("Clear \"xyz\""));
        }

        [Test]
        public void ModeExpression2()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Read);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("Read \"xyz\""));
        }

        [Test]
        public void ModeExpression3()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Write);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("Write \"xyz\""));
        }

        [Test]
        public void ModeExpression4()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("ReadClear \"xyz\""));
        }

        [Test]
        public void ModeExpression5()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.WriteClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("WriteClear \"xyz\""));
        }

        [Test]
        public void ModeExpression6()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadWrite);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("Default \"xyz\""));
        }

        [Test]
        public void ModeExpression7()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadWriteClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("ReadWriteClear \"xyz\""));
        }

        [Test]
        public void ModeExpression8()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Default);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("Default \"xyz\""));
        }

        [Test]
        public void ModeExpression9()
        {
            var e = new ModeExpression(new UserDefinedUnaryOperator("Property", new ThisExpression()), Modifier.WriteClear);

            Assert.IsFalse(e.IsTrivial);
            // This case looks a bit odd, but is actually correct. Written in UX, this would be "WriteClearProperty this", but this actually gets
            //  parsed to the tree above. We could do some magic when printing the expression string, but it makes most sense with other cases
            //  (such as "WriteClear this") to just always add the extra space between the mode and the ModeExpression's Expression, at the expense
            //  of ModeExpression + UserDefinedUnaryOperator looking slightly different.
            Assert.That(e.ToString(), Is.EqualTo("WriteClear Property this"));
        }

        [Test]
        public void RawExpression1()
        {
            var e = new RawExpression(new Literal("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("{= a}"));
        }

        [Test]
        public void RawExpression2()
        {
            var e = new RawExpression(new StringLiteral("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("{= \"a\"}"));
        }

        [Test]
        public void UserDefinedUnaryOperator()
        {
            var e = new UserDefinedUnaryOperator("MyOp", new Literal("1337"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("MyOp 1337"));
        }

        [Test]
        public void MemberExpression1()
        {
            var e = new MemberExpression(new Literal("a"), "xyz");

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("a.xyz"));
        }

        [Test]
        public void MemberExpression2()
        {
            var e = new MemberExpression(new Literal("xyz"), "a");

            Assert.IsTrue(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("xyz.a"));
        }

        [Test]
        public void MemberExpression3()
        {
            var e = new MemberExpression(new RawExpression(new StringLiteral("xyz")), "a");

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("{= \"xyz\"}.a"));
        }

        [Test]
        public void LookUpExpression1()
        {
            var e = new LookUpExpression(new Literal("a"), new Literal("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("a[b]"));
        }

        [Test]
        public void LookUpExpression2()
        {
            var e = new LookUpExpression(new StringLiteral("123"), new StringLiteral("abc"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("\"123\"[\"abc\"]"));
        }

        [Test]
        public void ConditionalExpression()
        {
            var e = new ConditionalExpression(new Literal("true"), new Literal("yes"), new Literal("no"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(true ? yes : no)"));
        }

        [Test]
        public void AddExpression()
        {
            var e = new AddExpression(new StringLiteral("wheee"), new Literal("6"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(\"wheee\"+6)"));
            Assert.That(e.Name, Is.EqualTo("Add"));
        }

        [Test]
        public void SubtractExpression()
        {
            var e = new SubtractExpression(new Literal("6"), new StringLiteral("wheee"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(6-\"wheee\")"));
            Assert.That(e.Name, Is.EqualTo("Subtract"));
        }

        [Test]
        public void MultiplyExpression()
        {
            var e = new MultiplyExpression(new StringLiteral("hi"), new StringLiteral("bye"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(\"hi\"*\"bye\")"));
            Assert.That(e.Name, Is.EqualTo("Multiply"));
        }

        [Test]
        public void DivideExpression()
        {
            var e = new DivideExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13/37)"));
            Assert.That(e.Name, Is.EqualTo("Divide"));
        }

        [Test]
        public void LessThanExpression()
        {
            var e = new LessThanExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13<37)"));
            Assert.That(e.Name, Is.EqualTo("LessThan"));
        }

        [Test]
        public void LessOrEqualExpression()
        {
            var e = new LessOrEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13<=37)"));
            Assert.That(e.Name, Is.EqualTo("LessOrEqual"));
        }

        [Test]
        public void GreaterThanExpression()
        {
            var e = new GreaterThanExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13>37)"));
            Assert.That(e.Name, Is.EqualTo("GreaterThan"));
        }

        [Test]
        public void GreaterOrEqualExpression()
        {
            var e = new GreaterOrEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13>=37)"));
            Assert.That(e.Name, Is.EqualTo("GreaterOrEqual"));
        }

        [Test]
        public void EqualExpression()
        {
            var e = new EqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13==37)"));
            Assert.That(e.Name, Is.EqualTo("Equal"));
        }

        [Test]
        public void NotEqualExpression()
        {
            var e = new NotEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13!=37)"));
            Assert.That(e.Name, Is.EqualTo("NotEqual"));
        }

        [Test]
        public void LogicalOrExpression()
        {
            var e = new LogicalOrExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13||37)"));
            Assert.That(e.Name, Is.EqualTo("LogicalOr"));
        }

        [Test]
        public void LogicalAndExpression()
        {
            var e = new LogicalAndExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13&&37)"));
            Assert.That(e.Name, Is.EqualTo("LogicalAnd"));
        }

        [Test]
        public void NullCoalesceExpression()
        {
            var e = new NullCoalesceExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(13??37)"));
            Assert.That(e.Name, Is.EqualTo("NullCoalesce"));
        }

        [Test]
        public void VectorExpression1()
        {
            var e = new VectorExpression();

            Assert.IsFalse(e.IsTrivial);
            Assert.Throws<InvalidOperationException>(() => e.ToString());
            Assert.Throws<InvalidOperationException>(() => e.TryFold());
        }

        [Test]
        public void VectorExpression2()
        {
            var e = new VectorExpression(new Literal("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(a)"));
            AssertExtensions.AreEqualValues(new Literal("a"), e.TryFold());
        }

        [Test]
        public void VectorExpression3()
        {
            var e = new VectorExpression(new StringLiteral("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(\"a\")"));
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression4()
        {
            var e = new VectorExpression(new Literal("a"), new StringLiteral("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(a, \"b\")"));
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression5()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(a, b)"));
            AssertExtensions.AreEqualValues(new Literal("a, b"), e.TryFold());
        }

        [Test]
        public void VectorExpression6()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"), new StringLiteral("c"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(a, b, \"c\")"));
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression7()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"), new Literal("c"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(a, b, c)"));
            AssertExtensions.AreEqualValues(new Literal("a, b, c"), e.TryFold());
        }

        [Test]
        public void NameValuePairExpression()
        {
            var e = new NameValuePairExpression(new Literal("Fuse"), new Literal("1337"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(Fuse: 1337)"));
            AssertExtensions.AreEqualValues(e.Name, new Literal("Fuse"));
            AssertExtensions.AreEqualValues(e.Value, new Literal("1337"));
        }

        [Test]
        public void LogicalNotExpression()
        {
            var e = new LogicalNotExpression(new StringLiteral("lol"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(!\"lol\")"));
            Assert.That(e.Name, Is.EqualTo("LogicalNot"));
        }

        [Test]
        public void NegateExpression()
        {
            var e = new NegateExpression(new StringLiteral("lol"));

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("(-\"lol\")"));
            Assert.That(e.Name, Is.EqualTo("Negate"));
        }

        [Test]
        public void FunctionCallExpression1()
        {
            var e = new FunctionCallExpression("someFunc", new Expression[0]);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("someFunc()"));
        }

        [Test]
        public void FunctionCallExpression2()
        {
            var e = new FunctionCallExpression("", new Expression[0]);

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("()"));
        }

        [Test]
        public void FunctionCallExpression3()
        {
            var e = new FunctionCallExpression("f", new[] { (Expression)new Literal("x") });

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("f(x)"));
        }

        [Test]
        public void FunctionCallExpression4()
        {
            var e = new FunctionCallExpression("f", new [] { (Expression)new Literal("x"), new StringLiteral("y") });

            Assert.IsFalse(e.IsTrivial);
            Assert.That(e.ToString(), Is.EqualTo("f(x, \"y\")"));
        }
    }
}
