using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class RefOut
    {
        private struct TestStruct
        {
            public TestStruct(int defaultValue)
            {
                TestProperty = defaultValue;
                TestField = defaultValue;
            }

            public int TestField;
            public int TestProperty { get; set; }
        }

        private class TestClass
        {
            public TestClass(int defaultValue)
            {
                TestProperty = defaultValue;
                TestField = defaultValue;
            }

            public int TestField;
            public int TestProperty { get; set; }
        }

        private int _testField;

        [Test]
        public void SimpleRefTests()
        {
            int a = 1;
            int b = 2;
            int c = 3;
            RefFunction1(a, ref b, c);
            Assert.AreEqual(1, a);
            Assert.AreEqual(6, b);
            Assert.AreEqual(3, c);

            int d = 1;
            int e = 2;
            RefFunction2(ref d, ref e);
            Assert.AreEqual(2, d);
            Assert.AreEqual(1, e);
        }

        [Test]
        public void SimpleOutTests()
        {
            int a;
            OutFunction1(out a);
            Assert.AreEqual(5, a);

            int b = 6;
            OutFunction1(out b);
            Assert.AreEqual(5, b);

            int c = 4;
            int d = 6;
            OutFunction2(c, out d);
            Assert.AreEqual(4, c);
            Assert.AreEqual(8, d);
        }

        [Test]
        public void IncrementRefArgument()
        {
            int t = 0;
            IncrementingRefFunction(ref t);
            Assert.AreEqual(t, 1);
        }

        [Test]
        public void IntermediateTests()
        {
            IntermediateFunction(1, 2);

            int a = 1;
            int b = 2;
            IntermediateRefFunction(ref a, ref b);
            Assert.AreEqual(2, a);
            Assert.AreEqual(1, b);
        }

        [Test]
        [Ignore("#276", "webgl")]
        public void StructRefTests()
        {
            var a = new TestStruct(100);
            StructRefFunction1(ref a);
            Assert.AreEqual(50, a.TestProperty);

            var b = new TestStruct(500);
            var c = new TestStruct(1000);
            StructRefFunction2(b, ref c);
            Assert.AreEqual(500, b.TestProperty);
            Assert.AreEqual(500, c.TestProperty);

            b.TestProperty = 888;
            Assert.AreEqual(888, b.TestProperty);
            Assert.AreEqual(500, c.TestProperty);
        }

        [Test]
        [Ignore("#276", "webgl")]
        public void StructOutTests()
        {
            TestStruct a;
            StructOutFunction1(out a);
            Assert.AreEqual(50, a.TestProperty);

            TestStruct b = new TestStruct(100);
            StructOutFunction1(out b);
            Assert.AreEqual(50, b.TestProperty);

            TestStruct c = new TestStruct(500);
            TestStruct d;
            StructOutFunction2(c, out d);
            Assert.AreEqual(500, c.TestProperty);
            Assert.AreEqual(500, d.TestProperty);

            c.TestProperty = 888;
            Assert.AreEqual(888, c.TestProperty);
            Assert.AreEqual(500, d.TestProperty);
        }

        [Test]
        public void ClassRefTests()
        {
            var a = new TestClass(100);
            ClassRefFunction1(ref a);
            Assert.AreEqual(50, a.TestProperty);

            var b = new TestClass(500);
            var c = new TestClass(1000);
            ClassRefFunction2(b, ref c);
            Assert.AreEqual(500, b.TestProperty);
            Assert.AreEqual(500, c.TestProperty);

            b.TestProperty = 888;
            Assert.AreEqual(888, b.TestProperty);
            Assert.AreEqual(888, c.TestProperty);
        }

        [Test]
        public void ClassOutTests()
        {
            TestClass a;
            ClassOutFunction1(out a);
            Assert.AreEqual(77, a.TestProperty);

            var b = new TestClass(100);
            ClassOutFunction1(out b);
            Assert.AreEqual(77, b.TestProperty);

            var c = new TestClass(500);
            TestClass d;
            ClassOutFunction2(c, out d);
            Assert.AreEqual(500, c.TestProperty);
            Assert.AreEqual(500, d.TestProperty);

            c.TestProperty = 888;
            Assert.AreEqual(888, c.TestProperty);
            Assert.AreEqual(888, d.TestProperty);
        }

        [Test]
        public void CalmVectorTests()
        {
            var j = float3(1.0f, 2.0f, 3.0f);
            CalmVector(ref j, .5f);
            Assert.AreEqual(float3(100.0f), j);

            j = float3(1.0f, 2.0f, 3.0f);
            CalmVector(out j, j, .5f);
            Assert.AreEqual(float3(100.0f), j);
        }

        [Test]
        public void ArrayRefTests()
        {
            var a = new int [] { 1, 2, 3 };
            var b = new int [] { 4, 5, 6 };
            ArrayRefFunction1(ref a, ref b);
            Assert.AreEqual(new int [] { 4, 5, 6 }, a);
            Assert.AreEqual(new int [] { 1, 2, 3 }, b);

            var c = new int [] { 11, 21, 31 };
            var d = new int [] { 41, 51, 61 };
            ArrayRefFunction2(ref c, d);
            Assert.AreEqual(new int [] { 41, 51, 61 }, c);
            Assert.AreEqual(new int [] { 41, 51, 61 }, d);
        }

        [Test]
        public void ArrayOutTests()
        {
            int [] a;
            ArrayOutFunction1(out a);
            Assert.AreEqual(new int [] { 100, 200, 300 }, a);

            var b = new int [] { 4, 5, 6 };
            ArrayOutFunction1(out b);
            Assert.AreEqual(new int [] { 100, 200, 300 }, b);

            var c = new int [] { 1, 2, 3 };
            var d = new int [] { 4, 5, 6 };
            ArrayOutFunction2(out c, d);
            Assert.AreEqual(new int [] { 4, 5, 6 }, c);
            Assert.AreEqual(new int [] { 4, 5, 6 }, d);
        }

        [Test]
        public void SimpleFieldRefTests()
        {
            int a = 1;
            _testField = 2;
            int c = 3;
            RefFunction1(a, ref _testField, c);
            Assert.AreEqual(1, a);
            Assert.AreEqual(6, _testField);
            Assert.AreEqual(3, c);

            _testField = 1;
            int b = 2;
            RefFunction2(ref _testField, ref b);
            Assert.AreEqual(2, _testField);
            Assert.AreEqual(1, b);
        }

        [Test]
        public void SimpleFieldOutTests()
        {
            OutFunction1(out _testField);
            Assert.AreEqual(5, _testField);

            _testField = 0;
            OutFunction1(out _testField);
            Assert.AreEqual(5, _testField);

            int a = 4;
            OutFunction2(a, out _testField);
            Assert.AreEqual(4, a);
            Assert.AreEqual(8, _testField);
        }

        [Test]
        public void ArrayMemberRefTests()
        {
            var a = new int [] { 1, 2, 3 };
            RefFunction3(ref a[1]);
            Assert.AreEqual(new int [] { 1, 101, 3 }, a);
        }

        [Test]
        public void ArrayMemberOutTests()
        {
            var a = new int [] { 1, 2, 3 };
            OutFunction1(out a[0]);
            Assert.AreEqual(new int [] { 5, 2, 3 }, a);
        }

        [Test]
        public void StructMemberRefTests()
        {
            var s = new TestStruct(10);
            RefFunction3(ref s.TestField);
            Assert.AreEqual(101, s.TestField);
        }

        [Test]
        public void StructMemberOutTests()
        {
            var s = new TestStruct(10);
            OutFunction1(out s.TestField);
            Assert.AreEqual(5, s.TestField);
        }

        [Test]
        public void ClassMemberRefTests()
        {
            var c = new TestClass(10);
            RefFunction3(ref c.TestField);
            Assert.AreEqual(101, c.TestField);
        }

        [Test]
        public void ClassMemberOutTests()
        {
            var c = new TestClass(10);
            OutFunction1(out c.TestField);
            Assert.AreEqual(5, c.TestField);
        }

        //helpers
        void RefFunction1(int a, ref int b, int c)
        {
            b = a + b + c;
            c = 90;
        }

        void RefFunction2(ref int a, ref int b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        void RefFunction3(ref int a)
        {
            a = 101;
        }

        void IncrementingRefFunction(ref int a)
        {
            a++;
        }

        void OutFunction1(out int a)
        {
            a = 5;
        }

        void OutFunction2(int a, out int b)
        {
            b = a * 2;
            a = 33;
        }

        void IntermediateFunction(int x, int y)
        {
            Assert.AreEqual(1, x);
            Assert.AreEqual(2, y);
            RefFunction2(ref x, ref y);
            Assert.AreEqual(2, x);
            Assert.AreEqual(1, y);
        }

        void IntermediateRefFunction(ref int i, ref int j)
        {
            Assert.AreEqual(1, i);
            Assert.AreEqual(2, j);
            RefFunction2(ref i, ref j);
            Assert.AreEqual(2, i);
            Assert.AreEqual(1, j);
        }

        void StructRefFunction1(ref TestStruct a)
        {
            a = new TestStruct(50);
        }

        void StructRefFunction2(TestStruct a, ref TestStruct b)
        {
            b = a;
        }

        void StructOutFunction1(out TestStruct a)
        {
            a = new TestStruct(50);
        }

        void StructOutFunction2(TestStruct a, out TestStruct b)
        {
            b = a;
        }

        void ClassRefFunction1(ref TestClass a)
        {
            a = new TestClass(50);
        }

        void ClassRefFunction2(TestClass a, ref TestClass b)
        {
            b = a;
        }

        void ClassOutFunction1(out TestClass a)
        {
            a = new TestClass(77);
        }

        void ClassOutFunction2(TestClass a, out TestClass b)
        {
            b = a;
        }

        void CalmVector(ref float3 inputFloat, float amount)
        {
            inputFloat = Math.Lerp(inputFloat, float3(0,0,0), amount);
            inputFloat = float3(100,100,100);
        }

        void CalmVector(out float3 outputFloat, float3 inputFloat, float amount)
        {
            outputFloat = Math.Lerp(inputFloat, float3(0,0,0), amount);
            outputFloat = float3(100,100,100);
        }

        void ArrayRefFunction1(ref int[] a, ref int[] b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        void ArrayRefFunction2(ref int[] a, int[] b)
        {
            a = b;
        }

        void ArrayOutFunction1(out int[] a)
        {
            a = new int[] { 100, 200, 300};
        }

        void ArrayOutFunction2(out int[] a, int[] b)
        {
            a = b;
        }
    }
}