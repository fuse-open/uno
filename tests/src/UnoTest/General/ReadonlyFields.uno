using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class ReadonlyFields
    {
        struct Struct
        {
            public int Integer;

            public Struct(int value)
            {
                Integer = value;
            }

            public void DoubleInteger()
            {
                Integer *= 2;
            }
        }

        static readonly Struct ReadOnlyStruct = new Struct(10);
        static Struct NonReadOnlyStruct = new Struct(10);

        [Test]
        public void Run()
        {
            ReadOnlyStruct.DoubleInteger();
            NonReadOnlyStruct.DoubleInteger();

            var ros = ReadOnlyStruct;
            Assert.AreEqual(10, ros.Integer);
            Assert.AreEqual(10, ReadOnlyStruct.Integer);
            Assert.AreEqual(20, NonReadOnlyStruct.Integer);
        }

        class Class1
        {
            public readonly int ROInteger;
            public int Integer;
            public readonly Class1 RONext;
            public Class1 Next;

            public Class1(int i, Class1 next)
            {
                ROInteger = i;
                RONext = next;
            }
        }

        [Test]
        public void ClassFieldTest()
        {
            var inner = new Class1(10, null);
            var outer = new Class1(20, inner);

            Assert.AreEqual(10, inner.ROInteger);
            Assert.AreEqual(null, inner.RONext);
            Assert.AreEqual(20, outer.ROInteger);
            Assert.AreEqual(inner, outer.RONext);

            Assert.AreEqual(10, outer.RONext.ROInteger);
            Assert.AreEqual(null, outer.RONext.RONext);

            outer.Integer = 30;
            outer.RONext.Integer = 40;

            Assert.AreEqual(30, outer.Integer);
            Assert.AreEqual(40, outer.RONext.Integer);
            Assert.AreEqual(40, inner.Integer);

            var innerinner = new Class1(50, null);

            outer.RONext.Next = innerinner;

            Assert.AreEqual(innerinner, outer.RONext.Next);

            Assert.AreEqual(50, outer.RONext.Next.ROInteger);
            Assert.AreEqual(null, outer.RONext.Next.RONext);
        }

        static readonly Class1 Inner = new Class1(10, null);
        static readonly Class1 Outer = new Class1(20, Inner);

        [Test]
        public void StaticClassFieldTest()
        {
            Assert.AreEqual(10, Inner.ROInteger);
            Assert.AreEqual(null, Inner.RONext);
            Assert.AreEqual(20, Outer.ROInteger);
            Assert.AreEqual(Inner, Outer.RONext);

            Assert.AreEqual(10, Outer.RONext.ROInteger);
            Assert.AreEqual(null, Outer.RONext.RONext);

            Outer.Integer = 30;
            Outer.RONext.Integer = 40;

            Assert.AreEqual(30, Outer.Integer);
            Assert.AreEqual(40, Outer.RONext.Integer);
            Assert.AreEqual(40, Inner.Integer);

            var innerinner = new Class1(50, null);

            Outer.RONext.Next = innerinner;

            Assert.AreEqual(innerinner, Outer.RONext.Next);

            Assert.AreEqual(50, Outer.RONext.Next.ROInteger);
            Assert.AreEqual(null, Outer.RONext.Next.RONext);
        }

        struct Struct1
        {
            public readonly int ROInteger;
            public int Integer;
            public readonly Class1 RONext;
            public Class1 Next;

            public Struct1(int i, Class1 next)
            {
                ROInteger = i;
                RONext = next;
            }
        }

        [Test]
        public void StructFieldTest()
        {
            var inner = new Class1(10, null);
            var outer = new Struct1(20, inner);

            Assert.AreEqual(10, inner.ROInteger);
            Assert.AreEqual(null, inner.RONext);
            Assert.AreEqual(20, outer.ROInteger);
            Assert.AreEqual(inner, outer.RONext);

            Assert.AreEqual(10, outer.RONext.ROInteger);
            Assert.AreEqual(null, outer.RONext.RONext);

            outer.Integer = 30;
            outer.RONext.Integer = 40;

            Assert.AreEqual(30, outer.Integer);
            Assert.AreEqual(40, outer.RONext.Integer);
            Assert.AreEqual(40, inner.Integer);

            var innerinner = new Class1(50, null);

            outer.RONext.Next = innerinner;

            Assert.AreEqual(innerinner, outer.RONext.Next);

            Assert.AreEqual(50, outer.RONext.Next.ROInteger);
            Assert.AreEqual(null, outer.RONext.Next.RONext);
        }

        static readonly Class1 Inner2 = new Class1(10, null);
        static readonly Struct1 Outer2 = new Struct1(20, Inner2);

        [Test]
        public void StaticStructFieldTest()
        {
            Assert.AreEqual(10, Inner2.ROInteger);
            Assert.AreEqual(null, Inner2.RONext);
            Assert.AreEqual(20, Outer2.ROInteger);
            Assert.AreEqual(Inner2, Outer2.RONext);

            Assert.AreEqual(10, Outer2.RONext.ROInteger);
            Assert.AreEqual(null, Outer2.RONext.RONext);

            Outer2.Integer = 30;
            Outer2.RONext.Integer = 40;

            Assert.AreEqual(30, Outer2.Integer);
            Assert.AreEqual(40, Outer2.RONext.Integer);
            Assert.AreEqual(40, Inner2.Integer);

            var innerinner = new Class1(50, null);

            Outer2.RONext.Next = innerinner;

            Assert.AreEqual(innerinner, Outer2.RONext.Next);

            Assert.AreEqual(50, Outer2.RONext.Next.ROInteger);
            Assert.AreEqual(null, Outer2.RONext.Next.RONext);
        }
    }
}
