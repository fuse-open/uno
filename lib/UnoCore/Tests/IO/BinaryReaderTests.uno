using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;

namespace UnoCore_Uno_IO
{
    public class BinaryReaderTests
    {
        [Test]
        public void InitializeTest()
        {
            var memoryStream = new MemoryStream();
            var binaryReader = new BinaryReader(memoryStream);
            Assert.AreEqual(true, binaryReader.LittleEndian);
            Assert.AreEqual(memoryStream, binaryReader.BaseStream);
        }

        [Test]
        public void ReadBooleanTest()
        {
            var reader = new BinaryReader(import("TestData/boolean.bin").OpenRead());
            Assert.AreEqual(true, reader.ReadBoolean());
        }

        [Test]
        public void ReadByteTest()
        {
            var reader = new BinaryReader(import("TestData/byte.bin").OpenRead());
            Assert.AreEqual(42, reader.ReadByte());
        }

        [Test]
        public void ReadCharTest()
        {
            var reader = new BinaryReader(import("TestData/char.bin").OpenRead());
            Assert.AreEqual('f', reader.ReadChar());
            Assert.AreEqual('æ', reader.ReadChar());
        }

        [Test]
        public void ReadIntTest()
        {
            var reader = new BinaryReader(import("TestData/int.bin").OpenRead());
            Assert.AreEqual(-100000, reader.ReadInt());
        }

        [Test]
        public void ReadLongTest()
        {
            var reader = new BinaryReader(import("TestData/long.bin").OpenRead());
            Assert.AreEqual(151515151515L, reader.ReadLong());
        }

        [Test]
        public void ReadFloatTest()
        {
            var reader = new BinaryReader(import("TestData/float.bin").OpenRead());
            Assert.AreEqual(13.37f, reader.ReadFloat());
        }

        [Test]
        public void ReadDoubleTest()
        {
            var reader = new BinaryReader(import("TestData/double.bin").OpenRead());
            Assert.AreEqual(13.38f, reader.ReadDouble());
        }

        [Test]
        public void ReadInt4Test()
        {
            var reader = new BinaryReader(import("TestData/int4.bin").OpenRead());
            Assert.AreEqual(int4(1,2,3,4), reader.ReadInt4());
        }

        [Test]
        public void ReadFloat3x3Test()
        {
            var reader = new BinaryReader(import("TestData/float3x3.bin").OpenRead());
            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), reader.ReadFloat3x3());
        }

        [Test]
        public void ReadStringTest()
        {
            var reader = new BinaryReader(import("TestData/string.bin").OpenRead());
            Assert.AreEqual("string data", reader.ReadString());
        }

        [Test]
        public void ReadEmptyStringTest()
        {
            var reader = new BinaryReader(import("TestData/empty_string.bin").OpenRead());
            Assert.AreEqual("", reader.ReadString());
        }

        [Test]
        public void ReadBytesTest()
        {
            var reader = new BinaryReader(import("TestData/bytes.bin").OpenRead());
            var readBytes = reader.ReadBytes(3);
            Assert.AreEqual(3, readBytes.Length);
            Assert.AreEqual(0x85, readBytes[0]);
            Assert.AreEqual(0xeb, readBytes[1]);
            Assert.AreEqual(0x55, readBytes[2]);
        }

        [Test]
        public void ReadZeroBytesTest()
        {
            var reader = new BinaryReader(import("TestData/bytes.bin").OpenRead());
            var readBytes = reader.ReadBytes(0);
            Assert.AreEqual(0, readBytes.Length);
        }
    }
}
