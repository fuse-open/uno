using Uno;
using Uno.Collections;
using Uno.Graphics;
using Uno.Testing;
using Uno.Testing.Assert;

namespace Uno.Test
{
    [Obsolete]
    public class BufferTest
    {
        static byte[] GetRandomTestData(int bytes)
        {
            var data = new byte[bytes];
            var random = new Random((int)Uno.Diagnostics.Clock.GetSeconds());

            for (int i = 0; i < bytes; ++i)
            {
                data[i] = (byte)random.Next();
            }

            return data;
        }

        [Test]
        public void SizeInByteFromArray()
        {
            var data = GetRandomTestData(100);
            var buffer = new Buffer(data);
            Assert.AreEqual(buffer.SizeInBytes, 100);
            Assert.Throws<ArgumentNullException>(() => new Buffer(null, 0, 100));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Buffer(data, -1, 100));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Buffer(data, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Buffer(data, 1, 100));
        }

        [Test]
        public void SizeInBytesFromConstructorArg()
        {
            var buffer = new Buffer(1337);
            Assert.AreEqual(buffer.SizeInBytes, 1337);
        }

        [Test]
        public void CreateReadOnlySubBuffer()
        {
            var buffer = new Buffer(GetRandomTestData(100));
            var subBuffer = buffer.CreateSubBuffer(25, 50);

            Assert.AreEqual(subBuffer.SizeInBytes, 50);

            for (int i = 0; i < 50; ++i)
            {
                Assert.AreEqual(buffer.GetByte(25 + i), subBuffer.GetByte(i));
            }

        }



        ///========================================================
        ///    sbyte
        ///========================================================

        [Test]
        public void SetSignedByteOutOfBounds()
        {
            var buffer = new Buffer(4);
            buffer.Set(0, (sbyte)-1);
            buffer.Set(3, (sbyte)-1);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, (sbyte)-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (sbyte)-1));
        }

        [Test]
        public void SetSignedByte2OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, sbyte2((sbyte)-1, (sbyte)1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, sbyte2((sbyte)-1, (sbyte)1)));
        }

        [Test]
        public void SetSignedByte4OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, sbyte4((sbyte)-1, (sbyte)1, (sbyte)-2, (sbyte)2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, sbyte4((sbyte)-1, (sbyte)1, (sbyte)-2, (sbyte)2)));
        }

        [Test]
        public void GetSignedByte1OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.AreEqual(0, buffer.GetSByte(0));
            Assert.AreEqual(0, buffer.GetSByte(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte(-1));
        }

        [Test]
        public void GetSignedByte2OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte2(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte2(-1));
        }

        [Test]
        public void GetSignedByte4OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte4(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetSByte4(-1));
        }

        ///========================================================
        ///    byte
        ///========================================================

        [Test]
        public void SetByte1OutOfBounds()
        {
            var buffer = new Buffer(4);
            buffer.Set(0, (byte)1);
            buffer.Set(3, (byte)1);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, (byte)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (byte)1));
        }

        [Test]
        public void SetByte2OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, byte2((byte)1, (byte)1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, byte2((byte)1, (byte)1)));
        }

        [Test]
        public void SetByte4OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(4, byte4((byte)1, (byte)1, (byte)2, (byte)2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, byte4((byte)1, (byte)1, (byte)2, (byte)2)));
        }

        [Test]
        public void GetByte1OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.AreEqual(0, buffer.GetByte(0));
            Assert.AreEqual(0, buffer.GetByte(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte(-1));
        }

        [Test]
        public void GetByte2OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte2(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte2(-1));
        }

        [Test]
        public void GetByte4OutOfBounds()
        {
            var buffer = new Buffer(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte4(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetByte4(1));
        }

        ///========================================================
        ///    short
        ///========================================================

        [Test]
        public void GetShort1OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetShort(0));
            Assert.AreEqual(0, buffer.GetShort(10 - 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort(10 - 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort(-1));
        }

        [Test]
        public void GetShort2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort2(-1));
        }

        [Test]
        public void GetShort4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort4(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShort4(-1));
        }

        [Test]
        public void SetShort1OutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, (short)1);
            buffer.Set(10 - 2, (short)1);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 1, (short)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (short)1));
        }

        [Test]
        public void SetShort2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, short2((short)1, (short)1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, short2((short)1, (short)1)));
        }

        [Test]
        public void SetShort4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, short4((short)1, (short)1, (short)2, (short)2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, short4((short)1, (short)1, (short)2, (short)2)));
        }

        ///========================================================
        ///    ushort
        ///========================================================

        [Test]
        public void GetUShort1OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetUShort(0));
            Assert.AreEqual(0, buffer.GetUShort(10 - 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort(-1));
        }

        [Test]
        public void GetUShort2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort2(-1));
        }

        [Test]
        public void GetUShort4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort4(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUShort4(-1));
        }

        [Test]
        public void SetUShort1OutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, (ushort)1);
            buffer.Set(10 - 2, (ushort)1);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 1, (ushort)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (ushort)1));
        }

        [Test]
        public void SetUShort2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, ushort2((ushort)1, (ushort)1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, ushort2((ushort)1, (ushort)1)));
        }

        [Test]
        public void SetUShort4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, ushort4((ushort)1, (ushort)1, (ushort)2, (ushort)2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, ushort4((ushort)1, (ushort)1, (ushort)2, (ushort)2)));
        }

        ///========================================================
        ///    int
        ///========================================================

        [Test]
        public void GetInt1OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetInt(0));
            Assert.AreEqual(0, buffer.GetInt(10 - 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt(10 - 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt(-1));
        }

        [Test]
        public void GetInt2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt2(-1));
        }

        [Test]
        public void GetInt3OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt3(-1));
        }

        [Test]
        public void GetInt4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetInt4(-1));
        }

        [Test]
        public void SetInt1OutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, (int)1);
            buffer.Set(10 - 4, (int)1);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 3, (int)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (int)1));
        }

        [Test]
        public void SetInt2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, int2(1, 2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, int2(1, 2)));
        }

        [Test]
        public void SetInt3OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, int3(1, 2, 3)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, int3(1, 2, 3)));
        }

        [Test]
        public void SetInt4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, int4(1, 1, 2, 2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, int4(1, 1, 2, 2)));
        }

        ///========================================================
        ///    uint
        ///========================================================

        [Test]
        public void SetUIntOutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, (uint)10);
            buffer.Set(10 - 4, (uint)10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 3, (uint)10));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (uint)10));
        }

        [Test]
        public void GetUIntOutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetUInt(0));
            Assert.AreEqual(0, buffer.GetUInt(10 - 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUInt(10 - 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetUInt(-1));
        }

        ///========================================================
        ///    ulong
        ///========================================================

        [Test]
        public void SetULongOutOfBounds()
        {
            var buffer = new Buffer(20);
            buffer.Set(0, (ulong)10);
            buffer.Set(20 - 8, (ulong)10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(20 - 7, (ulong)10));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, (ulong)10));
        }

        [Test]
        public void GetULongOutOfBounds()
        {
            var buffer = new Buffer(20);
            Assert.AreEqual(0, buffer.GetULong(0));
            Assert.AreEqual(0, buffer.GetULong(20 - 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetULong(20 - 7));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetULong(-1));
        }

        ///========================================================
        ///    float
        ///========================================================


        [Test]
        public void GetFloat1OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetFloat(0));
            Assert.AreEqual(0, buffer.GetFloat(10 - 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat(10 - 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat(-1));
        }

        [Test]
        public void GetFloat2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat2(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat2(-1));
        }

        [Test]
        public void GetFloat3OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat3(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat3(-1));
        }

        [Test]
        public void GetFloat4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat4(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat4(-1));
        }

        [Test]
        public void SetFloat1OutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, 1.0f);
            buffer.Set(10 - 4, 1.0f);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 3, 1.0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, 1.0f));
        }

        [Test]
        public void SetFloat2OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, float2(2.0f, 2.0f)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, float2(2.0f, 2.0f)));
        }

        [Test]
        public void SetFloat3OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, float3(1.0f, 2.0f, 3.0f)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, float3(1.0f, 2.0f, 3.0f)));
        }

        [Test]
        public void SetFloat4OutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(9, float4(1.0f, 2.0f, 3.0f, 4.0f)));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, float4(1.0f, 2.0f, 3.0f, 4.0f)));
        }

        [Test]
        public void GetFloat3x3OutOfBounds()
        {
            var buffer = new Buffer(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat3x3(80));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat3x3(-1));
        }

        [Test]
        public void SetFloat3x3OutOfBounds()
        {
            var buffer = new Buffer(100);
            var m = float3x3(
                float3(1.0f),
                float3(2.0f),
                float3(3.0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(80, m));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, m));
        }

        [Test]
        public void GetFloat4x4OutOfBounds()
        {
            var buffer = new Buffer(200);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat4x4(150));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetFloat4x4(-1));
        }

        ///========================================================
        ///    double
        ///========================================================

        [Test]
        public void GetDoubleOutOfBounds()
        {
            var buffer = new Buffer(10);
            Assert.AreEqual(0, buffer.GetDouble(0));
            Assert.AreEqual(0, buffer.GetDouble(10 - 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDouble(10 - 7));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDouble(-1));
        }

        [Test]
        public void SetDoubleOutOfBounds()
        {
            var buffer = new Buffer(10);
            buffer.Set(0, 1000.0);
            buffer.Set(10 - 8, 1000.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(10 - 7, 1000.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Set(-1, 1000.0));
        }


        //===========================================================================================
        // read write tests
        //===========================================================================================


        ///========================================================
        ///    sbyte
        ///========================================================

        [Test]
        public void GetSignedByte()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0xFF, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 });
            Assert.AreEqual((sbyte)0, buffer.GetSByte(0));
            Assert.AreEqual((sbyte)-1, buffer.GetSByte(1));
            Assert.AreEqual((sbyte)-103, buffer.GetSByte(9));
        }

        [Test]
        public void SetSignedByte()
        {
            var buffer = new Buffer(10);
            var data = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 };

            for (int i = 0; i < data.Length; ++i)
            {
                buffer.Set(i, data[i]);
            }

            Assert.AreEqual((sbyte)0, buffer.GetSByte(0));
            Assert.AreEqual((sbyte)-103, buffer.GetSByte(9));

            buffer.Set(0, (sbyte)-1);
            buffer.Set(9, (sbyte)-127);

            Assert.AreEqual((sbyte)-1, buffer.GetSByte(0));
            Assert.AreEqual((sbyte)-127, buffer.GetSByte(9));

        }

        [Test]
        public void GetSignedByte2()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 });

            Assert.AreEqual(sbyte2((sbyte)0x00, (sbyte)0x11), buffer.GetSByte2(0));
            Assert.AreEqual(sbyte2((sbyte)0x22, (sbyte)0x33), buffer.GetSByte2(2));
            Assert.AreEqual(sbyte2((sbyte)0x44, (sbyte)0x55), buffer.GetSByte2(4));
            Assert.AreEqual(sbyte2((sbyte)0x66, (sbyte)0x77), buffer.GetSByte2(6));

        }

        [Test]
        public void SetSignedByte2()
        {
            var buffer = new Buffer(4);

            var a = sbyte2((sbyte)0x77, (sbyte)0x66);
            var b = sbyte2((sbyte)0x33, (sbyte)0x55);

            buffer.Set(0, a);
            buffer.Set(2, b);

            Assert.AreEqual(a, buffer.GetSByte2(0));
            Assert.AreEqual(b, buffer.GetSByte2(2));

            buffer.Set(1, a);

            Assert.AreEqual(a, buffer.GetSByte2(1));

        }

        [Test]
        public void GetSignedByte4()
        {
            var buffer = new Buffer(new byte[] { 0x11, 0x22, 0x33, 0x44 });
            var sb4 = buffer.GetSByte4(0);

            Assert.AreEqual(sb4.X, (sbyte)0x11);
            Assert.AreEqual(sb4.Y, (sbyte)0x22);
            Assert.AreEqual(sb4.Z, (sbyte)0x33);
            Assert.AreEqual(sb4.W, (sbyte)0x44);
        }

        [Test]
        public void SetSignedByte4()
        {
            var buffer = new Buffer(4);
            buffer.Set(0, sbyte4((sbyte)0x11, (sbyte)0x22, (sbyte)0x33, (sbyte)0x44));

            Assert.AreEqual((sbyte)0x11, buffer.GetSByte(0));
            Assert.AreEqual((sbyte)0x22, buffer.GetSByte(1));
            Assert.AreEqual((sbyte)0x33, buffer.GetSByte(2));
            Assert.AreEqual((sbyte)0x44, buffer.GetSByte(3));

        }

        ///========================================================
        ///    byte
        ///========================================================


        [Test]
        public void GetByte()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0xFF, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 });
            Assert.AreEqual(0, buffer.GetByte(0));
            Assert.AreEqual(255, buffer.GetByte(1));
            Assert.AreEqual(153, buffer.GetByte(9));
        }

        [Test]
        public void SetByte()
        {
            var buffer = new Buffer(10);
            var data = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 };

            for (int i = 0; i < data.Length; ++i)
            {
                buffer.Set(i, data[i]);
            }

            Assert.AreEqual(0, buffer.GetByte(0));
            Assert.AreEqual(153, buffer.GetByte(9));

            buffer.Set(0, (byte)255);
            buffer.Set(9, (byte)128);

            Assert.AreEqual(255, buffer.GetByte(0));
            Assert.AreEqual(128, buffer.GetByte(9));

        }

        [Test]
        public void GetByte2()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 });

            Assert.AreEqual(byte2(0x00, 0x11), buffer.GetByte2(0));
            Assert.AreEqual(byte2(0x22, 0x33), buffer.GetByte2(2));
            Assert.AreEqual(byte2(0x44, 0x55), buffer.GetByte2(4));
            Assert.AreEqual(byte2(0x66, 0x77), buffer.GetByte2(6));

        }

        [Test]
        public void SetByte2()
        {
            var buffer = new Buffer(4);

            var a = byte2(0x77, 0x66);
            var b = byte2(0x33, 0x55);

            buffer.Set(0, a);
            buffer.Set(2, b);

            Assert.AreEqual(a, buffer.GetByte2(0));
            Assert.AreEqual(b, buffer.GetByte2(2));

            buffer.Set(1, a);

            Assert.AreEqual(a, buffer.GetByte2(1));

        }

        [Test]
        public void GetByte4()
        {
            var buffer = new Buffer(new byte[] { 0x11, 0x22, 0x33, 0x44 });
            var b4 = buffer.GetByte4(0);

            Assert.AreEqual(b4.X, 0x11);
            Assert.AreEqual(b4.Y, 0x22);
            Assert.AreEqual(b4.Z, 0x33);
            Assert.AreEqual(b4.W, 0x44);
        }

        [Test]
        public void SetByte4()
        {
            var buffer = new Buffer(4);
            buffer.Set(0, byte4(0x11, 0x22, 0x33, 0x44));

            Assert.AreEqual(0x11, buffer.GetByte(0));
            Assert.AreEqual(0x22, buffer.GetByte(1));
            Assert.AreEqual(0x33, buffer.GetByte(2));
            Assert.AreEqual(0x44, buffer.GetByte(3));

        }

        ///========================================================
        ///    short
        ///========================================================

        [Test]
        public void GetShort()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F });

            var bigEndian = buffer.GetShort(0, false);
            Assert.AreEqual((short)-4545, bigEndian);

            var littleEndian = buffer.GetShort(0, true);
            Assert.AreEqual((short)16366, littleEndian);

        }

        [Test]
        public void SetShort()
        {
            var buffer = new Buffer(2);

            buffer.Set(0, (short)12345, false);

            Assert.AreEqual(buffer.GetByte(0), 0x30);
            Assert.AreEqual(buffer.GetByte(1), 0x39);

            buffer.Set(0, (short)12345, true);

            Assert.AreEqual(buffer.GetByte(0), 0x39);
            Assert.AreEqual(buffer.GetByte(1), 0x30);

        }

        [Test]
        public void GetShort2()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F, 0x30, 0x39 });

            var bigEndian = buffer.GetShort2(0, false);

            Assert.AreEqual((short)-4545, bigEndian.X);
            Assert.AreEqual((short)12345, bigEndian.Y);

            var littleEndian = buffer.GetShort2(0, true);

            Assert.AreEqual((short)16366, littleEndian.X);
            Assert.AreEqual((short)14640, littleEndian.Y);

        }

        [Test]
        public void SetShort2()
        {
            var buffer = new Buffer(4);

            buffer.Set(0, short2((short)-4545, (short)12345), false);

            Assert.AreEqual(buffer.GetByte(0), 0xEE);
            Assert.AreEqual(buffer.GetByte(1), 0x3F);
            Assert.AreEqual(buffer.GetByte(2), 0x30);
            Assert.AreEqual(buffer.GetByte(3), 0x39);

            buffer.Set(0, short2((short)-4545, (short)12345), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x39);
            Assert.AreEqual(buffer.GetByte(3), 0x30);

        }

        [Test]
        public void GetShort4()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F, 0x30, 0x39, 0x80, 0x80, 0x45, 0x65 });

            var bigEndian = buffer.GetShort4(0, false);

            Assert.AreEqual((short)-4545, bigEndian.X);
            Assert.AreEqual((short)12345, bigEndian.Y);
            Assert.AreEqual((short)-32640, bigEndian.Z);
            Assert.AreEqual((short)17765, bigEndian.W);


            var littleEndian = buffer.GetShort4(0, true);

            Assert.AreEqual((short)16366, littleEndian.X);
            Assert.AreEqual((short)14640, littleEndian.Y);
            Assert.AreEqual((short)-32640, littleEndian.Z);
            Assert.AreEqual((short)25925, littleEndian.W);

        }

        [Test]
        public void SetShort4()
        {
            var buffer = new Buffer(8);

            buffer.Set(0, short4((short)-4545, (short)12345, (short)-32640, (short)17765), false);

            Assert.AreEqual(buffer.GetByte(0), 0xEE);
            Assert.AreEqual(buffer.GetByte(1), 0x3F);
            Assert.AreEqual(buffer.GetByte(2), 0x30);
            Assert.AreEqual(buffer.GetByte(3), 0x39);
            Assert.AreEqual(buffer.GetByte(4), 0x80);
            Assert.AreEqual(buffer.GetByte(5), 0x80);
            Assert.AreEqual(buffer.GetByte(6), 0x45);
            Assert.AreEqual(buffer.GetByte(7), 0x65);

            buffer.Set(0, short4((short)-4545, (short)12345, (short)-32640, (short)17765), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x39);
            Assert.AreEqual(buffer.GetByte(3), 0x30);
            Assert.AreEqual(buffer.GetByte(4), 0x80);
            Assert.AreEqual(buffer.GetByte(5), 0x80);
            Assert.AreEqual(buffer.GetByte(6), 0x65);
            Assert.AreEqual(buffer.GetByte(7), 0x45);

        }

        ///========================================================
        ///    ushort
        ///========================================================

        [Test]
        public void GetUShort()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F });

            var bigEndian = buffer.GetUShort(0, false);
            Assert.AreEqual((ushort)60991, bigEndian);

            var littleEndian = buffer.GetUShort(0, true);
            Assert.AreEqual((ushort)16366, littleEndian);

        }

        [Test]
        public void SetUShort()
        {
            var buffer = new Buffer(2);

            buffer.Set(0, (ushort)12345, false);

            Assert.AreEqual(buffer.GetByte(0), 0x30);
            Assert.AreEqual(buffer.GetByte(1), 0x39);

            buffer.Set(0, (ushort)12345, true);

            Assert.AreEqual(buffer.GetByte(0), 0x39);
            Assert.AreEqual(buffer.GetByte(1), 0x30);

        }

        [Test]
        public void GetUShort2()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F, 0x30, 0x39 });

            var bigEndian = buffer.GetUShort2(0, false);

            Assert.AreEqual((ushort)60991, bigEndian.X);
            Assert.AreEqual((ushort)12345, bigEndian.Y);

            var littleEndian = buffer.GetUShort2(0, true);

            Assert.AreEqual((ushort)16366, littleEndian.X);
            Assert.AreEqual((ushort)14640, littleEndian.Y);

        }

        [Test]
        public void SetUShort2()
        {
            var buffer = new Buffer(4);

            buffer.Set(0, ushort2((ushort)60991, (ushort)12345), false);

            Assert.AreEqual(buffer.GetByte(0), 0xEE);
            Assert.AreEqual(buffer.GetByte(1), 0x3F);
            Assert.AreEqual(buffer.GetByte(2), 0x30);
            Assert.AreEqual(buffer.GetByte(3), 0x39);

            buffer.Set(0, ushort2((ushort)60991, (ushort)12345), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x39);
            Assert.AreEqual(buffer.GetByte(3), 0x30);

        }

        [Test]
        public void GetUShort4()
        {
            var buffer = new Buffer(new byte[] { 0xEE, 0x3F, 0x30, 0x39, 0x80, 0x80, 0x45, 0x65 });

            var bigEndian = buffer.GetUShort4(0, false);

            Assert.AreEqual((ushort)60991, bigEndian.X);
            Assert.AreEqual((ushort)12345, bigEndian.Y);
            Assert.AreEqual((ushort)32896, bigEndian.Z);
            Assert.AreEqual((ushort)17765, bigEndian.W);


            var littleEndian = buffer.GetUShort4(0, true);

            Assert.AreEqual((ushort)16366, littleEndian.X);
            Assert.AreEqual((ushort)14640, littleEndian.Y);
            Assert.AreEqual((ushort)32896, littleEndian.Z);
            Assert.AreEqual((ushort)25925, littleEndian.W);

        }


        [Test]
        public void SetUShort4()
        {
            var buffer = new Buffer(8);

            buffer.Set(0, ushort4((ushort)60991, (ushort)12345, (ushort)32896, (ushort)17765), false);

            Assert.AreEqual(buffer.GetByte(0), 0xEE);
            Assert.AreEqual(buffer.GetByte(1), 0x3F);
            Assert.AreEqual(buffer.GetByte(2), 0x30);
            Assert.AreEqual(buffer.GetByte(3), 0x39);
            Assert.AreEqual(buffer.GetByte(4), 0x80);
            Assert.AreEqual(buffer.GetByte(5), 0x80);
            Assert.AreEqual(buffer.GetByte(6), 0x45);
            Assert.AreEqual(buffer.GetByte(7), 0x65);

            buffer.Set(0, ushort4((ushort)60991, (ushort)12345, (ushort)32896, (ushort)17765), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x39);
            Assert.AreEqual(buffer.GetByte(3), 0x30);
            Assert.AreEqual(buffer.GetByte(4), 0x80);
            Assert.AreEqual(buffer.GetByte(5), 0x80);
            Assert.AreEqual(buffer.GetByte(6), 0x65);
            Assert.AreEqual(buffer.GetByte(7), 0x45);

        }

        ///========================================================
        ///    int
        ///========================================================

        [Test]
        public void GetInt()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0x00, 0xEE, 0x3F });

            var bigEndian = buffer.GetInt(0, false);
            Assert.AreEqual((int)60991, bigEndian);

            var littleEndian = buffer.GetInt(0, true);
            Assert.AreEqual((int)1072562176, littleEndian);

        }

        [Test]
        public void SetInt()
        {
            var buffer = new Buffer(4);

            buffer.Set(0, (int)12345, false);

            Assert.AreEqual(buffer.GetByte(0), 0x00);
            Assert.AreEqual(buffer.GetByte(1), 0x00);
            Assert.AreEqual(buffer.GetByte(2), 0x30);
            Assert.AreEqual(buffer.GetByte(3), 0x39);

            buffer.Set(0, (int)12345, true);


            Assert.AreEqual(buffer.GetByte(0), 0x39);
            Assert.AreEqual(buffer.GetByte(1), 0x30);
            Assert.AreEqual(buffer.GetByte(2), 0x00);
            Assert.AreEqual(buffer.GetByte(3), 0x00);


        }

        [Test]
        public void GetInt2()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0x00, 0xEE, 0x3F, 0x00, 0x00, 0x30, 0x39 });

            var bigEndian = buffer.GetInt2(0, false);

            Assert.AreEqual((int)60991, bigEndian.X);
            Assert.AreEqual((int)12345, bigEndian.Y);

            var littleEndian = buffer.GetInt2(0, true);

            Assert.AreEqual((int)1072562176, littleEndian.X);
            Assert.AreEqual((int)959447040, littleEndian.Y);

        }

        [Test]
        public void SetInt2()
        {
            var buffer = new Buffer(8);

            buffer.Set(0, int2((int)60991, (int)12345), false);

            Assert.AreEqual(buffer.GetByte(0), 0x00);
            Assert.AreEqual(buffer.GetByte(1), 0x00);
            Assert.AreEqual(buffer.GetByte(2), 0xEE);
            Assert.AreEqual(buffer.GetByte(3), 0x3F);
            Assert.AreEqual(buffer.GetByte(4), 0x00);
            Assert.AreEqual(buffer.GetByte(5), 0x00);
            Assert.AreEqual(buffer.GetByte(6), 0x30);
            Assert.AreEqual(buffer.GetByte(7), 0x39);


            buffer.Set(0, int2((int)60991, (int)12345), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x00);
            Assert.AreEqual(buffer.GetByte(3), 0x00);
            Assert.AreEqual(buffer.GetByte(4), 0x39);
            Assert.AreEqual(buffer.GetByte(5), 0x30);
            Assert.AreEqual(buffer.GetByte(6), 0x00);
            Assert.AreEqual(buffer.GetByte(7), 0x00);

        }

        [Test]
        public void GetInt4()
        {
            var buffer = new Buffer(new byte[] { 0x00, 0x00, 0xEE, 0x3F, 0x00, 0x00, 0x30, 0x39, 0x00, 0x00, 0x80, 0x80, 0x00, 0x00, 0x45, 0x65 });

            var bigEndian = buffer.GetInt4(0, false);

            Assert.AreEqual((int)60991, bigEndian.X);
            Assert.AreEqual((int)12345, bigEndian.Y);
            Assert.AreEqual((int)32896, bigEndian.Z);
            Assert.AreEqual((int)17765, bigEndian.W);


            var littleEndian = buffer.GetInt4(0, true);

            Assert.AreEqual((int)1072562176, littleEndian.X);
            Assert.AreEqual((int)959447040, littleEndian.Y);
            Assert.AreEqual((int)-2139095040, littleEndian.Z);
            Assert.AreEqual((int)1699020800, littleEndian.W);

        }


        [Test]
        public void SetInt4()
        {
            var buffer = new Buffer(16);

            buffer.Set(0, int4((int)60991, (int)12345, (int)32896, (int)17765), false);

            Assert.AreEqual(buffer.GetByte(0), 0x00);
            Assert.AreEqual(buffer.GetByte(1), 0x00);
            Assert.AreEqual(buffer.GetByte(2), 0xEE);
            Assert.AreEqual(buffer.GetByte(3), 0x3F);

            Assert.AreEqual(buffer.GetByte(4), 0x00);
            Assert.AreEqual(buffer.GetByte(5), 0x00);
            Assert.AreEqual(buffer.GetByte(6), 0x30);
            Assert.AreEqual(buffer.GetByte(7), 0x39);

            Assert.AreEqual(buffer.GetByte(8), 0x00);
            Assert.AreEqual(buffer.GetByte(9), 0x00);
            Assert.AreEqual(buffer.GetByte(10), 0x80);
            Assert.AreEqual(buffer.GetByte(11), 0x80);

            Assert.AreEqual(buffer.GetByte(12), 0x00);
            Assert.AreEqual(buffer.GetByte(13), 0x00);
            Assert.AreEqual(buffer.GetByte(14), 0x45);
            Assert.AreEqual(buffer.GetByte(15), 0x65);

            buffer.Set(0, int4((int)60991, (int)12345, (int)32896, (int)17765), true);

            Assert.AreEqual(buffer.GetByte(0), 0x3F);
            Assert.AreEqual(buffer.GetByte(1), 0xEE);
            Assert.AreEqual(buffer.GetByte(2), 0x00);
            Assert.AreEqual(buffer.GetByte(3), 0x00);

            Assert.AreEqual(buffer.GetByte(4), 0x39);
            Assert.AreEqual(buffer.GetByte(5), 0x30);
            Assert.AreEqual(buffer.GetByte(6), 0x00);
            Assert.AreEqual(buffer.GetByte(7), 0x00);

            Assert.AreEqual(buffer.GetByte(8), 0x80);
            Assert.AreEqual(buffer.GetByte(9), 0x80);
            Assert.AreEqual(buffer.GetByte(10), 0x00);
            Assert.AreEqual(buffer.GetByte(11), 0x00);

            Assert.AreEqual(buffer.GetByte(12), 0x65);
            Assert.AreEqual(buffer.GetByte(13), 0x45);
            Assert.AreEqual(buffer.GetByte(14), 0x00);
            Assert.AreEqual(buffer.GetByte(15), 0x00);

        }

    }
}
