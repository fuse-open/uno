using Uno;
using Uno;
using Foo;
using Foo2;
using Uno.Testing;

namespace Foo2
{
    class Baz
    {
        public Baz()
        {
            Assert.AreEqual(1, new Bar().Int);
        }
    }
}

namespace Foo
{
    using Uno;

    class Bar
    {
        public int Int = 2;
    }

    class Baz
    {
        class Bar
        {
            public int Int = 4;
        }

        class Bar<T>
        {
            public int Int = 3;
        }

        class Bar<T, U>
        {
            public int Int = 5;
        }

        class Bar2
        {
            public int Int = 4;
        }

        public Baz()
        {
            Assert.AreEqual(4, new Bar().Int);
            Assert.AreEqual(3, new Bar<int>().Int);
            Assert.AreEqual(5, new Bar<int, int>().Int);
            Assert.AreEqual(2, new Foo.Bar().Int);
            Assert.AreEqual(1, new global::Bar().Int);
            Assert.AreEqual(1, global.Foo);
        }
    }
}

class global
{
    public static int Foo = 1;
}

class Bar
{
    public int Int = 1;
}

namespace Namespace
{
    //using Namespace;

    enum ComponentType
    {
        Float,
        Int,
        UShort,
        Short,
        SByte,
        Byte
    }
}

namespace UnoTest.General
{
    using Uno.Math;
    using Uno.Vector;
    using Uno.Matrix;
    using UsingStaticStuff;
    using Namespace;

    class Foobar
    {
        block Quad
        {
            block Circle
            {
            }
        }
    }

    class UsingStaticStuff
    {
        public const float StaticConstant = 3.0f;

        public static float StaticField = 0;

        public static float StaticProperty
        {
            get { return 0; }
            set { }
        }
    }

    public class Members
    {
        apply Foobar.Quad;
        apply Foobar.Quad.Circle;

        [Test]
        public void Run()
        {
            new Foo.Baz();
            new Foo2.Baz();

            float2 a = float2(0,0), b = float2(0,0);
            float3 c = float3(0,0,0), d = float3(0,0,0);
            int e = 0, f = 0;

            Max(a, b);
            Max(c, d);
            Max(e, f);

            StaticField = StaticConstant;
            StaticProperty = StaticConstant;
            StaticField = StaticProperty;
            StaticProperty = StaticField;
        }

        static string Foo(sbyte val, bool test = true)
        {
            return "sbyte: " + val;
        }

        static string Foo(int val, bool test = true)
        {
            return "int: " + val;
        }

        [Test]
        public void OverloadsWithDefaultArguments()
        {
            Assert.AreEqual("int: 1", Foo((int)1));

            int tmp = 1;
            Assert.AreEqual("int: 1", Foo(tmp));
        }

        static string Bar(sbyte val)
        {
            return "sbyte: " + val;
        }

        [Test]
        public void OverloadWithImplicitConversion()
        {
            Assert.AreEqual("sbyte: 1", Bar((int)1));
        }
    }

    class AmbiguousInstanceFieldMemberOrStaticTypeMember
    {
        public ComponentType ComponentType;

        public int GetSizeOfComponentType()
        {
            switch (ComponentType)
            {
                case ComponentType.Byte:
                case ComponentType.SByte:
                    return 1;

                case ComponentType.Short:
                case ComponentType.UShort:
                    return 2;

                case ComponentType.Float:
                case ComponentType.Int:
                    return 4;

                default:
                    throw new Exception("Invalid component type: " + ComponentType);
            }
        }
    }

    class AmbiguousInstanceFieldOrDataTypeOrParameter
    {
        public ComponentType ComponentType;

        public static int GetSizeOfComponentType(ComponentType ComponentType)
        {
            switch (ComponentType)
            {
                case ComponentType.Byte:
                case ComponentType.SByte:
                    return 1;

                case ComponentType.Short:
                case ComponentType.UShort:
                    return 2;

                case ComponentType.Float:
                case ComponentType.Int:
                    return 4;

                default:
                    throw new Exception("Invalid component type: " + ComponentType);
            }
        }
    }
}

namespace Blocks {
    block Choco {
    }
}

namespace SomeNamespaceWithUniqueName987632
{
    using Blocks;

    namespace Appy {
        block Bar : Choco {
        }
    }
}

namespace AccessBugs
{
    public class Test1
    {
        protected Test2 Bla { get; set; }

        protected class Test2
        {

        }
    }
}
