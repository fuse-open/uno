using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;

namespace UnoCore_Uno_IO
{
    public class MemoryStreamTests
    {
        [Test]
        public void Init()
        {
            var stream = new MemoryStream();
            Assert.AreEqual(0, stream.Length);
            Assert.AreEqual(0, stream.Position);
            Assert.IsTrue(stream.CanRead);
            Assert.IsTrue(stream.CanWrite);
            Assert.IsTrue(stream.CanSeek);
            Assert.IsFalse(stream.CanTimeout);
            stream.Position = 10; //Should be allowed
        }

        [Test]
        public void SimpleReadWrite()
        {
            var data = new byte[]{1, 4, 9};
            var stream = new MemoryStream();
            stream.Write(data, 0, 3);
            Assert.AreEqual(3, stream.Position);
            Assert.AreEqual(3, stream.Length);
            AssertContentsAre(stream, data);
        }

        [Test]
        public void WritePartsOfArray()
        {
            var data = new byte[]{1, 2, 3, 4};
            var stream = new MemoryStream();
            stream.Write(data, 1, 2);
            Assert.AreEqual(2, stream.Position);
            Assert.AreEqual(2, stream.Length);
            AssertContentsAre(stream, new byte[]{2,3});
        }

        [Test]
        public void ReadToPartsOfArray()
        {
            var stream = new MemoryStream(new byte[]{1, 2, 3, 4});

            var readBack = new byte[]{0, 0, 0, 0};
            Assert.AreEqual(2, stream.Read(readBack, 2, 2));
            Assert.AreEqual(new byte[]{0, 0, 1, 2}, readBack);
        }

        [Test]
        public void ReadMoreThanIsInStream()
        {
            var stream = new MemoryStream(new byte[]{1, 2});
            var readBack = new byte[4];
            Assert.AreEqual(2, stream.Read(readBack, 0, 4));
            Assert.AreEqual(new byte[]{1, 2, 0, 0}, readBack);
        }

        [Test]
        public void ReadMoreThanIsInStreamWithNonZeroPosition()
        {
            var stream = new MemoryStream(new byte[]{1, 2});
            stream.Position = 1;
            var readBack = new byte[4];
            Assert.AreEqual(1, stream.Read(readBack, 0, 4));
            Assert.AreEqual(new byte[]{2, 0, 0, 0}, readBack);
        }

        [Test]
        public void Seek()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
            stream.Seek(0, SeekOrigin.Begin);
            AssertNextIs(stream, 1);
            stream.Seek(1, SeekOrigin.Begin);
            AssertNextIs(stream, 2);
            stream.Seek(-1, SeekOrigin.End);
            AssertNextIs(stream, 4);
            stream.Seek(-2, SeekOrigin.End);
            AssertNextIs(stream, 3);
            stream.Position = 2;
            stream.Seek(0, SeekOrigin.Current);
            AssertNextIs(stream, 3);
            stream.Seek(-2, SeekOrigin.Current);
            AssertNextIs(stream, 2);
        }

        [Test]
        public void ResizeDoublesTheSizeIfThatsEnough_OtherwiseIncreasesWithByteCount()
        {
            var stream = new MemoryStream();
            Assert.AreEqual(0, stream.Capacity);
            WriteBytes(stream, 1);
            Assert.AreEqual(256, stream.Capacity);
            WriteBytes(stream, 255);
            Assert.AreEqual(256, stream.Capacity);
            WriteBytes(stream, 1);
            Assert.AreEqual(512, stream.Capacity);
            WriteBytes(stream, 1000);
            Assert.AreEqual(1257, stream.Capacity);
            WriteBytes(stream, 1);
            Assert.AreEqual(2514, stream.Capacity);
        }

        [Test]
        public void GetBuffer()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] {1,2}, 0, 2);
            var buf = stream.GetBuffer();
            Assert.AreEqual(1, buf[0]);
            Assert.AreEqual(2, buf[1]);
        }

        private void WriteBytes(MemoryStream stream, int count)
        {
            stream.Write(new byte[count], 0, count);
        }

        private void AssertNextIs(MemoryStream stream, byte b)
        {
            var buf = new byte[1];
            stream.Read(buf, 0, 1);
            Assert.AreEqual(b, buf[0]);
        }

        private void AssertContentsAre(MemoryStream stream, byte[] contents)
        {
            stream.Position = 0;
            var readBack = new byte[contents.Length];
            Assert.AreEqual(contents.Length, stream.Read(readBack, 0, contents.Length));
            for (int i=0; i < contents.Length; ++i)
            {
                Assert.AreEqual(contents[i], readBack[i]);
            }
        }

        void ThrowsOnResize()
        {
            MemoryStream stream = null;
            try
            {
                // this part shoud not throw
                stream = new MemoryStream(new byte[]{1, 2});
                stream.Write(new byte[]{3, 4}, 0, 2);
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected exception", e);
            }

            // this part should throw
            stream.Write(new byte[]{5}, 0, 1);
        }

        [Test]
        public void WriteToNonResizable()
        {
            var stream = new MemoryStream(new byte[]{1, 2});
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual(0, stream.Position);
            Assert.IsTrue(stream.CanWrite);

            stream.Write(new byte[]{3, 4}, 0, 2);

            Assert.Throws<NotSupportedException>(ThrowsOnResize);
        }

        void ThrowsOnNullBuffer()
        {
            var stream = new MemoryStream(null);
        }

        [Test]
        public void CreateFromNull()
        {
            Assert.Throws<ArgumentNullException>(ThrowsOnNullBuffer);
        }
    }
}
