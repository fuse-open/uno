using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ArraySortTest
    {
        [Test]
        public void IntAscendingSort()
        {
            var unsortedData = new [] { 13, 13, 13, 122, 11, 13, -13, -12, -11, 1241712474, 1337, -1337 };
            var expectedSortedData = new [] { -1337, -13, -12, -11, 11, 13, 13, 13, 13, 122, 1337, 1241712474 };

            Array.Sort(unsortedData, IntAscendingCompare);
            ArrayHelper.AssertArrayEqual(unsortedData, expectedSortedData);
        }

        [Test]
        public void IntDescendingSort()
        {
            var unsortedData = new [] { 13, 13, 13, 122, 11, 13, -13, -12, -11, 1241712474, 1337, -1337 };
            var expectedSortedData = new [] { 1241712474, 1337, 122, 13, 13, 13, 13, 11, -11, -12, -13, -1337 };

            Array.Sort(unsortedData, IntDescendingCompare);
            ArrayHelper.AssertArrayEqual(unsortedData, expectedSortedData);
        }

        [Test]
        public void NullArray()
        {
            Assert.Throws(NullArrayInternal);
        }

        void NullArrayInternal()
        {
            Array.Sort<int>(null, 0, 10, IntDescendingCompare);
        }

        [Test]
        public void InvalidIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InvalidIndexInternal);
        }

        void InvalidIndexInternal()
        {
            var test = new int[2];
            Array.Sort<int>(test, -5, 10, IntDescendingCompare);
        }

        [Test]
        public void InvalidLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(InvalidLengthInternal);
        }

        void InvalidLengthInternal()
        {
            var test = new int[2];
            Array.Sort<int>(test, 0, -5, IntDescendingCompare);
        }

        [Test]
        public void OutOfRange()
        {
            Assert.Throws<ArgumentException>(OutOfRangeInternal);
        }

        void OutOfRangeInternal()
        {
            var test = new int[2];
            Array.Sort<int>(test, 0, 5, IntDescendingCompare);
        }

        [Test]
        public void ComparisonNull()
        {
            Assert.Throws(ComparisonNullInternal);
        }

        void ComparisonNullInternal()
        {
            var test = new int[2];
            Array.Sort<int>(test, null);
        }

        static int IntAscendingCompare(int a, int b)
        {
            return a - b;
        }

        static int IntDescendingCompare(int a, int b)
        {
            return b - a;
        }
    }
}