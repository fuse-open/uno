using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Primitives
    {
        [Foreign(Language.ObjC)] static byte ByteId(byte x) @{ return x; @}
        [Foreign(Language.ObjC)] static sbyte SByteId(sbyte x) @{ return x; @}
        [Foreign(Language.ObjC)] static short ShortId(short x) @{ return x; @}
        [Foreign(Language.ObjC)] static ushort UShortId(ushort x) @{ return x; @}
        [Foreign(Language.ObjC)] static char CharId(char x) @{ return x; @}
        [Foreign(Language.ObjC)] static int IntId(int x) @{ return x; @}
        [Foreign(Language.ObjC)] static uint UIntId(uint x) @{ return x; @}
        [Foreign(Language.ObjC)] static long LongId(long x) @{ return x; @}
        [Foreign(Language.ObjC)] static ulong ULongId(ulong x) @{ return x; @}
        [Foreign(Language.ObjC)] static float FloatId(float x) @{ return x; @}
        [Foreign(Language.ObjC)] static double DoubleId(double x) @{ return x; @}
        [Foreign(Language.ObjC)] static bool BoolId(bool x) @{ return x; @}
        [Foreign(Language.ObjC)] static object ObjectId(object x) @{ return x; @}

        [Test]
        public void Identities()
        {
            Assert.AreEqual((byte)123, ByteId(123));
            Assert.AreEqual((sbyte)123, SByteId(123));
            Assert.AreEqual((short)123, ShortId(123));
            Assert.AreEqual((ushort)123, UShortId(123));
            Assert.AreEqual((char)123, CharId((char)123));
            Assert.AreEqual(123, IntId(123));
            Assert.AreEqual((uint)123, UIntId(123));
            Assert.AreEqual((long)123, LongId(123));
            Assert.AreEqual((ulong)123, ULongId(123));
            Assert.AreEqual((float)123, FloatId((float)123));
            Assert.AreEqual((double)123, DoubleId((double)123));
            Assert.AreEqual(false, BoolId(false));
            Assert.AreEqual(true, BoolId(true));
            var o = new object();
            Assert.AreEqual(o, ObjectId(o));
        }
    }
}
