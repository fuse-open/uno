using Uno;
using Uno.Testing;

enum RootNamespaceEnum
{
    Foo,
    Bar
}

namespace UnoTest.General
{
    public class Enums
    {
        enum Foo
        {
            A = 1 << 0,
            B = 1 << 1,
            C = 1 << 2,
        }

        enum LongFoo : long
        {
            A = 1,
        }

        enum UIntFoo : uint
        {
            A = 1,
        }

        enum ShortFoo : short
        {
            A = 1,
        }

        enum SByteFoo : sbyte
        {
            A = 1,
        }
/*
        enum FloatFoo : float
        {
            A = 1,
        }
*/
        [Test]
        public void Run()
        {
            var f = Foo.A | Foo.B | Foo.C;
            var c = f ^ Foo.B;
            var d = f - Foo.A;
            var e = ~f;

            Assert.IsTrue(f.HasFlag(Foo.B));
            Assert.IsTrue((f & (Foo.A | Foo.B)) == (Foo.A | Foo.B));

            var c1 = (byte)f;
            var c2 = (sbyte)f;
            var c3 = (short)f;
            var c4 = (ushort)f;
            var c5 = (int)f;
            var c6 = (uint)f;
            var c7 = (long)f;
            var c8 = (ulong)f;
            var c9 = (float)f;
            var c0 = (double)f;

            var d1 = (Foo)c1;
            var d2 = (Foo)c2;
            var d3 = (Foo)c3;
            var d4 = (Foo)c4;
            var d5 = (Foo)c5;
            var d6 = (Foo)c6;
            var d7 = (Foo)c7;
            var d8 = (Foo)c8;
            var d9 = (Foo)c9;
            var d0 = (Foo)c0;

            Assert.AreEqual(8, sizeof(LongFoo));
            Assert.AreEqual(4, sizeof(UIntFoo));
            Assert.AreEqual(2, sizeof(ShortFoo));
            Assert.AreEqual(1, sizeof(SByteFoo));
        }
    }
}
