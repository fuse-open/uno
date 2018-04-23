using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Operators
    {
        [Test]
        public void Cast()
        {
            // NOTE: this test uses {int,long}.Parse() to avoid constant-folding. Please do not try to be clever ;)

            Assert.AreEqual(65535, (int)(ushort)int.Parse("-1"));

            Assert.AreEqual(4294967295L, (long)(uint)long.Parse("-1"));

            // ensure that the types does not sign-extended
            Assert.IsTrue((long)(byte)long.Parse("-1") > 0);
            Assert.IsTrue((long)(ushort)long.Parse("-1") > 0);
            Assert.IsTrue((long)(uint)long.Parse("-1") > 0);

            // ensure that these types gets sign-extended
            Assert.IsTrue((long)(sbyte)long.Parse("-1") < 0);
            Assert.IsTrue((long)(short)long.Parse("-1") < 0);
            Assert.IsTrue((long)(int)long.Parse("-1") < 0);
        }

        [Test]
        public void AddWithOverflow()
        {
            // NOTE: this test uses {byte,ushort,uint,ulong}.Parse() to avoid constant-folding. Please do not try to be clever ;)

            // byte and ushort widens to int
            Assert.AreEqual(256, byte.MaxValue + byte.Parse("1"));
            Assert.AreEqual(65536, ushort.MaxValue + ushort.Parse("1"));

            // ...but uint and ulong should not
            Assert.AreEqual(0, uint.MaxValue + uint.Parse("1"));
            Assert.AreEqual(0, ulong.MaxValue + ulong.Parse("1"));
        }

        [Test]
        public void RightShift()
        {
            // NOTE: this test uses {uint,ulong}.Parse() to avoid constant-folding. Please do not try to be clever ;)

            Assert.AreEqual(2147483647, uint.Parse("4294967295") >> 1);
            Assert.AreEqual(9223372036854775807UL, ulong.Parse("18446744073709551615") >> 1);
        }

        [Test]
        public void Division()
        {
            // NOTE: this test uses int.Parse() to avoid constant-folding. Please do not try to be clever ;)

            Assert.AreEqual(-64, sbyte.MinValue / (sbyte)int.Parse("2")); // for some reason, sbyte is missing a Parse-method
            Assert.AreEqual(64, sbyte.MinValue / (sbyte)int.Parse("-2")); // for some reason, sbyte is missing a Parse-method
            Assert.AreEqual(64, ((byte)sbyte.MaxValue + 1) / byte.Parse("2"));

            Assert.AreEqual(-16384, short.MinValue / short.Parse("2"));
            Assert.AreEqual(16384, short.MinValue / short.Parse("-2"));
            Assert.AreEqual(16384, ((ushort)short.MaxValue + 1) / ushort.Parse("2"));

            Assert.AreEqual(-1073741824, int.MinValue / int.Parse("2"));
            Assert.AreEqual(1073741824, int.MinValue / int.Parse("-2"));
            Assert.AreEqual(1073741824, ((uint)int.MaxValue + 1) / uint.Parse("2"));

            Assert.AreEqual(-4611686018427387904, long.MinValue / long.Parse("2"));
            Assert.AreEqual(4611686018427387904, long.MinValue / long.Parse("-2"));
            Assert.AreEqual(4611686018427387904, ((ulong)long.MaxValue + 1) / ulong.Parse("2"));
        }

        [Test]
        public void Modulo()
        {
            // NOTE: this test uses int.Parse() to avoid constant-folding. Please do not try to be clever ;)

            Assert.AreEqual(1, (int)(((1ul << 63) | (ulong)int.Parse("1")) % 16));

            // signed modulo (int % int)
            Assert.AreEqual(15, int.Parse("15") % -16);
            Assert.AreEqual(0, int.Parse("16") % -16);
            Assert.AreEqual(-15, int.Parse("-15") % -16);
            Assert.AreEqual(0, int.Parse("-16") % -16);
            Assert.AreEqual(15, int.Parse("15") % 16);
            Assert.AreEqual(0, int.Parse("16") % 16);
            Assert.AreEqual(-15, int.Parse("-15") % 16);
            Assert.AreEqual(0, int.Parse("-16") % 16);
        }
    }
}
