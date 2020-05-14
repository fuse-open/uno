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
            Assert.AreEqual("this", e.ToString());
        }

        [Test]
        public void Literal()
        {
            var e = new Literal("abcd");

            Assert.IsTrue(e.IsTrivial);
            Assert.AreEqual("abcd", e.ToString());
        }

        [Test]
        public void StringLiteral()
        {
            var e = new StringLiteral("hi");

            Assert.IsTrue(e.IsTrivial);
            Assert.AreEqual("\"hi\"", e.ToString());
        }

        [Test]
        public void Identifier()
        {
            var e = new Identifier("hello");

            Assert.IsTrue(e.IsTrivial);
            Assert.AreEqual("hello", e.ToString());
        }

        [Test]
        public void Binding1()
        {
            var e = new Binding(new Identifier("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("{a}", e.ToString());
        }

        [Test]
        public void Binding2()
        {
            var e = new Binding(new Literal("17"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("{17}", e.ToString());
        }

        [Test]
        public void ModeExpression1()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Clear);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("Clear \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression2()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Read);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("Read \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression3()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Write);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("Write \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression4()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("ReadClear \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression5()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.WriteClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("WriteClear \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression6()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadWrite);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("Default \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression7()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.ReadWriteClear);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("ReadWriteClear \"xyz\"", e.ToString());
        }

        [Test]
        public void ModeExpression8()
        {
            var e = new ModeExpression(new StringLiteral("xyz"), Modifier.Default);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("Default \"xyz\"", e.ToString());
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
            Assert.AreEqual("WriteClear Property this", e.ToString());
        }

        [Test]
        public void RawExpression1()
        {
            var e = new RawExpression(new Literal("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("{= a}", e.ToString());
        }

        [Test]
        public void RawExpression2()
        {
            var e = new RawExpression(new StringLiteral("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("{= \"a\"}", e.ToString());
        }

        [Test]
        public void UserDefinedUnaryOperator()
        {
            var e = new UserDefinedUnaryOperator("MyOp", new Literal("1337"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("MyOp 1337", e.ToString());
        }

        [Test]
        public void MemberExpression1()
        {
            var e = new MemberExpression(new Literal("a"), "xyz");

            Assert.IsTrue(e.IsTrivial);
            Assert.AreEqual("a.xyz", e.ToString());
        }

        [Test]
        public void MemberExpression2()
        {
            var e = new MemberExpression(new Literal("xyz"), "a");

            Assert.IsTrue(e.IsTrivial);
            Assert.AreEqual("xyz.a", e.ToString());
        }

        [Test]
        public void MemberExpression3()
        {
            var e = new MemberExpression(new RawExpression(new StringLiteral("xyz")), "a");

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("{= \"xyz\"}.a", e.ToString());
        }

        [Test]
        public void LookUpExpression1()
        {
            var e = new LookUpExpression(new Literal("a"), new Literal("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("a[b]", e.ToString());
        }

        [Test]
        public void LookUpExpression2()
        {
            var e = new LookUpExpression(new StringLiteral("123"), new StringLiteral("abc"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("\"123\"[\"abc\"]", e.ToString());
        }

        [Test]
        public void ConditionalExpression()
        {
            var e = new ConditionalExpression(new Literal("true"), new Literal("yes"), new Literal("no"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(true ? yes : no)", e.ToString());
        }

        [Test]
        public void AddExpression()
        {
            var e = new AddExpression(new StringLiteral("wheee"), new Literal("6"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(\"wheee\"+6)", e.ToString());
            Assert.AreEqual("Add", e.Name);
        }

        [Test]
        public void SubtractExpression()
        {
            var e = new SubtractExpression(new Literal("6"), new StringLiteral("wheee"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(6-\"wheee\")", e.ToString());
            Assert.AreEqual("Subtract", e.Name);
        }

        [Test]
        public void MultiplyExpression()
        {
            var e = new MultiplyExpression(new StringLiteral("hi"), new StringLiteral("bye"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(\"hi\"*\"bye\")", e.ToString());
            Assert.AreEqual("Multiply", e.Name);
        }

        [Test]
        public void DivideExpression()
        {
            var e = new DivideExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13/37)", e.ToString());
            Assert.AreEqual("Divide", e.Name);
        }

        [Test]
        public void LessThanExpression()
        {
            var e = new LessThanExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13<37)", e.ToString());
            Assert.AreEqual("LessThan", e.Name);
        }

        [Test]
        public void LessOrEqualExpression()
        {
            var e = new LessOrEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13<=37)", e.ToString());
            Assert.AreEqual("LessOrEqual", e.Name);
        }

        [Test]
        public void GreaterThanExpression()
        {
            var e = new GreaterThanExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13>37)", e.ToString());
            Assert.AreEqual("GreaterThan", e.Name);
        }

        [Test]
        public void GreaterOrEqualExpression()
        {
            var e = new GreaterOrEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13>=37)", e.ToString());
            Assert.AreEqual("GreaterOrEqual", e.Name);
        }

        [Test]
        public void EqualExpression()
        {
            var e = new EqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13==37)", e.ToString());
            Assert.AreEqual("Equal", e.Name);
        }

        [Test]
        public void NotEqualExpression()
        {
            var e = new NotEqualExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13!=37)", e.ToString());
            Assert.AreEqual("NotEqual", e.Name);
        }

        [Test]
        public void LogicalOrExpression()
        {
            var e = new LogicalOrExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13||37)", e.ToString());
            Assert.AreEqual("LogicalOr", e.Name);
        }

        [Test]
        public void LogicalAndExpression()
        {
            var e = new LogicalAndExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13&&37)", e.ToString());
            Assert.AreEqual("LogicalAnd", e.Name);
        }

        [Test]
        public void NullCoalesceExpression()
        {
            var e = new NullCoalesceExpression(new Literal("13"), new Literal("37"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(13??37)", e.ToString());
            Assert.AreEqual("NullCoalesce", e.Name);
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
            Assert.AreEqual("(a)", e.ToString());
            AssertExtensions.AreEqualValues(new Literal("a"), e.TryFold());
        }

        [Test]
        public void VectorExpression3()
        {
            var e = new VectorExpression(new StringLiteral("a"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(\"a\")", e.ToString());
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression4()
        {
            var e = new VectorExpression(new Literal("a"), new StringLiteral("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(a, \"b\")", e.ToString());
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression5()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(a, b)", e.ToString());
            AssertExtensions.AreEqualValues(new Literal("a, b"), e.TryFold());
        }

        [Test]
        public void VectorExpression6()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"), new StringLiteral("c"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(a, b, \"c\")", e.ToString());
            AssertExtensions.AreEqualValues(e, e.TryFold());
        }

        [Test]
        public void VectorExpression7()
        {
            var e = new VectorExpression(new Literal("a"), new Literal("b"), new Literal("c"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(a, b, c)", e.ToString());
            AssertExtensions.AreEqualValues(new Literal("a, b, c"), e.TryFold());
        }

        [Test]
        public void NameValuePairExpression()
        {
            var e = new NameValuePairExpression(new Literal("Fuse"), new Literal("1337"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(Fuse: 1337)", e.ToString());
            AssertExtensions.AreEqualValues(e.Name, new Literal("Fuse"));
            AssertExtensions.AreEqualValues(e.Value, new Literal("1337"));
        }

        [Test]
        public void LogicalNotExpression()
        {
            var e = new LogicalNotExpression(new StringLiteral("lol"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(!\"lol\")", e.ToString());
            Assert.AreEqual("LogicalNot", e.Name);
        }

        [Test]
        public void NegateExpression()
        {
            var e = new NegateExpression(new StringLiteral("lol"));

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("(-\"lol\")", e.ToString());
            Assert.AreEqual("Negate", e.Name);
        }

        [Test]
        public void FunctionCallExpression1()
        {
            var e = new FunctionCallExpression("someFunc", new Expression[0]);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("someFunc()", e.ToString());
        }

        [Test]
        public void FunctionCallExpression2()
        {
            var e = new FunctionCallExpression("", new Expression[0]);

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("()", e.ToString());
        }

        [Test]
        public void FunctionCallExpression3()
        {
            var e = new FunctionCallExpression("f", new[] { (Expression)new Literal("x") });

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("f(x)", e.ToString());
        }

        [Test]
        public void FunctionCallExpression4()
        {
            var e = new FunctionCallExpression("f", new [] { (Expression)new Literal("x"), new StringLiteral("y") });

            Assert.IsFalse(e.IsTrivial);
            Assert.AreEqual("f(x, \"y\")", e.ToString());
        }
    }
}
