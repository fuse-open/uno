using System;
using NUnit.Framework;
using Uno.UX.Markup.UXIL.Expressions;
using Uno.UX.Markup.Tests.Helpers;

namespace Uno.UX.Markup.Tests.Expressions
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ThisExpression()
        {
            AssertParseResult("this", new ThisExpression());
        }

        [Test]
        public void NumericLiteral()
        {
            AssertParseResult(" 17", new Literal("17"));
            AssertParseResult("123.456", new Literal("123.456"));
            AssertParseResult("123.", new Literal("123."));
            AssertParseResult(".0003", new Literal(".0003"));
            AssertParseResult("0.0", new Literal("0.0"));
            AssertParseResult("0.34f", new Literal("0.34f"));
            AssertParseResult("-12", new Literal("-12"));
            AssertParseResult("(12,24)", new Literal("12, 24")); // TODO: Why is this a literal and not a VectorExpression?

            AssertParseThrows(".");

            // TODO: Test literals with units
        }

        [Test]
        public void ColorCodeLiteral()
        {
            AssertParseResult("#abcdef", new Literal("#abcdef"));
            AssertParseResult("#0123456789abcdef", new Literal("#0123456789abcdef")); // TODO: Not sure if we should allow such literals, but they're valid according to the current lexer/parser

            AssertParseThrows("#");
            AssertParseThrows("#x");
        }

        [Test]
        public void StringLiteral()
        {
            AssertParseResult("\"\"", new StringLiteral(""));
            AssertParseResult(" \"hello\"", new StringLiteral("hello"));
            AssertParseResult("\"'\" ", new StringLiteral("'"));
            AssertParseResult("\"''\" ", new StringLiteral("''"));
            //AssertParseResult("\"\\\"\"", new StringLiteral("\\\"")); // TODO: Escaping quotes doesn't seem to work properly atm; that should probably be fixed

            AssertParseThrows("\"");

            // TODO: Test escape sequences
            // TODO: Test hex literals
            // TODO: Test unicode literals
        }

        [Test]
        public void CharLiteral()
        {
            AssertParseResult("'a'", new StringLiteral("a"));
            AssertParseResult("'b'", new StringLiteral("b"));
            AssertParseResult("'x'", new StringLiteral("x"));
            AssertParseResult("'🍝'", new StringLiteral("🍝"));
            AssertParseResult("''", new StringLiteral("")); // TODO: This should probably yield an error rather than an empty StringLiteral
            AssertParseResult("'this should not work'", new StringLiteral("this should not work")); // TODO: This should not work (multiple characters)
            //AssertParseResult("'\\''", new StringLiteral("'")); // TODO: Results in incorrect string (should be single char, results in backslash)
            AssertParseResult("'\\\"'", new StringLiteral("\\\""));
            AssertParseResult("'\\\\'", new StringLiteral("\\\\")); // Intracks
            AssertParseResult("'\\0'", new StringLiteral("\\0"));
            AssertParseResult("'\\a'", new StringLiteral("\\a"));
            AssertParseResult("'\\b'", new StringLiteral("\\b"));
            AssertParseResult("'\\f'", new StringLiteral("\\f"));
            AssertParseResult("'\\n'", new StringLiteral("\\n"));
            AssertParseResult("'\\r'", new StringLiteral("\\r"));
            AssertParseResult("'\\t'", new StringLiteral("\\t"));
            AssertParseResult("'\\v'", new StringLiteral("\\v"));

            AssertParseResult("'\\x0'", new StringLiteral("\\x0"));
            AssertParseResult("'\\xa'", new StringLiteral("\\xa"));
            AssertParseResult("'\\xab'", new StringLiteral("\\xab"));
            AssertParseResult("'\\xabc'", new StringLiteral("\\xabc"));
            AssertParseResult("'\\xabcd'", new StringLiteral("\\xabcd"));
            AssertParseResult("'\\x0000'", new StringLiteral("\\x0000"));
            AssertParseResult("'\\xffff'", new StringLiteral("\\xffff"));
            AssertParseResult("'\\xFFFF'", new StringLiteral("\\xFFFF"));

            AssertParseThrows("'");
            //AssertParseThrows("'\\q'"); // TODO: Unrecognized escape codes should throw
            //AssertParseThrows("'\\x'"); // TODO: Empty hex escape sequence should throw

            // TODO: Test more hex literal corner cases
            // TODO: Test unicode literals
        }

        [Test]
        public void TrueLiteral()
        {
            AssertParseResult("true", new Literal("true"));
        }

        [Test]
        public void FalseLiteral()
        {
            AssertParseResult("false", new Literal("false"));
        }

        [Test]
        public void Identifier()
        {
            AssertParseResult("_", new Identifier("_"));
            AssertParseResult("a", new Identifier("a"));
            AssertParseResult("A", new Identifier("A"));
            AssertParseResult("abcdefg", new Identifier("abcdefg"));
            AssertParseResult("AbCdEfG", new Identifier("AbCdEfG"));
            AssertParseResult("aBcDeFg", new Identifier("aBcDeFg"));
            AssertParseResult("_abcdefg", new Identifier("_abcdefg"));
            AssertParseResult("_AbCdEfG", new Identifier("_AbCdEfG"));
            AssertParseResult("_aBcDeFg", new Identifier("_aBcDeFg"));
            AssertParseResult("______", new Identifier("______"));
            AssertParseResult("_123", new Identifier("_123"));
            AssertParseResult("a_1_b_c_3_9_8_774628sdf_____", new Identifier("a_1_b_c_3_9_8_774628sdf_____"));
            AssertParseResult("_________0", new Identifier("_________0"));
            AssertParseResult("  abcdefg", new Identifier("abcdefg"));
            AssertParseResult(" AbCdEfG ", new Identifier("AbCdEfG"));
            AssertParseResult("AbCdEfG     ", new Identifier("AbCdEfG"));
        }

        [Test]
        public void Binding()
        {
            AssertParseResult("{a}", new Binding(new Identifier("a")));
            AssertParseResult("{ \"yooo\" }", new Binding(new StringLiteral("yooo")));
            AssertParseResult(" { true}  ", new Binding(new Literal("true")));
            AssertParseResult("{}", new Binding(new Identifier(""))); // TODO: Perhaps binding's expression should be nullable to allow for this instead of using an empty identifier?
            AssertParseResult("   {\t\t}\t", new Binding(new Identifier("")));

            AssertParseThrows("{");
            AssertParseThrows("{6");
        }

        [Test]
        public void ModeExpression()
        {
            AssertParseResult("ReadClear hi", new ModeExpression(new Identifier("hi"), Modifier.ReadClear));
            AssertParseResult("WriteClear {}", new ModeExpression(new Binding(new Identifier("")), Modifier.WriteClear));
            AssertParseResult("Clear        this ", new ModeExpression(new ThisExpression(), Modifier.ReadWriteClear)); // Clear actually becomes ReadWriteClear!
            AssertParseResult("Read books", new ModeExpression(new Identifier("books"), Modifier.Read));
            AssertParseResult("   Write \t code", new ModeExpression(new Identifier("code"), Modifier.Write));
            AssertParseResult("ReadProperty this", new ModeExpression(new UserDefinedUnaryOperator("Property", new ThisExpression()), Modifier.Read));
            AssertParseResult("Clearthis this", new ModeExpression(new UserDefinedUnaryOperator("this", new ThisExpression()), Modifier.ReadWriteClear)); // This is an odd corner case, but fits grammatically

            AssertParseThrows("ReadClear 1234");
            AssertParseThrows("Write !this");
        }

        [Test]
        public void UserDefinedUnaryOperator()
        {
            AssertParseResult("MyCustomProperty arg", new UserDefinedUnaryOperator("MyCustomProperty", new Identifier("arg")));
            AssertParseResult("___any_valid_identifier this", new UserDefinedUnaryOperator("___any_valid_identifier", new ThisExpression()));
            AssertParseResult("Snapshot{}", new UserDefinedUnaryOperator("Snapshot", new Binding(new Identifier(""))));
            // The "Snapshot" prefix is hard-coded in the parser
            AssertParseResult("SnapshotSomeOtherThing{}", new UserDefinedUnaryOperator("Snapshot", new UserDefinedUnaryOperator("SomeOtherThing", new Binding(new Identifier("")))));
            AssertParseResult("SnapshotProperty {this}", new UserDefinedUnaryOperator("Snapshot", new UserDefinedUnaryOperator("Property", new Binding(new ThisExpression()))));
            // Yes, they nest :)
            AssertParseResult("a b c", new UserDefinedUnaryOperator("a", new UserDefinedUnaryOperator("b", new Identifier("c"))));
            AssertParseResult("a b c d e", new UserDefinedUnaryOperator("a", new UserDefinedUnaryOperator("b", new UserDefinedUnaryOperator("c", new UserDefinedUnaryOperator("d", new Identifier("e"))))));
            AssertParseResult("a Snapshotb mySnapshotc this", new UserDefinedUnaryOperator("a", new UserDefinedUnaryOperator("Snapshot", new UserDefinedUnaryOperator("b", new UserDefinedUnaryOperator("mySnapshotc", new ThisExpression())))));

            AssertParseThrows("some_identifier 5"); // can only accept expr's starting with identifier, this, or {
            AssertParseThrows("a this b"); // `this` is not an identifier and shouldn't nest
        }

        [Test]
        public void RawExpression()
        {
            AssertParseResult("{= a}", new RawExpression(new Identifier("a")));
            AssertParseResult(" { = 1234 }", new RawExpression(new Literal("1234")));

            AssertParseThrows("{=");
            AssertParseThrows("{=}");
            AssertParseThrows("{= }");
        }

        [Test]
        public void MemberExpression()
        {
            AssertParseResult(" a . b", new MemberExpression(new Identifier("a"), "b"));
            AssertParseResult("this.that", new MemberExpression(new ThisExpression(), "that"));
            AssertParseResult("12.seventy", new MemberExpression(new Literal("12"), "seventy"));

            AssertParseThrows("hi.");
            AssertParseThrows("hi.12");
        }

        [Test]
        public void LookupExpression()
        {
            AssertParseResult("a [ b ]", new LookUpExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this[{\"that\"}]", new LookUpExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("[");
            AssertParseThrows("]");
            AssertParseThrows("[]");
            //AssertParseThrows("a[]"); // TODO: Should throw
            AssertParseThrows("[b]");
        }

        [Test]
        public void ConditionalExpression()
        {
            AssertParseResult("a ? b : c", new ConditionalExpression(new Identifier("a"), new Identifier("b"), new Identifier("c")));
            AssertParseResult("\"hi\" ? -this : 12", new ConditionalExpression(new StringLiteral("hi"), new NegateExpression(new ThisExpression()), new Literal("12")));

            AssertParseThrows("a?");
            AssertParseThrows("?b:c");
            AssertParseThrows("a?b");
            AssertParseThrows("a?b:");
        }

        [Test]
        public void AddExpression()
        {
            AssertParseResult("a + b", new AddExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this + {\"that\"}", new AddExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("+");
            AssertParseThrows("a+");
            AssertParseThrows("+b");
        }

        [Test]
        public void SubtractExpression()
        {
            AssertParseResult("a - b", new SubtractExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this - {\"that\"}", new SubtractExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("-");
            AssertParseThrows("a-");
        }

        [Test]
        public void MultiplyExpression()
        {
            AssertParseResult("a * b", new MultiplyExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this * {\"that\"}", new MultiplyExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("*");
            AssertParseThrows("a*");
            AssertParseThrows("*b");
        }

        [Test]
        public void DivideExpression()
        {
            AssertParseResult("a / b", new DivideExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this / {\"that\"}", new DivideExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("/");
            AssertParseThrows("a/");
            AssertParseThrows("/b");
        }

        [Test]
        public void LessThanExpression()
        {
            AssertParseResult("a < b", new LessThanExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this < {\"that\"}", new LessThanExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("<");
            AssertParseThrows("a<");
            AssertParseThrows("<b");
        }

        [Test]
        public void LessOrEqualExpression()
        {
            AssertParseResult("a <= b", new LessOrEqualExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this <= {\"that\"}", new LessOrEqualExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("<=");
            AssertParseThrows("a<=");
            AssertParseThrows("<=b");
        }

        [Test]
        public void GreaterThanExpression()
        {
            AssertParseResult("a > b", new GreaterThanExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this > {\"that\"}", new GreaterThanExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows(">");
            AssertParseThrows("a>");
            AssertParseThrows(">b");
        }

        [Test]
        public void GreaterOrEqualExpression()
        {
            AssertParseResult("a >= b", new GreaterOrEqualExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this >= {\"that\"}", new GreaterOrEqualExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows(">=");
            AssertParseThrows("a>=");
            AssertParseThrows(">=b");
        }

        [Test]
        public void EqualExpression()
        {
            AssertParseResult("a == b", new EqualExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this == {\"that\"}", new EqualExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("==");
            AssertParseThrows("a==");
            AssertParseThrows("==b");
        }

        [Test]
        public void NotEqualExpression()
        {
            AssertParseResult("a != b", new NotEqualExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this != {\"that\"}", new NotEqualExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("!=");
            AssertParseThrows("a!=");
            AssertParseThrows("!=b");
        }

        [Test]
        public void LogicalOrExpression()
        {
            AssertParseResult("a || b", new LogicalOrExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this || {\"that\"}", new LogicalOrExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("||");
            AssertParseThrows("a||");
            AssertParseThrows("||b");
        }

        [Test]
        public void LogicalAndExpression()
        {
            AssertParseResult("a && b", new LogicalAndExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("this && {\"that\"}", new LogicalAndExpression(new ThisExpression(), new Binding(new StringLiteral("that"))));

            AssertParseThrows("&&");
            AssertParseThrows("a&&");
            AssertParseThrows("&&b");
        }

        [Test]
        public void NullCoalesceExpression()
        {
            AssertParseResult("a??b", new NullCoalesceExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult(" this ?? -that", new NullCoalesceExpression(new ThisExpression(), new NegateExpression(new Identifier("that"))));

            AssertParseThrows("??");
            AssertParseThrows("a??");
            AssertParseThrows("??b");
        }

        [Test]
        public void VectorExpression()
        {
            AssertParseResult("(hi,24)", new VectorExpression(new Identifier("hi"), new Literal("24")));
            AssertParseResult(" ( a   , b   ,c )", new VectorExpression(new Identifier("a"), new Identifier("b"), new Identifier("c")));
            AssertParseResult("( this.happy, 0, {Read bin})  ", new VectorExpression(new MemberExpression(new ThisExpression(), "happy"), new Literal("0"), new Binding(new ModeExpression(new Identifier("bin"), Modifier.Read))));

            AssertParseThrows("()");
            AssertParseThrows("(, )");
            AssertParseThrows("(a, )");
        }

        [Test]
        public void NameValuePairExpression()
        {
            AssertParseResult("a: b", new NameValuePairExpression(new Identifier("a"), new Identifier("b")));
            AssertParseResult("  a.b  : c.d  ", new NameValuePairExpression(new MemberExpression(new Identifier("a"), "b"), new MemberExpression(new Identifier("c"), "d")));

            AssertParseThrows(":");
            AssertParseThrows("a.b:");
            AssertParseThrows(":c.d");
        }

        [Test]
        public void LogicalNotExpression()
        {
            AssertParseResult("!this", new LogicalNotExpression(new ThisExpression()));

            AssertParseThrows("!");
        }

        [Test]
        public void NegateExpression()
        {
            AssertParseResult("-yes", new NegateExpression(new Identifier("yes")));
            AssertParseResult("-(no)", new NegateExpression(new Identifier("no")));
            //AssertParseResult(" - - - no ", new NegateExpression(new NegateExpression(new NegateExpression(new Identifier("no"))))); // TODO: This should probably work

            AssertParseThrows("-");
            AssertParseThrows("--");
        }

        [Test]
        public void FunctionCallExpression()
        {
            AssertParseResult("a()", new FunctionCallExpression("a", new Expression[] { }));
            AssertParseResult("   asdf  ( )   ", new FunctionCallExpression("asdf", new Expression[] { }));
            AssertParseResult("foo(a)", new FunctionCallExpression("foo", new Expression[] { new Identifier("a") }));
            AssertParseResult("foo(a,b,c)", new FunctionCallExpression("foo", new Expression[] { new Identifier("a"), new Identifier("b"), new Identifier("c") }));
            AssertParseResult("foo(-this, {a})", new FunctionCallExpression("foo", new Expression[] { new NegateExpression(new ThisExpression()), new Binding(new Identifier("a")) }));

            AssertParseThrows("(");
            AssertParseThrows("a(");
            AssertParseThrows("a(,)");
        }

        // TODO: Thoroughly test using stringMode = true
        void AssertParseResult(string code, Expression expected, bool stringMode = false)
        {
            var src = new Uno.Compiler.SourceFile("test.cs", code);
            var expr = Parser.Parse(src, code, stringMode);
            AssertExtensions.AreEqualValues(expected, expr);
        }

        void AssertParseThrows(string code, bool stringMode = false)
        {
            AssertParseThrows<Exception>(code, stringMode);
        }

        void AssertParseThrows<T>(string code, bool stringMode = false) where T : Exception
        {
            var src = new Uno.Compiler.SourceFile("test.cs", code);
            Assert.Throws<T>(() => Parser.Parse(src, code, stringMode));
        }
    }

    class TestErrorLog : Common.IMarkupErrorLog
    {
        public void ReportError(string message) { }
        public void ReportWarning(string message) { }

        public void ReportError(string path, int line, string message) { }
        public void ReportWarning(string path, int line, string message) { }
    }
}
