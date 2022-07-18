using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;

namespace Uno.Test
{
    public class StringTest
    {
        [Test]
        public void FromCharArray()
        {
            Assert.AreEqual(0, new String(new char[] {}).Length);
            Assert.AreEqual(0, new String((char[])null).Length);
            Assert.AreEqual(3, new String(new char[] { 'f', 'o', 'o' }).Length);

            Assert.Throws<InvalidCastException>(() => new String((char[])(object)new int[]{ 1 }));
        }

        [Test]
        public void FromCharArrayRange()
        {
            Assert.AreEqual("abc", new String(new char[] { 'a', 'b', 'c' }, 0, 3));
            Assert.AreEqual("ab", new String(new char[] { 'a', 'b', 'c' }, 0, 2));
            Assert.AreEqual("bc", new String(new char[] { 'a', 'b', 'c' }, 1, 2));
            Assert.AreEqual("b", new String(new char[] { 'a', 'b', 'c' }, 1, 1));

            Assert.Throws<ArgumentNullException>(() => new String((char[])null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new String(new char[] { 'f', 'o', 'o' }, -1, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new String(new char[] { 'f', 'o', 'o' }, 4, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new String(new char[] { 'f', 'o', 'o' }, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new String(new char[] { 'f', 'o', 'o' }, 0, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new String(new char[] { 'f', 'o', 'o' }, 2, 2));
        }


        [Test]
        public void TestEmpty()
        {
            Assert.AreEqual( String.Empty , "");
        }

        [Test]
        public void NullStringIsNullOrEmpty()
        {
            string s = null;
            Assert.IsTrue( String.IsNullOrEmpty(s));
        }

        [Test]
        public void EmptyStringIsNullOrEmpty()
        {
            string s = "";
            Assert.IsTrue( String.IsNullOrEmpty(s));
        }

        [Test]
        public void NonEmptyStringIsNotNullOrEmpty()
        {
            string s = " ";
            Assert.IsFalse(String.IsNullOrEmpty(s));
        }

        [Test]
        public void IsNullOrWhiteSpace()
        {
            Assert.IsTrue(String.IsNullOrWhiteSpace(null));
            Assert.IsTrue(String.IsNullOrWhiteSpace(""));
            Assert.IsTrue(String.IsNullOrWhiteSpace(" "));
            Assert.IsTrue(String.IsNullOrWhiteSpace("\t"));
            Assert.IsTrue(String.IsNullOrWhiteSpace(" \r "));
            Assert.IsTrue(String.IsNullOrWhiteSpace("  \n"));
            Assert.IsFalse(String.IsNullOrWhiteSpace(" \\ "));
            Assert.IsFalse(String.IsNullOrWhiteSpace(" s "));
            Assert.IsFalse(String.IsNullOrWhiteSpace("123 455"));
        }

        [Test]
        public void EmptyStringTrimsToEmpty()
        {
            Assert.AreEqual( String.Empty , String.Empty.Trim());
        }

        [Test]
        public void NoWhiteSpaceTrimsToIdenticalString()
        {
            Assert.AreEqual( "foo" , "foo".Trim());
        }

        [Test]
        public void LeadingWhiteSpaceIsRemoved()
        {
            Assert.AreEqual( "foo" , " \t\r\nfoo".Trim());
        }

        [Test]
        public void LeadingWhiteSpaceIsRemoved2()
        {
            Assert.AreEqual( "private" , " private".Trim(' '));
        }

        [Test]
        public void LeadingWhiteSpaceIsRemoved3()
        {
            Assert.AreEqual( "private" , " private".Trim());
        }

        [Test]
        public void LeadingWhiteSpaceIsRemoved4()
        {
            Assert.AreEqual( "foo" , " foo".Trim());
        }

        [Test]
        public void LeadingWhiteSpaceIsRemovedWithSpecialChars()
        {
            Assert.AreEqual( "fo/o=0" , " \t\r\nfo/o=0".Trim());
        }

        [Test]
        public void TrailingWhiteSpaceIsRemoved()
        {
            Assert.AreEqual( "fo/o=0" , "fo/o=0 \t\r\n".Trim());
        }

        [Test]
        public void OnlyLeadingAndTrailingWhiteSpaceIsRemoved()
        {
            Assert.AreEqual( "f o\to" , " \t\r\nf o\to \n".Trim());
        }

        [Test]
        public void TrimWithChars()
        {
            Assert.AreEqual("39712893", "123971289312".Trim('1', '2', '9'));
        }

        [Test]
        public void TrimStart()
        {
            Assert.AreEqual("123    ", "   123    ".TrimStart());
            Assert.AreEqual("3971289312", "123971289312".TrimStart('1', '2', '9'));
        }

        [Test]
        public void TrimEnd()
        {
            Assert.AreEqual("   123", "   123    ".TrimEnd());
            Assert.AreEqual("1239712893", "123971289312".TrimEnd('1', '2', '9'));
        }

        [Test]
        public void InsertingAtTheEndAppends()
        {
            var s = "foo";
            Assert.AreEqual( "foobar" , s.Insert(3, "bar"));
        }

        [Test]
        public void InsertingAtTheBeginningPrepends()
        {
            var s = "foo";
            Assert.AreEqual( "barfoo" , s.Insert(0, "bar"));
        }

        [Test]
        public void InsertingInTheMiddleInserts()
        {
            var s = "foo";
            Assert.AreEqual( "fobaro" , s.Insert(2, "bar"));
        }

        [Test]
        public void InsertingNullThrows()
        {
            Assert.Throws<ArgumentNullException>(InsertNull);
        }

        private void InsertNull()
        {
            var s = "string";
            s.Insert(2, null);
        }

        [Test]
        public void InsertingBeforeStartThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertBeforeStart);
        }

        private void InsertBeforeStart()
        {
            var s = "string";
            s.Insert(-1, "foo");
        }

        [Test]
        public void InsertingAfterEndThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InsertAfterEnd);
        }

        private void InsertAfterEnd()
        {
            var s = "string";
            s.Insert(7, "foo");
        }

        [Test]
        public void IndexOfNotFoundCharIsMinusOne()
        {
            Assert.AreEqual( -1 , "foo".IndexOf('b'));
            Assert.AreEqual( -1 , "".IndexOf('b'));
        }

        [Test]
        public void IndexOfFoundCharIsCorrect()
        {
            Assert.AreEqual( 1 , "foo".IndexOf('o'));
        }

        [Test]
        public void IndexOfCharIsCorrectUsingIndex()
        {
            Assert.AreEqual( 0 , "abcd".IndexOf('a', 0));
            Assert.AreEqual( 2 , "abab".IndexOf('a', 1));
            Assert.AreEqual( 2 , "abab".IndexOf('a', 2));
            Assert.AreEqual( -1 , "abab".IndexOf('a', 3));
        }

        [Test]
        public void IndexOfCharIsCorrectUsingCount()
        {
            Assert.AreEqual(-1, "abcd".IndexOf('a', 1, 2));
            Assert.AreEqual( 1, "abcd".IndexOf('b', 1, 2));
            Assert.AreEqual( 2, "abcd".IndexOf('c', 1, 2));
            Assert.AreEqual(-1, "abcd".IndexOf('d', 1, 2));
        }

        [Test]
        public void IndexOfCharWithStartIndexEqualToLengthReturnsMinusOne()
        {
            Assert.AreEqual( -1 , "abc".IndexOf('a', 3));
        }

        [Test]
        public void IndexOfCharWithStartIndexOutOfRangeThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithTooLargeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithNegativeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithEmptyStringAndNegativeStartIndex);
        }

        private void IndexOfCharWithTooLargeStartIndex()
        {
            "abc".IndexOf('a', 4);
        }

        private void IndexOfCharWithNegativeStartIndex()
        {
            "abc".IndexOf('a', -1);
        }

        private void IndexOfCharWithEmptyStringAndNegativeStartIndex()
        {
            "".IndexOf('a', -1);
        }

        [Test]
        public void IndexOfCharWithCountOutOfRangeThrows()
        {
            Assert.AreEqual(-1, "abc".IndexOf('a', 2, 1));
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithTooLargeCount);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithNegativeCount);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfCharWithEmptyStringAndNegativeCount);
        }

        private void IndexOfCharWithNegativeCount()
        {
            "abc".IndexOf('a', 2, -1);
        }

        private void IndexOfCharWithEmptyStringAndNegativeCount()
        {
            "".IndexOf('a', 2, -1);
        }

        private void IndexOfCharWithTooLargeCount()
        {
            "abc".IndexOf('a', 2, 2);
        }

        [Test]
        public void IndexOfEmptyStringIs0()
        {
            Assert.AreEqual( 0 , "foo".IndexOf(String.Empty));
            Assert.AreEqual( 0 , "foo".IndexOf(""));
        }

        [Test]
        public void IndexOfNotFoundStringIsMinusOne()
        {
            Assert.AreEqual( -1 , "foo".IndexOf("bar"));
            Assert.AreEqual( -1 , "abc".IndexOf("abcd"));
            Assert.AreEqual( -1 , "abc".IndexOf("bcd"));
            Assert.AreEqual( -1 , "bc".IndexOf("abc"));
        }

        [Test]
        public void IndexOfFoundStringIsCorrect()
        {
            Assert.AreEqual( 0 , "abcd".IndexOf("a"));
            Assert.AreEqual( 2 , "abcd".IndexOf("c"));
            Assert.AreEqual( 3 , "abcd".IndexOf("d"));
            Assert.AreEqual( 3 , "abcbcd".IndexOf("bcd"));
        }

        [Test]
        public void IndexOfStringIsCorrectUsingIndex()
        {
            Assert.AreEqual( 0 , "abcd".IndexOf("a", 0));
            Assert.AreEqual( 2 , "abab".IndexOf("ab", 1));
            Assert.AreEqual( 2 , "abab".IndexOf("ab", 2));
            Assert.AreEqual( -1 , "abab".IndexOf("ab", 3));
        }

        [Test]
        public void IndexOfWithStartIndexEqualToLengthReturnsMinusOne()
        {
            Assert.AreEqual( -1 , "abc".IndexOf("a", 3));
        }

        [Test]
        public void IndexOfNullThrows()
        {
            Assert.Throws<ArgumentNullException>(IndexOfNull);
        }

        private void IndexOfNull()
        {
            "".IndexOf(null);
        }

        [Test]
        public void IndexOfWithTooLargeStartIndexThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfWithTooLargeStartIndex);
        }

        private void IndexOfWithTooLargeStartIndex()
        {
            "abc".IndexOf("a", 4);
        }

        [Test]
        public void IndexOfAny()
        {
            Assert.AreEqual( 0, "abcd".IndexOfAny(new char[] {'a'}));
            Assert.AreEqual( 1, "abcd".IndexOfAny(new char[] {'b'}));
            Assert.AreEqual( 2, "abcd".IndexOfAny(new char[] {'c'}));
            Assert.AreEqual( 3, "abcd".IndexOfAny(new char[] {'d'}));
            Assert.AreEqual(-1, "abcd".IndexOfAny(new char[] {'e'}));

            Assert.AreEqual(0, "abcd".IndexOfAny(new char[] {'a', 'x'}));
            Assert.AreEqual(1, "abcd".IndexOfAny(new char[] {'x', 'b'}));

            // should find the *first* char if there's more than one
            Assert.AreEqual(1, "abbb".IndexOfAny(new char[] {'b'}));

            // startIndex
            Assert.AreEqual( 0, "abcd".IndexOfAny(new char[] {'a'}, 0));
            Assert.AreEqual(-1, "abcd".IndexOfAny(new char[] {'a'}, 1));
            Assert.AreEqual( 1, "abcd".IndexOfAny(new char[] {'b'}, 1));

            // startIndex + count
            Assert.AreEqual(-1, "abcd".IndexOfAny(new char[] {'a'}, 1, 2));
            Assert.AreEqual( 1, "abcd".IndexOfAny(new char[] {'b'}, 1, 2));
            Assert.AreEqual( 2, "abcd".IndexOfAny(new char[] {'c'}, 1, 2));
            Assert.AreEqual(-1, "abcd".IndexOfAny(new char[] {'d'}, 1, 2));
            Assert.AreEqual( 0, "abcd".IndexOfAny(new char[] {'a'}, 0, 2));
            Assert.AreEqual( 3, "abcd".IndexOfAny(new char[] {'d'}, 2, 2));

            // errors
            Assert.Throws<ArgumentNullException>(IndexOfAnyNull);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfAnyNegativeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfAnyTooLargeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfAnyNegativeCount);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfAnyTooLargeCount);
        }

        private void IndexOfAnyNull()
        {
            "abc".IndexOfAny(null);
        }

        void IndexOfAnyNegativeStartIndex()
        {
            "abc".IndexOfAny(new char[] {'x'}, -1);
        }

        void IndexOfAnyTooLargeStartIndex()
        {
            "abc".IndexOfAny(new char[] {'x'}, 4);
        }

        void IndexOfAnyNegativeCount()
        {
            "abc".IndexOfAny(new char[] {'x'}, 0, -1);
        }

        void IndexOfAnyTooLargeCount()
        {
            "abc".IndexOfAny(new char[] {'x'}, 1, 3);
        }

        [Test]
        public void StartsWith()
        {
            Assert.IsTrue("foo".StartsWith(""));
            Assert.IsTrue("foo".StartsWith("f"));
            Assert.IsTrue("foo".StartsWith("fo"));
            Assert.IsTrue("1241j 241 31".StartsWith("1241j 24"));
            Assert.Throws<ArgumentNullException>(StartsWithNull);
            Assert.IsFalse("foo".StartsWith("1"));
            Assert.IsFalse("foo".StartsWith(" fo2"));
            Assert.IsFalse("1241j 241 31".StartsWith("123"));
        }

        private void StartsWithNull()
        {
            Assert.IsFalse("1241j 241 31".StartsWith(null));
        }

        [Test]
        public void EndsWith()
        {
            Assert.IsTrue("foo".EndsWith(""));
            Assert.IsTrue("foo ".EndsWith(" "));
            Assert.IsTrue("foo ".EndsWith("o "));
            Assert.IsTrue("1241j 241 31".EndsWith("j 241 31"));
            Assert.Throws<ArgumentNullException>(EndsWithNull);
            Assert.IsFalse("foo".EndsWith("1"));
            Assert.IsFalse("foo ".EndsWith("  "));
            Assert.IsFalse("foo ".EndsWith("foofoo "));
            Assert.IsFalse("1241j 241 31".EndsWith("123"));
        }

        private void EndsWithNull()
        {
            Assert.IsFalse("1241j 241 31".EndsWith(null));
        }

        [Test]
        public void PadLeft()
        {
            Assert.AreEqual("  ", "".PadLeft(2));
            Assert.AreEqual(" 123", "123".PadLeft(4));
            Assert.AreEqual("123", "123".PadLeft(2));
        }

        [Test]
        public void PadLeftWithChar()
        {
            Assert.AreEqual("___", "".PadLeft(3, '_'));
            Assert.AreEqual("mmm12", "12".PadLeft(5, 'm'));
            Assert.AreEqual("123", "123".PadLeft(2, 'f'));
        }

        [Test]
        public void PadRight()
        {
            Assert.AreEqual("  ", "".PadRight(2));
            Assert.AreEqual("123 ", "123".PadRight(4));
            Assert.AreEqual("123", "123".PadRight(2));
        }

        [Test]
        public void PadRightWithChar()
        {
            Assert.AreEqual("___", "".PadRight(3, '_'));
            Assert.AreEqual("12mmm", "12".PadRight(5, 'm'));
            Assert.AreEqual("123", "123".PadRight(2, 'f'));
        }

        [Test]
        public void ReplacingWithSameStringReturnsSame()
        {
            Assert.AreEqual( "foo" , "foo".Replace("oo", "oo"));
        }

        [Test]
        public void ReplacingNotFoundStringReturnsSame()
        {
            Assert.AreEqual( "foo" , "foo".Replace("xyz", "abc"));
        }

        [Test]
        public void ReplacingNullThrows()
        {
            Assert.Throws<ArgumentNullException>(ReplaceNull);
        }

        private void ReplaceNull()
        {
            "foo".Replace(null, "bar");
        }

        [Test]
        public void ReplacingEmptyThrows()
        {
            Assert.Throws<ArgumentException>(ReplaceEmpty);
        }

        private void ReplaceEmpty()
        {
            "foo".Replace("", "bar");
        }

        [Test]
        public void ReplacingStringAtBeginningReplaces()
        {
            Assert.AreEqual( "babar" , "foobar".Replace("foo", "ba"));
        }

        [Test]
        public void ReplacingStringAtEndReplaces()
        {
            Assert.AreEqual( "foobarr!" , "foobar".Replace("ar", "arr!"));
        }

        [Test]
        public void ReplacingStringInTheMiddleReplaces()
        {
            Assert.AreEqual( "feebar" , "foobar".Replace("oo", "ee"));
        }

        [Test]
        public void ReplacingEntireStringReplaces()
        {
            Assert.AreEqual( "bar" , "foo".Replace("foo", "bar"));
        }

        [Test]
        public void ReplacingManyStringsReplaces()
        {
            Assert.AreEqual( "babarba" , "foobarfoo".Replace("foo", "ba"));
            Assert.AreEqual( "barbabarbabar" , "barfoobarfoobar".Replace("foo", "ba"));
        }

        [Test]
        public void ToUpper()
        {
            Assert.AreEqual( "THE-LAZY.FØX" , "The-lazy.føx".ToUpper());
        }

        [Test]
        public void ToLower()
        {
            Assert.AreEqual( "the-lazy.føx" , "THE-LAZY.FØx".ToLower());
        }

        [Test]
        public void EscapeSequences()
        {
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\'', 'b', 'a', 'r' }, "foo\'bar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\"', 'b', 'a', 'r' }, "foo\"bar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\\', 'b', 'a', 'r' }, "foo\\bar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\0', 'b', 'a', 'r' }, "foo\0bar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\a', 'b', 'a', 'r' }, "foo\abar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\b', 'b', 'a', 'r' }, "foo\bbar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\f', 'b', 'a', 'r' }, "foo\fbar".ToCharArray());
            Assert.AreEqual( new char[]{ 'f', 'o', 'o', '\n', 'b', 'a', 'r' }, "foo\nbar".ToCharArray());
        }

        [Test]
        public void UnicodeStringLiterals()
        {
            Assert.AreEqual( new char[]{ '\u000F', 'F' } , "\u000FF".ToCharArray());
        }


        [Test]
        public void Substring()
        {
            Assert.AreEqual( "somew= or/d0" , "Asomew= or/d0".Substring(1, 12));
        }

        [Test]
        public void Substring2()
        {
            Assert.AreEqual( "0" , "Asomew= or/d0".Substring(12, 1));
        }

        [Test]
        public void Substring3()
        {
            Assert.AreEqual( "= or/d0" , "Asomew= or/d0".Substring(6, 7));
        }

        [Test]
        public void Substring4()
        {
            var myString = "abc";
            Assert.AreEqual(myString.Substring(2, 1), "c");
            Assert.IsTrue(String.IsNullOrEmpty(myString.Substring(3, 0))); // This is true.
            Assert.AreEqual(myString.Substring(3, 0), string.Empty);
        }

        [Test]
        public void SubstringThrowsExceptionOnOutOfRangeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(TriggerSubstringException);
        }

        void TriggerSubstringException()
        {
            "abc".Substring(3, 1);
        }

        [Test]
        public void Length()
        {
            Assert.AreEqual(13, "AsØmew= or/d0".Length);
        }

        [Test]
        public void Contains()
        {
            var str = "The quick brown fox jumps over the lazy dog";
            Assert.IsTrue(str.Contains("fox"));
            Assert.IsTrue(str.Contains("The quick"));
            Assert.IsTrue(str.Contains("dog"));
            Assert.IsFalse(str.Contains("non"));
        }

        [Test]
        public void LastIndexOfNotFoundCharIsMinusOne()
        {
            Assert.AreEqual( -1 , "foo".LastIndexOf('b'));
            Assert.AreEqual( -1 , "".LastIndexOf('b'));
        }

        [Test]
        public void LastIndexOfFoundCharIsCorrect()
        {
            Assert.AreEqual( 2 , "foo".LastIndexOf('o'));
        }

        [Test]
        public void LastIndexOfCharIsCorrectUsingIndex()
        {
            Assert.AreEqual( 0 , "abcd".LastIndexOf('a', 0));
            Assert.AreEqual( 0 , "abab".LastIndexOf('a', 1));
            Assert.AreEqual( 2 , "abab".LastIndexOf('a', 2));
            Assert.AreEqual( 2 , "abab".LastIndexOf('a', 3));
        }

        [Test]
        public void LastIndexOfCharIsCorrectUsingCount()
        {
            Assert.AreEqual(-1, "abcd".LastIndexOf('a', 2, 2));
            Assert.AreEqual( 1, "abcd".LastIndexOf('b', 2, 2));
            Assert.AreEqual( 2, "abcd".LastIndexOf('c', 2, 2));
            Assert.AreEqual(-1, "abcd".LastIndexOf('d', 2, 2));
        }

        [Test]
        public void LastIndexOfCharWithStartIndexOutOfRangeThrows()
        {
            Assert.AreEqual(-1, string.Empty.LastIndexOf('a', -1));
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfCharWithStartIndexEqualToLength);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfCharWithNegativeStartIndex);
        }

        private void LastIndexOfCharWithStartIndexEqualToLength()
        {
            "abc".LastIndexOf('a', 3);
        }

        private void LastIndexOfCharWithNegativeStartIndex()
        {
            "abc".LastIndexOf('a', -1);
        }

        [Test]
        public void LastIndexOfCharWithTooLargeStartIndexThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfCharWithTooLargeStartIndex);
        }

        private void LastIndexOfCharWithTooLargeStartIndex()
        {
            "abc".LastIndexOf('a', 4);
        }

        [Test]
        public void LastIndexOfCharWithTooLargeCountThrows()
        {
            Assert.AreEqual(-1, "abc".LastIndexOf('c', 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfCharWithTooLargeCount);
        }

        private void LastIndexOfCharWithTooLargeCount()
        {
            "abc".LastIndexOf('c', 0, 2);
        }


        [Test]
        public void LastIndexOfAny()
        {
            Assert.AreEqual( 0, "abcd".LastIndexOfAny(new char[] {'a'}));
            Assert.AreEqual( 1, "abcd".LastIndexOfAny(new char[] {'b'}));
            Assert.AreEqual( 2, "abcd".LastIndexOfAny(new char[] {'c'}));
            Assert.AreEqual( 3, "abcd".LastIndexOfAny(new char[] {'d'}));
            Assert.AreEqual(-1, "abcd".LastIndexOfAny(new char[] {'e'}));

            Assert.AreEqual(0, "abcd".LastIndexOfAny(new char[] {'a', 'x'}));
            Assert.AreEqual(1, "abcd".LastIndexOfAny(new char[] {'x', 'b'}));

            // should find the *last* char if there's more than one
            Assert.AreEqual(2, "bbba".LastIndexOfAny(new char[] {'b'}));

            // startIndex
            Assert.AreEqual( 3, "abcd".LastIndexOfAny(new char[] {'d'}, 3));
            Assert.AreEqual(-1, "abcd".LastIndexOfAny(new char[] {'d'}, 2));
            Assert.AreEqual( 2, "abcd".LastIndexOfAny(new char[] {'c'}, 2));

            // startIndex + count
            Assert.AreEqual(-1, "abcd".LastIndexOfAny(new char[] {'a'}, 2, 2));
            Assert.AreEqual( 1, "abcd".LastIndexOfAny(new char[] {'b'}, 2, 2));
            Assert.AreEqual( 2, "abcd".LastIndexOfAny(new char[] {'c'}, 2, 2));
            Assert.AreEqual(-1, "abcd".LastIndexOfAny(new char[] {'d'}, 2, 2));
            Assert.AreEqual( 0, "abcd".LastIndexOfAny(new char[] {'a'}, 1, 2));
            Assert.AreEqual( 3, "abcd".LastIndexOfAny(new char[] {'d'}, 3, 2));

            // errors
            Assert.Throws<ArgumentNullException>(LastIndexOfAnyNull);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfAnyNegativeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfAnyTooLargeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfAnyNegativeCount);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfAnyTooLargeCount);
        }

        private void LastIndexOfAnyNull()
        {
            "abc".LastIndexOfAny(null);
        }

        void LastIndexOfAnyNegativeStartIndex()
        {
            "abc".LastIndexOfAny(new char[] {'x'}, -1);
        }

        void LastIndexOfAnyTooLargeStartIndex()
        {
            "abc".LastIndexOfAny(new char[] {'x'}, 4);
        }

        void LastIndexOfAnyNegativeCount()
        {
            "abc".LastIndexOfAny(new char[] {'x'}, 0, -1);
        }

        void LastIndexOfAnyTooLargeCount()
        {
            "abc".IndexOfAny(new char[] {'x'}, 1, 3);
        }

        [Test]
        public void Join()
        {
            Assert.AreEqual(string.Join(" ", new string[0]), "");
            Assert.AreEqual(string.Join(" ", new[] {"a"}), "a");
            Assert.AreEqual(string.Join(" ", new[] {"a", "b", "c"}), "a b c");
            Assert.AreEqual(string.Join(" ", new[] {"a", null, "c"}), "a  c");
            Assert.AreEqual(string.Join("ab", new[] {"a", "b", "c"}), "aabbabc");
            Assert.Throws<ArgumentNullException>(() => string.Join("", (object[])null));
            Assert.Throws<ArgumentNullException>(() => string.Join("", (string[])null));
        }
    }
}
