using Uno;
using Uno.Testing;
using UnoTest.General;

namespace UnoTest.General
{
    public class Arrays
    {
        [Test]
        public void Run()
        {
            var a = new byte[] { 1, 2, 3 };
            var b = new sbyte[] { 1, 2, 3 };
            var c = new short[] { 1, 2, 3 };
            var d = new ushort[] { 1, 2, 3 };
            var e = new int[] { 1, 2, 3 };
            var f = new uint[] { 1, 2, 3 };
            var g = new long[] { 1, 2, 3 };
            var h = new ulong[] { 1, 2, 3 };
            var i = new float[] { 1, 2, 3 };
            var j = new double[] { 1, 2, 3 };
            var k = new bool[] { true, false };
            var l = new[] { byte2(1), byte2(2), byte2(3) };
            var m = new[] { short2(1), short2(2), short2(3) };
            var n = new[] { float3(1), float3(2), float3(3) };
            var o = new[] { int4(1), int4(2), int4(3) };
            var p = new object[] { null, "foo" };
            var q = new[] { new NonTrivial("foo") };

            assert a[0] == 1;
            assert b[0] == 1;
            assert c[0] == 1;
            assert d[0] == 1;
            assert e[0] == 1;
            assert f[0] == 1;
            assert g[0] == 1;
            assert h[0] == 1;
            assert i[0] == 1;
            assert j[0] == 1;
            assert k[0] == true;
            assert l[0] == byte2(1);
            assert m[0] == short2(1);
            assert n[0] == float3(1);
            assert o[0] == int4(1);
            assert p[0] == null;
            assert q[0].Object == "foo";
        }

        struct NonTrivial
        {
            public readonly object Object;

            public NonTrivial(object obj)
            {
                Object = obj;
            }
        }
    }
}
