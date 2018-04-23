using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;

namespace Uno.Test
{
    public class CharTest
    {
        [Test]
        public void WhiteSpaceIsWhiteSpace()
        {
            var spaceSeparators = "\u0020\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u202F\u205F\u3000";
            var lineSpearators = "\u2028";
            var paragraphSeparators = "\u2029";
            var characterTabulations = "\u0009\u000A\u000B\u000C\u000D\u0085\u00A0";
            string whiteSpace = spaceSeparators + lineSpearators + paragraphSeparators + characterTabulations;
            foreach (char c in whiteSpace)
            {
                Assert.IsTrue(Char.IsWhiteSpace(c), "Char " + (int)c + " failed");
            }
        }

        [Test]
        public void NonWhiteSpaceIsNotWhiteSpace()
        {
            foreach (var c in "1234567890abcdefghijklmnopqrstuvwxyzæøå^-_;.()=/")
            {
                Assert.IsTrue(!Char.IsWhiteSpace(c));
            }
        }

        [Test]
        public void ToUpper()
        {
            Assert.AreEqual( 'A' , Char.ToUpper('a'));
            Assert.AreEqual( '.' , Char.ToUpper('.'));
            Assert.AreEqual( 'A' , Char.ToUpper('a'));
            Assert.AreEqual( 'A' , Char.ToUpper('A'));
            Assert.AreEqual( '\u00c5' , Char.ToUpper('\u00e5')); //æ
        }

        [Test]
        public void ToLower()
        {
            Assert.AreEqual( '.' , Char.ToLower('.'));
            Assert.AreEqual( 'a' , Char.ToLower('a'));
            Assert.AreEqual( 'a' , Char.ToLower('A'));
            Assert.AreEqual( '\u00e5' , Char.ToLower('\u00c5')); //æ
        }

        [Test]
        public void IsDigit()
        {
            Assert.IsTrue(char.IsDigit('0'));
            Assert.IsTrue(char.IsDigit('5'));
            Assert.IsTrue(char.IsDigit('9'));
            Assert.IsTrue(char.IsDigit('\u0030'));

            Assert.IsFalse(char.IsDigit('t'));
            Assert.IsFalse(char.IsDigit('-'));
        }

        [Test]
        public void IsLetter()
        {
            //russian chars
            Assert.IsTrue(char.IsLetter('ш'));
            Assert.IsTrue(char.IsLetter('\u0448'));
            Assert.IsTrue(char.IsLetter('Ш'));

            //english chars
            Assert.IsTrue(char.IsLetter('t'));
            Assert.IsTrue(char.IsLetter('T'));

            //greek char
            Assert.IsTrue(char.IsLetter('\u03AD'));

            Assert.IsFalse(char.IsLetter('-'));
            Assert.IsFalse(char.IsLetter('9'));
            Assert.IsFalse(char.IsLetter('\u0030'));
        }

        [Test]
        public void IsLetterOrDigit()
        {
            Assert.IsTrue(char.IsLetterOrDigit('t'));
            Assert.IsTrue(char.IsLetterOrDigit('T'));
            Assert.IsTrue(char.IsLetterOrDigit('9'));
            Assert.IsFalse(char.IsLetterOrDigit('-'));
        }

        [Test]
        public void IsPunctuation()
        {
            Assert.IsTrue(char.IsPunctuation(','));
            Assert.IsTrue(char.IsPunctuation('#'));
            Assert.IsTrue(char.IsPunctuation('&'));
            Assert.IsTrue(char.IsPunctuation('.'));
            Assert.IsTrue(char.IsPunctuation(','));
            Assert.IsTrue(char.IsPunctuation('!'));
            Assert.IsTrue(char.IsPunctuation('-'));
            Assert.IsFalse(char.IsPunctuation('~'));
            Assert.IsFalse(char.IsPunctuation('t'));
            Assert.IsFalse(char.IsPunctuation('6'));
            Assert.IsFalse(char.IsPunctuation(' '));
        }

        [Test]
        public void IsControl()
        {
            Assert.IsTrue(char.IsControl('\u0003'));
            Assert.IsTrue(char.IsControl('\u007F'));
            Assert.IsTrue(char.IsControl('\u0088'));
            Assert.IsFalse(char.IsControl(' '));
            Assert.IsFalse(char.IsControl('4'));
        }

        [Test]
        public void IsLower()
        {
            Assert.IsTrue(char.IsLower('a'));
            Assert.IsTrue(char.IsLower('z'));
            Assert.IsTrue(char.IsLower('ш')); //russian char
            Assert.IsFalse(char.IsLower('5'));
            Assert.IsFalse(char.IsLower('A'));
            Assert.IsFalse(char.IsLower('Z'));
        }

        [Test]
        public void IsUpper()
        {
            Assert.IsTrue(char.IsUpper('A'));
            Assert.IsTrue(char.IsUpper('Z'));
            Assert.IsTrue(char.IsUpper('Ш')); //russian char
            Assert.IsFalse(char.IsUpper('5'));
            Assert.IsFalse(char.IsUpper('a'));
            Assert.IsFalse(char.IsUpper('z'));
        }

        [Test]
        public void TryParse()
        {
            char res;
            var b = char.TryParse("t", out res);
            Assert.IsTrue(b);
            Assert.AreEqual('t', res);

            b = char.TryParse(null, out res);
            Assert.IsFalse(b);
            Assert.AreEqual('\0', res);

            b = char.TryParse("null", out res);
            Assert.IsFalse(b);
            Assert.AreEqual('\0', res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual('s', char.Parse("s"));
            Assert.AreEqual('T', char.Parse("T"));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseStringAction()
        {
            char.Parse("long_string");
        }

        void ParseEmptyStringAction()
        {
            char.Parse("");
        }

        void ParseNullAction()
        {
            char.Parse(null);
        }
    }
}
