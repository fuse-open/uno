using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ArrayCopyTest
    {
        [Test]
        public void CopyArray()
        {
            var arrayA = new [] { 13, 13, 13, 122, 11, 13, -13, -12, -11, 1241712474, 1337, -1337 };
            var arrayB = new int[arrayA.Length];
            Array.Copy(arrayA, arrayB, arrayA.Length);
            ArrayHelper.AssertArrayEqual(arrayA, arrayB);
        }

        [Test]
        public void CopySectionOfArray()
        {
            var arrayA = new [] { 13, 13, 13, 122, 11, 13, -13, -12, -11, 1241712474, 1337, -1337 };
            var arrayB = new int[arrayA.Length];
            Array.Copy(arrayA, arrayB, 5);
            ArrayHelper.AssertArrayEqual(arrayA, arrayB, 5);
        }

        [Test]
        public void CopyToLessSizedArray()
        {
            Assert.Throws<ArgumentException>(CopyToLessSizedArrayInternal);
        }

        void CopyToLessSizedArrayInternal()
        {
            var arrayA = new [] { 13, 13, 13, 122, 11, 13, -13, -12, -11, 1241712474, 1337, -1337 };
            var arrayB = new int[2];
            Array.Copy(arrayA, arrayB, arrayA.Length);
        }

        [Test]
        public void CopyFromLessSizedArray()
        {
            Assert.Throws<ArgumentException>(CopyFromLessSizedArrayInternal);
        }

        void CopyFromLessSizedArrayInternal()
        {
            var arrayA = new [] { 13, 13 };
            var arrayB = new int[20];
            Array.Copy(arrayA, arrayB, 20);
        }

        [Test]
        public void CopyWithInvalidOffset()
        {
            Assert.Throws<ArgumentOutOfRangeException>(CopyWithInvalidOffsetInternal);
        }

        void CopyWithInvalidOffsetInternal()
        {
            var arrayA = new [] { 13, 13 };
            var arrayB = new int[20];
            Array.Copy(arrayA, arrayB, -1);
        }

        [Test]
        public void CopyFromNullSourceArray()
        {
            Assert.Throws<ArgumentNullException>(CopyFromNullSourceArrayInternal);
        }

        void CopyFromNullSourceArrayInternal()
        {
            var arrayB = new int[10];
            Array.Copy(null, arrayB, 5);
        }

        [Test]
        public void CopyToNullDestinationArray()
        {
            Assert.Throws<ArgumentNullException>(CopyToNullDestinationArrayInternal);
        }

        void CopyToNullDestinationArrayInternal()
        {
            var arrayA = new int[5];
            Array.Copy(arrayA, null, 5);
        }
    }
}