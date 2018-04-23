using Uno;
using Uno.Testing;

namespace Uno.Test
{

    public static class GuidExtensions
    {
        public static int GetVersion(this Guid guid)
        {
            return guid.ToByteArray()[7] >> 4;
        }

        public static int GetVariant(this Guid guid)
        {
            var variantByte = guid.ToByteArray()[8];
            if ((variantByte >> 7) == 0)
                return 0;
            if (((variantByte >> 6) & 3) == 2)
                return 1;
            switch ((variantByte >> 5) & 7)
            {
                case 6: return 2;
                case 7: return 3; // reserved for future use
                default: return -1;
            }
        }
    }

    class GuidTest
    {
        [Test]
        public void SimpleConstructorsAndToString()
        {
            Assert.AreEqual(
                "00000001-0002-0003-0405-060708090a0b",
                new Guid(1,2,3,4,5,6,7,8,9,10,11).ToString());

            Assert.AreEqual(
                "dfd002cd-55ed-4662-8e62-fd2a18c9cf61",
                new Guid(0xdfd002cd,0x55ed,0x4662,0x8e,0x62,0xfd,0x2a,0x18,0xc9,0xcf,0x61).ToString());
        }

        [Test]
        public void ByteArrayConstructor()
        {
            var bytes = new byte[] {0x37, 0x7a, 0x8b, 0x0c, 0x82, 0x75, 0xce, 0x46, 0xa4, 0xad, 0xb7, 0xd1, 0xa6, 0xe9, 0x20, 0x28};
            Assert.AreEqual(
                "0c8b7a37-7582-46ce-a4ad-b7d1a6e92028",
                new Guid(bytes).ToString());
            var big_bytes = new byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff};
            Assert.AreEqual(
                "ffffffff-ffff-ffff-ffff-ffffffffffff",
                new Guid(big_bytes).ToString());
        }

        [Test]
        public void ByteArrayConstructorThrowsForNull()
        {
            Assert.Throws<ArgumentNullException>(ByteArrayConstructorWithNull);
        }

        void ByteArrayConstructorWithNull()
        {
            new Guid((byte[])null);
        }

        [Test]
        public void ByteArrayConstructorThrowsForWrongLength()
        {
            Assert.Throws<ArgumentException>(ByteArrayConstructorWithNull);
        }

        void ByteArrayConstructorWithWrongLength()
        {
            new Guid(new byte[15]);
        }

        [Test]
        public void MixedIntegerAndByteArrayConstructor()
        {
            var bytes = new byte[] {0xb7, 0xe6, 0x45, 0x96, 0xfe, 0xf2, 0xa8, 0x2b};
            Assert.AreEqual(
                "0659416e-0f1c-4a52-b7e6-4596fef2a82b",
                new Guid((int)0x0659416e, (short)0x0f1c, (short)0x4a52, bytes).ToString());

            var big_bytes = new byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff};
            Assert.AreEqual(
                "ffffffff-ffff-ffff-ffff-ffffffffffff",
                new Guid((int)0xffffffff, (short)0xffff, (short)0xffff, big_bytes).ToString());
        }

        [Test]
        public void MixedIntegerAndByteArrayConstructorThrowsForNull()
        {
            Assert.Throws<ArgumentNullException>(MixedIntegerAndByteArrayConstructorWithNull);
        }

        void MixedIntegerAndByteArrayConstructorWithNull()
        {
            new Guid(1,2,3,null);
        }

        [Test]
        public void MixedIntegerAndByteArrayConstructorThrowsForWrongLength()
        {
            Assert.Throws<ArgumentException>(MixedIntegerAndByteArrayConstructorWithWrongLength);
        }

        void MixedIntegerAndByteArrayConstructorWithWrongLength()
        {
            new Guid(1,2,3, new byte[7]);
        }

        [Test]
        public void StringContsructor()
        {
            Assert.AreEqual("c375bf01-009e-4a7a-aa3a-b595ed6808a2", new Guid("c375bf01-009e-4a7a-aa3a-b595ed6808a2").ToString());
        }

        [Test]
        public void StringConstructorThrowsForNull()
        {
            Assert.Throws<ArgumentException>(StringConstructorWithNull);
        }

        void StringConstructorWithNull()
        {
            new Guid((string)null);
        }

        [Test]
        public void StringConstructorThrowsForInvalidGuids()
        {
            Assert.Throws<FormatException>(MalformedGuidEmpty);
            Assert.Throws<FormatException>(MalformedGuidTooFewGroups);
            Assert.Throws<FormatException>(MalformedGuidTooManyGroups);
            Assert.Throws<FormatException>(MalformedGuidGroupWrongLength1);
            Assert.Throws<FormatException>(MalformedGuidGroupWrongLength2);
            Assert.Throws<FormatException>(MalformedGuidGroupWrongLength3);
            Assert.Throws<FormatException>(MalformedGuidGroupWrongLength4);
            Assert.Throws<FormatException>(MalformedGuidGroupWrongLength5);
            Assert.Throws<FormatException>(MalformedGuidInvalidCharacter1);
        }

        void MalformedGuidEmpty() { new Guid(""); }
        void MalformedGuidTooFewGroups() { new Guid("00000000-0000"); }
        void MalformedGuidTooManyGroups() { new Guid("00000000-0000-0000-0000-000000000000-0"); }
        void MalformedGuidGroupWrongLength1() { new Guid("0000000-0000-0000-0000-000000000000"); }
        void MalformedGuidGroupWrongLength2() { new Guid("00000000-000-0000-0000-000000000000"); }
        void MalformedGuidGroupWrongLength3() { new Guid("00000000-0000-000-0000-000000000000"); }
        void MalformedGuidGroupWrongLength4() { new Guid("00000000-0000-0000-000-000000000000"); }
        void MalformedGuidGroupWrongLength5() { new Guid("00000000-0000-0000-0000-00000000000"); }
        void MalformedGuidInvalidCharacter1() { new Guid("g0000000-0000-0000-0000-000000000000"); }
        void MalformedGuidInvalidCharacter2() { new Guid("00000000-g000-0000-0000-000000000000"); }
        void MalformedGuidInvalidCharacter3() { new Guid("00000000-0000-g000-0000-000000000000"); }
        void MalformedGuidInvalidCharacter4() { new Guid("00000000-0000-0000-g000-000000000000"); }
        void MalformedGuidInvalidCharacter5() { new Guid("00000000-0000-0000-0000-g00000000000"); }

        [Test]
        public void EqualGuidsAreEqual()
        {
            var g1 = new Guid(1,2,3,4,5,6,7,8,9,10,11);
            var g2 = new Guid(1,2,3,4,5,6,7,8,9,10,11);

            Assert.IsTrue(g1.Equals(g1));
            Assert.IsTrue(g1.Equals(g2));
            Assert.IsTrue(g2.Equals(g1));

            Assert.IsTrue(((object)g1).Equals(g1));
            Assert.IsTrue(((object)g1).Equals(g2));
            Assert.IsTrue(((object)g2).Equals(g1));
            Assert.IsTrue(g1.Equals((object)g1));
            Assert.IsTrue(g1.Equals((object)g2));
            Assert.IsTrue(g2.Equals((object)g1));

            Assert.IsTrue(g1 == g1);
            Assert.IsTrue(g1 == g2);
            Assert.IsTrue(g2 == g1);
        }

        [Test]
        public void UnEqualGuidsAreNotEqual()
        {
            var g  = new Guid(1,1,1,1,1,1,1,1,1,1,1);
            var g0 = new Guid(0,1,1,1,1,1,1,1,1,1,1);
            var g1 = new Guid(1,0,1,1,1,1,1,1,1,1,1);
            var g2 = new Guid(1,1,0,1,1,1,1,1,1,1,1);
            var g3 = new Guid(1,1,1,0,1,1,1,1,1,1,1);
            var g4 = new Guid(1,1,1,1,0,1,1,1,1,1,1);
            var g5 = new Guid(1,1,1,1,1,0,1,1,1,1,1);
            var g6 = new Guid(1,1,1,1,1,1,0,1,1,1,1);
            var g7 = new Guid(1,1,1,1,1,1,1,0,1,1,1);
            var g8 = new Guid(1,1,1,1,1,1,1,1,0,1,1);
            var g9 = new Guid(1,1,1,1,1,1,1,1,1,0,1);
            var ga = new Guid(1,1,1,1,1,1,1,1,1,1,0);

            Assert.IsFalse(g.Equals(g0));
            Assert.IsFalse(g.Equals(g1));
            Assert.IsFalse(g.Equals(g2));
            Assert.IsFalse(g.Equals(g3));
            Assert.IsFalse(g.Equals(g4));
            Assert.IsFalse(g.Equals(g5));
            Assert.IsFalse(g.Equals(g6));
            Assert.IsFalse(g.Equals(g7));
            Assert.IsFalse(g.Equals(g8));
            Assert.IsFalse(g.Equals(g9));
            Assert.IsFalse(g.Equals(ga));

            Assert.IsFalse(((object)g).Equals(g0));
            Assert.IsFalse(g.Equals((object)g0));

            Assert.IsFalse(g == g0);
            Assert.IsFalse(g == g1);
            Assert.IsFalse(g == g2);
            Assert.IsFalse(g == g3);
            Assert.IsFalse(g == g4);
            Assert.IsFalse(g == g5);
            Assert.IsFalse(g == g6);
            Assert.IsFalse(g == g7);
            Assert.IsFalse(g == g8);
            Assert.IsFalse(g == g9);
            Assert.IsFalse(g == ga);
        }

        [Test]
        public void GuidsAreNotEqualToOtherTypes()
        {
            Assert.IsFalse(new Guid().Equals("horse"));
        }

        [Test]
        public void GuidsAreNotEqualToNull()
        {
            Assert.IsFalse(new Guid().Equals(null));
        }

        [Test]
        public void Empty()
        {
            Assert.AreEqual("00000000-0000-0000-0000-000000000000", Guid.Empty.ToString());
        }

        [Test]
        public void GuidFromToByteArrayMatches()
        {
            var g0 = new Guid(1,2,3,4,5,6,7,8,9,10,11);
            var g1 = new Guid(g0.ToByteArray());
            Assert.AreEqual(g0, g1);
        }

        [Test]
        public void TwoRandomIDsDontMatch()
        {
            Assert.AreNotEqual(Guid.NewGuid(), Guid.NewGuid());
        }

        [Test]
        public void TwoRandomIDsVersionsMatch()
        {
            var g0 = Guid.NewGuid();
            var g1 = Guid.NewGuid();
            Assert.AreEqual(g0.GetVersion(), g1.GetVersion());
            Assert.AreEqual(g0.GetVariant(), g1.GetVariant());
        }
    }
}
