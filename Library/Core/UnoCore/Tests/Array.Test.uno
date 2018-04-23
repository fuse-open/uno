using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ArrayTest
    {
        [Test]
        public void IndexOf()
        {
            var valueArray = new int[] {10, 11, 12, 13};
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(i, Array.IndexOf(valueArray, valueArray[i]));
            Assert.AreEqual(-1, Array.IndexOf(valueArray, -1));

            var objectArray = new object[]
            {
                new object(), new object(), new object(), new object()
            };
            for (int i = 0; i < objectArray.Length; ++i)
                Assert.AreEqual(i, Array.IndexOf(objectArray, objectArray[i]));
            Assert.AreEqual(-1, Array.IndexOf(objectArray, new object()));

            // should find the *first* element if there's more than one
            Assert.AreEqual(0, Array.IndexOf(new int[] { 10, 10 }, 10));

            // startIndex
            Assert.AreEqual( 0, Array.IndexOf(valueArray, 10, 0));
            Assert.AreEqual(-1, Array.IndexOf(valueArray, 10, 1));
            Assert.AreEqual( 1, Array.IndexOf(valueArray, 11, 1));

            // startIndex + count
            Assert.AreEqual(-1, Array.IndexOf(valueArray, 10, 1, 2));
            Assert.AreEqual( 1, Array.IndexOf(valueArray, 11, 1, 2));
            Assert.AreEqual( 2, Array.IndexOf(valueArray, 12, 1, 2));
            Assert.AreEqual(-1, Array.IndexOf(valueArray, 13, 1, 2));
            Assert.AreEqual( 0, Array.IndexOf(valueArray, 10, 0, 2));
            Assert.AreEqual( 3, Array.IndexOf(valueArray, 13, 2, 2));

            // errors
            Assert.Throws<ArgumentNullException>(IndexOfNullArray);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfNegativeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfTooLargeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfNegativeCount);
            Assert.Throws<ArgumentOutOfRangeException>(IndexOfTooLargeCount);
        }

        void IndexOfNullArray()
        {
            int[] nullArray = null;
            Array.IndexOf(nullArray, 5);
        }

        void IndexOfNegativeStartIndex()
        {
            Array.IndexOf(new int[] {1, 2, 3}, 5, -1);
        }

        void IndexOfTooLargeStartIndex()
        {
            Array.IndexOf(new int[] {1, 2, 3}, 5, 4);
        }

        void IndexOfNegativeCount()
        {
            Array.IndexOf(new int[] {1, 2, 3}, 5, 0, -1);
        }

        void IndexOfTooLargeCount()
        {
            Array.IndexOf(new int[] {1, 2, 3}, 5, 1, 3);
        }

        [Test]
        public void LastIndexOf()
        {
            var valueArray = new int[] {10, 11, 12, 13};
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(i, Array.LastIndexOf(valueArray, valueArray[i]));
            Assert.AreEqual(-1, Array.LastIndexOf(valueArray, -1));

            var objectArray = new object[]
            {
                new object(), new object(), new object(), new object()
            };
            for (int i = 0; i < objectArray.Length; ++i)
                Assert.AreEqual(i, Array.LastIndexOf(objectArray, objectArray[i]));
            Assert.AreEqual(-1, Array.LastIndexOf(objectArray, new object()));

            // should find the *last* element if there's more than one
            Assert.AreEqual(1, Array.LastIndexOf(new int[] { 10, 10 }, 10));

            // startIndex
            Assert.AreEqual( 3, Array.LastIndexOf(valueArray, 13, 3));
            Assert.AreEqual(-1, Array.LastIndexOf(valueArray, 13, 2));
            Assert.AreEqual( 2, Array.LastIndexOf(valueArray, 12, 2));

            // startIndex + count
            Assert.AreEqual(-1, Array.LastIndexOf(valueArray, 10, 2, 2));
            Assert.AreEqual( 1, Array.LastIndexOf(valueArray, 11, 2, 2));
            Assert.AreEqual( 2, Array.LastIndexOf(valueArray, 12, 2, 2));
            Assert.AreEqual(-1, Array.LastIndexOf(valueArray, 13, 2, 2));
            Assert.AreEqual( 0, Array.LastIndexOf(valueArray, 10, 1, 2));
            Assert.AreEqual( 3, Array.LastIndexOf(valueArray, 13, 3, 2));

            // errors
            Assert.Throws<ArgumentNullException>(LastIndexOfNullArray);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfNegativeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfTooLargeStartIndex);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfNegativeCount);
            Assert.Throws<ArgumentOutOfRangeException>(LastIndexOfTooLargeCount);
        }

        void LastIndexOfNullArray()
        {
            int[] nullArray = null;
            Array.LastIndexOf(nullArray, 5);
        }

        void LastIndexOfNegativeStartIndex()
        {
            Array.LastIndexOf(new int[] {1, 2, 3}, 5, -1);
        }

        void LastIndexOfTooLargeStartIndex()
        {
            Array.LastIndexOf(new int[] {1, 2, 3}, 5, 4);
        }

        void LastIndexOfNegativeCount()
        {
            Array.LastIndexOf(new int[] {1, 2, 3}, 5, 0, -1);
        }

        void LastIndexOfTooLargeCount()
        {
            Array.LastIndexOf(new int[] {1, 2, 3}, 5, 1, 3);
        }


    }
}
