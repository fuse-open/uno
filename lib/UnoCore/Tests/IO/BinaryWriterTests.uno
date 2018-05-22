using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;

namespace UnoCore_Uno_IO
{
    public class BinaryWriterTests
    {
        public BinaryWriterTests()
        {
            Directory.SetCurrentDirectory(Directory.GetUserDirectory(UserDirectory.Data));
        }

        [Test]
        public void InitializeTest()
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            Assert.AreEqual(true, binaryWriter.LittleEndian);
            Assert.AreEqual(memoryStream, binaryWriter.BaseStream);
        }

        [Test]
        public void WriteBooleanTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteBoolean.bin"));
            w.Write(false);
            w.Dispose();
            Assert.AreEqual(false, new BinaryReader(File.OpenRead("TestWriteBoolean.bin")).ReadBoolean());
        }

        [Test]
        public void WriteByteTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteByte.bin"));
            w.Write((byte)255);
            w.Dispose();
            Assert.AreEqual((byte)255, new BinaryReader(File.OpenRead("TestWriteByte.bin")).ReadByte());
        }

        [Test]
        public void WriteSByteTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteSByte.bin"));
            w.Write((sbyte)-128);
            w.Dispose();
            Assert.AreEqual((sbyte)-128, new BinaryReader(File.OpenRead("TestWriteSByte.bin")).ReadSByte());
        }

        [Test]
        public void WriteCharTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteChar.bin"));
            w.Write('u');
            w.Write('©');
            w.Dispose();
            var r = new BinaryReader(File.OpenRead("TestWriteChar.bin"));
            Assert.AreEqual('u', r.ReadChar());
            Assert.AreEqual('©', r.ReadChar());
        }

        [Test]
        public void WriteIntTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteInt.bin"));
            w.Write(int.MinValue);
            w.Dispose();
            Assert.AreEqual(int.MinValue, new BinaryReader(File.OpenRead("TestWriteInt.bin")).ReadInt());
        }

        [Test]
        public void WriteLongTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteLong.bin"));
            w.Write(2147483648L);
            w.Dispose();
            Assert.AreEqual(2147483648L, new BinaryReader(File.OpenRead("TestWriteLong.bin")).ReadLong());
        }

        [Test]
        public void WriteSByte4Test()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteSByte4.bin"));
            var sbytes = sbyte4((sbyte)0, (sbyte)0, (sbyte)-128, (sbyte)127);
            w.Write(sbytes);
            w.Dispose();
            Assert.AreEqual(sbytes, new BinaryReader(File.OpenRead("TestWriteSByte4.bin")).ReadSByte4());
        }

        [Test]
        public void WriteUShort2Test()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteUShort2.bin"));
            w.Write(ushort2((ushort)0,(ushort)65535));
            w.Dispose();
            Assert.AreEqual(ushort2((ushort)0,(ushort)65535), new BinaryReader(File.OpenRead("TestWriteUShort2.bin")).ReadUShort2());
        }

        [Test]
        public void WriteFloat3x3Test()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteFloat3x3.bin"));
            var floats = float3x3(0,1,2,3,4,5,6,7,8);
            w.Write(floats);
            w.Dispose();
            Assert.AreEqual(floats, new BinaryReader(File.OpenRead("TestWriteFloat3x3.bin")).ReadFloat3x3());
        }

        [Test]
        public void WriteStringTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteString.bin"));
            w.Write("foo");
            w.Dispose();
            Assert.AreEqual("foo", new BinaryReader(File.OpenRead("TestWriteString.bin")).ReadString());
        }

        [Test]
        public void WriteBytesTest()
        {
            var w = new BinaryWriter(File.OpenWrite("TestWriteBytes.bin"));
            w.Write(new byte[] { 1, 2, 3 });
            w.Dispose();
            Assert.AreEqual(new byte[] { 1, 2, 3 }, new BinaryReader(File.OpenRead("TestWriteBytes.bin")).ReadBytes(3));
        }

        [Test]
        public void UsingOperator()
        {
            using (var stream = File.OpenWrite("TestDispose.bin"))
            {
                using (var writer = new BinaryWriter(stream))
                {
                }
            }
        }
    }
}
