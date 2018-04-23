using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class SizeOf
    {
        enum UserEnum
        {
            Foo,
            Bar,
        }

        struct UserStruct
        {
            int i;
            float f;
            float4x4 m;
            byte b;
        }

        [Test]
        public void Run()
        {
            Assert.AreEqual(1, sizeof(byte));
            Assert.AreEqual(1, sizeof(sbyte));

            Assert.AreEqual(2, sizeof(char));
            Assert.AreEqual(2, sizeof(short));
            Assert.AreEqual(2, sizeof(ushort));

            Assert.AreEqual(4, sizeof(int));
            Assert.AreEqual(4, sizeof(uint));

            Assert.AreEqual(8, sizeof(long));
            Assert.AreEqual(8, sizeof(ulong));

            Assert.AreEqual(4, sizeof(float));
            Assert.AreEqual(8, sizeof(double));

            Assert.AreEqual(2, sizeof(byte2));
            Assert.AreEqual(4, sizeof(byte4));

            Assert.AreEqual(2, sizeof(sbyte2));
            Assert.AreEqual(4, sizeof(sbyte4));

            Assert.AreEqual(4, sizeof(short2));
            Assert.AreEqual(8, sizeof(short4));

            Assert.AreEqual(4, sizeof(ushort2));
            Assert.AreEqual(8, sizeof(ushort4));

            Assert.AreEqual(8, sizeof(int2));
            Assert.AreEqual(12, sizeof(int3));
            Assert.AreEqual(16, sizeof(int4));

            Assert.AreEqual(8, sizeof(float2));
            Assert.AreEqual(12, sizeof(float3));
            Assert.AreEqual(16, sizeof(float4));
            Assert.AreEqual(9 * 4, sizeof(float3x3));
            Assert.AreEqual(16 * 4, sizeof(float4x4));

            Assert.AreEqual(4, sizeof(UserEnum));
            Assert.AreEqual(76, sizeof(UserStruct)); // 3 bytes padding
        }
    }
}
