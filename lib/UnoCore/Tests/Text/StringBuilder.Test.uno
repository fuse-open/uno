using Uno;
using Uno.Testing;
using Uno.Text;

namespace Uno.Text.Test
{
    public class StringBuilderTest
    {
        [Test]
        public void EmptyBuilderReturnsEmptyString()
        {
            var sb = new StringBuilder();
            Assert.AreEqual( string.Empty, sb.ToString());
            Assert.AreEqual( 0 , sb.Length);
        }

        [Test]
        public void AppendingOneCharArrayReturnsThat()
        {
            var sb = new StringBuilder();
            sb.Append(new char[] {'a','b'});
            Assert.AreEqual( "ab", sb.ToString());
            Assert.AreEqual( 2 , sb.Length);
        }

        [Test]
        public void CharArrayCopies()
        {
            var sb = new StringBuilder();
            var data = new char[] {'a', 'b'};
            sb.Append(data);
            data[0] = 'c';
            data[1] = 'd';
            sb.Append(data);
            Assert.AreEqual("abcd", sb.ToString());
            Assert.AreEqual(4, sb.Length);
        }


        [Test]
        public void AppendingSeveralCharArraysReturnsSum()
        {
            var sb = new StringBuilder();
            sb.Append(new char[] {'a','b'});
            sb.Append(new char[] {'c','d'});
            sb.Append(new char[] {'e','f'});
            Assert.AreEqual( "abcdef", sb.ToString());
            Assert.AreEqual( 6 , sb.Length);
        }

        [Test]
        public void AppendingSeveralStringsReturnsSum()
        {
            var sb = new StringBuilder();
            sb.Append("ab");
            sb.Append("c");
            sb.Append("def");
            Assert.AreEqual( "abcdef", sb.ToString());
            Assert.AreEqual( 6 , sb.Length);
        }

        [Test]
        public void MaxCapacityIsIntMax()
        {
            Assert.AreEqual( int.MaxValue , new StringBuilder().MaxCapacity);
        }
    }
}
