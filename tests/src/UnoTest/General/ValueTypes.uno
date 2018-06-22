using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace UnoTest.General
{
    struct Value
    {
        int A, B;

        public Value(int a, int b)
        {
            A = a;
            B = b;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }
    }

    class ValueTypes
    {
        [Test]
        public static void Test()
        {
            var a = (object) new Value(1, 2);
            var b = (object) new Value(1, 2);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }
    }

    [Set("TypeName", "int")]
    extern(CPLUSPLUS) struct CppValueHandle
    {
    }

    extern(CPLUSPLUS) class CppValue
    {
        readonly CppValueHandle _value;

        public override bool Equals(object o)
        {
            var that = o as CppValue;
            return that != null && _value.Equals(that._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        [Test]
        public static void Test()
        {
            var a = new CppValue();
            var b = new CppValue();
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }
    }
}
