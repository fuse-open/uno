using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;
using Uno.Internal;

namespace Uno.Test
{
    public class FormatStringTokenizerTest
    {
        [Test]
        public void EmptyFormatStringTokenizesToEmptyList()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("");
            PrintList(actual, "EmptyFormatStringTokenizesToEmptyList");
            Assert.AreEqual(0, actual.Count);
            Assert.AreCollectionsEqual(new List<FormatStringToken>(), actual);
        }

        [Test]
        public void NoFormatItemsTokenizesToOriginalString()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("foo");
            PrintList(actual, "NoFormatItemsTokenizesToOriginalString");
            var expected = new List<FormatStringToken>
            {
                new FormatStringLiteral("foo"),
            };
            Assert.AreCollectionsEqual(expected, actual);
        }

        [Test]
        public void WithFormatItemsInTheMiddle()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("foo{0}bar");
            PrintList(actual, "WithFormatItemsInTheMiddle");
            var expected = new List<FormatStringToken>
            {
                new FormatStringLiteral("foo"),
                new FormatStringItem("{0}"),
                new FormatStringLiteral("bar"),
            };
            PrintList(expected, "Expected");
            Assert.AreCollectionsEqual(expected, actual);
        }

        [Test]
        public void WithFormatItemsAtBeginning()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("{0}bar");
            PrintList(actual, "WithFormatItemsAtBeginning");
            var expected = new List<FormatStringToken>
            {
                new FormatStringItem("{0}"),
                new FormatStringLiteral("bar"),
            };
            Assert.AreCollectionsEqual(expected, actual);
        }

        [Test]
        public void WithFormatItemsAtEnd()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("foo{0}");
            PrintList(actual, "WithFormatItemsAtEnd");
            var expected = new List<FormatStringToken>
            {
                new FormatStringLiteral("foo"),
                new FormatStringItem("{0}"),
            };
            Assert.AreCollectionsEqual(expected, actual);
        }

        [Test]
        public void UnclosedCurlyBracketThrows()
        {
            Assert.Throws<FormatException>(FormatWithUnclosedBracket1);
            Assert.Throws<FormatException>(FormatWithUnclosedBracket2);
        }

        [Test]
        public void UnopenedCurlyBracketThrows()
        {
            Assert.Throws<FormatException>(FormatWithUnopenedBracket1);
            Assert.Throws<FormatException>(FormatWithUnopenedBracket2);
        }

        private void FormatWithUnclosedBracket1()
        {
            FormatStringTokenizer.TokenizeFormatString("foo{");
        }

        private void FormatWithUnclosedBracket2()
        {
            FormatStringTokenizer.TokenizeFormatString("foo{10");
        }

        private void FormatWithUnopenedBracket1()
        {
            FormatStringTokenizer.TokenizeFormatString("}");
        }

        private void FormatWithUnopenedBracket2()
        {
            FormatStringTokenizer.TokenizeFormatString("foo}bar");
        }

        [Test]
        public void CurlyBracketsCanBeEscaped()
        {
            var actual = FormatStringTokenizer.TokenizeFormatString("{{foo{0}{{");
            PrintList(actual, "CurlyBracketsCanBeEscaped");
            var expected = new List<FormatStringToken>
            {
                new FormatStringLiteral("{foo"),
                new FormatStringItem("{0}"),
                new FormatStringLiteral("{"),
            };
            Assert.AreCollectionsEqual(expected, actual);
        }

        [Test]
        public void FormatStringItemKnowsItsNumber()
        {
            Assert.AreEqual(0, new FormatStringItem("{0}").Number);
            Assert.AreEqual(1, new FormatStringItem("{1}").Number);
            //Assert.AreEqual(2, new FormatStringItem("{2,10}").Number);
            //Assert.AreEqual(3, new FormatStringItem("{3,10:D}").Number);
            //Assert.AreEqual(4, new FormatStringItem("{4:D}").Number);
        }


        private static void PrintList(List<FormatStringToken> list, string header="")
        {
//             debug_log header;
//             foreach(var item in list)
//             {
//                 debug_log "  <" + item + ",\"" + item.Lexeme + "\">";
//             }
        }
    }
}
