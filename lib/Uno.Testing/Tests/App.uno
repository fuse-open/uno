using Uno;
using Uno.Collections;
using Uno.Testing;


namespace Uno.Testing.Tests
{

    public class Tests
    {
        [Test]
        public void AreEqualTests()
        {
            Assert.AreEqual(null,null);
            Assert.AreEqual(1, 1);
            Assert.AreEqual("foo", "foo");
            Assert.AreEqual(new ClassWithEquals(), new ClassWithEquals());
            Assert.Throws<AssertionFailedException>(AreNotEqual1);
            Assert.Throws<AssertionFailedException>(AreNotEqual2);
            Assert.Throws<AssertionFailedException>(AreNotEqual3);
            Assert.Throws<AssertionFailedException>(LeftIsNull);
            Assert.Throws<AssertionFailedException>(RightIsNull);
        }

        private void AreNotEqual1()
        {
            Assert.AreEqual(1, 2);
        }

        private void AreNotEqual2()
        {
            Assert.AreEqual("foo", "bar");
        }

        private void AreNotEqual3()
        {
            Assert.AreEqual(new ClassWithoutEquals(), new ClassWithoutEquals());
        }

        private void LeftIsNull()
        {
            Assert.AreEqual(null, "foo");
        }

        private void RightIsNull()
        {
            Assert.AreEqual("foo", null);
        }

        [Test]
        public void AreNotEqualTests()
        {
            Assert.AreNotEqual(1,2);
            Assert.AreNotEqual("foo", "bar");
            Assert.AreNotEqual(new ClassWithoutEquals(), new ClassWithoutEquals());
            Assert.AreNotEqual(null, 1);
            Assert.AreNotEqual(1, null);
            Assert.Throws<AssertionFailedException>(AreEqual1);
            Assert.Throws<AssertionFailedException>(AreEqual2);
            Assert.Throws<AssertionFailedException>(AreEqual3);
            Assert.Throws<AssertionFailedException>(AreEqualBothNull);
        }

        private void AreEqual1()
        {
            Assert.AreNotEqual(1, 1);
        }

        private void AreEqual2()
        {
            Assert.AreNotEqual("foo", "foo");
        }

        private void AreEqual3()
        {
            Assert.AreNotEqual(new ClassWithEquals(), new ClassWithEquals());
        }

        private void AreEqualBothNull()
        {
            Assert.AreNotEqual(null, null);
        }

        [Test]
        public void IsTrueTests()
        {
            Assert.IsTrue(1 == 1);
            Assert.Throws<AssertionFailedException>(IsNotTrue);
        }

        private void IsNotTrue()
        {
            Assert.IsTrue(1 == 2);
        }

        [Test]
        public void IsFalseTests()
        {
            Assert.IsFalse(1 == 2);
            Assert.Throws<AssertionFailedException>(IsNotFalse);
        }

        private void IsNotFalse()
        {
            Assert.IsFalse(1 == 1);
        }

        [Test]
        public void ThrowTests()
        {
            var ex = Assert.Throws(ThrowsCustomException);
            Assert.AreEqual("Custom exception message", ex.Message);

            ex = Assert.Throws<CustomException>(ThrowsCustomException);
            Assert.AreEqual("Custom exception message", ex.Message);

            ex = Assert.Throws<Exception>(ThrowsCustomException);
            Assert.AreEqual("Custom exception message", ex.Message);

            try
            {
                Assert.Throws(ThisDoesNotThrow);
                throw new Exception("Test failed");
            }
            catch (AssertionFailedException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail("");
            }

            try
            {
                Assert.Throws<OtherException>(ThrowsCustomException);
                Assert.Fail("");
            }
            catch (CustomException e)
            {
            }
        }

        [Test]
        public void DoesNotThrowTests()
        {
            Assert.DoesNotThrowAny(ThisDoesNotThrow);
            try
            {
                Assert.DoesNotThrowAny(ThrowsCustomException);
                Assert.Fail("");
            }
            catch (AssertionFailedException e)
            {
            }
        }

        private void ThrowsCustomException()
        {
            throw new CustomException("Custom exception message");
        }

        private void ThisDoesNotThrow()
        {
        }

        [Test]
        public void ContainsTests()
        {
            List<int> ints = new List<int> {1,2,3};
            Assert.Contains(1, ints);
            Assert.Throws<AssertionFailedException>(DoesntContain1);

            double[] doubles = new double[] {1.0, 2.0, 3.0};
            Assert.Contains(1.0, doubles);
            Assert.Throws<AssertionFailedException>(DoesntContain2);

            var cwoe = new ClassWithoutEquals();
            List<ClassWithoutEquals> cwoes = new List<ClassWithoutEquals> { cwoe };
            Assert.Contains(cwoe, cwoes);
            Assert.Throws<AssertionFailedException>(DoesntContain3);

            var cwe1 = new ClassWithEquals();
            var cwe2 = new ClassWithEquals();
            List<ClassWithEquals> cwes = new List<ClassWithEquals> { cwe1 };
            Assert.Contains(cwe1, cwes);
            Assert.Contains(cwe2, cwes);

        }

        private void DoesntContain1()
        {
            List<int> ints = new List<int> {1,2,3};
            Assert.Contains(4, ints);
        }

        private void DoesntContain2()
        {
            double[] doubles = new double[] {1.0, 2.0, 3.0};
            Assert.Contains(4.0, doubles);
        }

        private void DoesntContain3()
        {
            var cwoe1 = new ClassWithoutEquals();
            var cwoe2 = new ClassWithoutEquals();
            List<ClassWithoutEquals> cwoes = new List<ClassWithoutEquals> { cwoe1 };
            Assert.Contains(cwoe2, cwoes);
        }

        [Test]
        public void ContainsTestsForString()
        {
            Assert.Contains("a", "abc");
            Assert.Contains("ab", "abc");
            Assert.Contains("abc", "abc");
            Assert.Contains("bc", "abc");
            Assert.Contains("c", "abc");
            Assert.Contains("bcd", "abcbcd");
            Assert.Contains("", "");
            Assert.Contains("", "abc");
            Assert.Throws<AssertionFailedException>(DoesntContainString1);
            Assert.Throws<AssertionFailedException>(DoesntContainString2);
            Assert.Throws<AssertionFailedException>(DoesntContainString3);
            Assert.Throws<AssertionFailedException>(DoesntContainString4);
            Assert.Throws<AssertionFailedException>(DoesntContainString5);
        }

        private void DoesntContainString1()
        {
            Assert.Contains("d", "abc");
        }

        private void DoesntContainString2()
        {
            Assert.Contains("cd", "abc");
        }

        private void DoesntContainString3()
        {
            Assert.Contains("ac", "abc");
        }

        private void DoesntContainString4()
        {
            Assert.Contains("aa", "abc");
        }

        private void DoesntContainString5()
        {
            Assert.Contains("a", "");
        }

        [Test]
        public void FloatEqualityTests()
        {
            Assert.AreEqual(13.37f, 13.37f);
            Assert.AreEqual(13.37f, 13.36999999999f);
            Assert.Throws<AssertionFailedException>(FloatIsNotEqual);
            Assert.AreEqual(1f, 1.9f, 1f);
        }

        private void FloatIsNotEqual()
        {
            Assert.AreEqual(13.37f, 13.371f);
        }

        [Test]
        public void FloatNonEqualityTests()
        {
            Assert.AreNotEqual(42.0f, 42.9999f);
            Assert.AreNotEqual(42.0f, 43.1f, 1f);
            Assert.Throws<AssertionFailedException>(FloatIsEqual1);
            Assert.Throws<AssertionFailedException>(FloatIsEqual2);
        }

        private void FloatIsEqual1()
        {
            Assert.AreNotEqual(42.1f, 42.1f);
        }

        private void FloatIsEqual2()
        {
            Assert.AreNotEqual(42.1f, 43.0f, 1.0f);
        }

        [Test]
        public void DoubleEqualityTests()
        {
            Assert.AreEqual(13.37, 13.37);
            Assert.AreEqual(13.37, 13.36999999999);
            Assert.Throws<AssertionFailedException>(DoubleIsNotEqual);
            Assert.AreEqual(1, 1.9, 2);
        }

        private void DoubleIsNotEqual()
        {
            Assert.AreEqual(13.37, 13.371);
        }

        [Test]
        public void DoubleNonEqualityTests()
        {
            Assert.AreNotEqual(42.0, 42.9999);
            Assert.AreNotEqual(42.0, 43.1, 1);
            Assert.Throws<AssertionFailedException>(DoubleIsEqual1);
            Assert.Throws<AssertionFailedException>(DoubleIsEqual2);
        }

        private void DoubleIsEqual1()
        {
            Assert.AreNotEqual(42.1, 42.1);
        }

        private void DoubleIsEqual2()
        {
            Assert.AreNotEqual(42.1, 43.0, 1.0);
        }

        [Test]
        public void IntVectorEqualityTests()
        {
            Assert.AreEqual(int2(1,2), int2(1,2));
            Assert.Throws<AssertionFailedException>(Int2IsNotEqual);
            Assert.AreEqual(int3(1,2,3), int3(1,2,3));
            Assert.Throws<AssertionFailedException>(Int3IsNotEqual);
            Assert.AreEqual(int4(1,2,3,4), int4(1,2,3,4));
            Assert.Throws<AssertionFailedException>(Int4IsNotEqual);
        }

        private void Int2IsNotEqual()
        {
            Assert.AreEqual(int2(1,2), int2(1,3));
        }

        private void Int3IsNotEqual()
        {
            Assert.AreEqual(int3(1,2,3), int3(1,2,4));
        }

        private void Int4IsNotEqual()
        {
            Assert.AreEqual(int4(1,2,3,4), int4(1,2,3,5));
        }

        [Test]
        public void FloatVectorEqualityTests()
        {
            Assert.AreEqual(float2(1,2), float2(1,2));
            Assert.AreEqual(float2(1,2), float2(1,3), 2);
            Assert.Throws<AssertionFailedException>(Float2IsNotEqual1);
            Assert.Throws<AssertionFailedException>(Float2IsNotEqual2);

            Assert.AreEqual(float3(1,2,3), float3(1,2,3));
            Assert.AreEqual(float3(1,2,3), float3(1,2,4), 2);
            Assert.Throws<AssertionFailedException>(Float3IsNotEqual1);
            Assert.Throws<AssertionFailedException>(Float3IsNotEqual2);
            Assert.Throws<AssertionFailedException>(Float3IsNotEqual3);

            Assert.AreEqual(float4(1,2,3,4), float4(1,2,3,4));
            Assert.AreEqual(float4(1,2,3,4), float4(1,2,3,5), 2);
            Assert.Throws<AssertionFailedException>(Float4IsNotEqual1);
            Assert.Throws<AssertionFailedException>(Float4IsNotEqual2);
            Assert.Throws<AssertionFailedException>(Float4IsNotEqual3);
            Assert.Throws<AssertionFailedException>(Float4IsNotEqual4);

            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,9));
            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,10), 2);
            Assert.Throws<AssertionFailedException>(Float3x3IsNotEqual1);
            Assert.Throws<AssertionFailedException>(Float3x3IsNotEqual2);
            Assert.Throws<AssertionFailedException>(Float3x3IsNotEqual3);

            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,7), 2);
            Assert.Throws<AssertionFailedException>(Float4x4IsNotEqual1);
            Assert.Throws<AssertionFailedException>(Float4x4IsNotEqual2);
            Assert.Throws<AssertionFailedException>(Float4x4IsNotEqual3);
            Assert.Throws<AssertionFailedException>(Float4x4IsNotEqual4);
        }

        private void Float2IsNotEqual1()
        {
            Assert.AreEqual(float2(1,2), float2(1,3));
        }

        private void Float2IsNotEqual2()
        {
            Assert.AreEqual(float2(1,2), float2(2,2));
        }

        private void Float3IsNotEqual1()
        {
            Assert.AreEqual(float3(1,2,3), float3(2,2,3));
        }

        private void Float3IsNotEqual2()
        {
            Assert.AreEqual(float3(1,2,3), float3(1,3,3));
        }

        private void Float3IsNotEqual3()
        {
            Assert.AreEqual(float3(1,2,3), float3(1,2,4));
        }

        private void Float4IsNotEqual1()
        {
            Assert.AreEqual(float4(1,2,3,4), float4(0,2,3,4));
        }

        private void Float4IsNotEqual2()
        {
            Assert.AreEqual(float4(1,2,3,4), float4(1,0,3,4));
        }

        private void Float4IsNotEqual3()
        {
            Assert.AreEqual(float4(1,2,3,4), float4(1,2,0,4));
        }

        private void Float4IsNotEqual4()
        {
            Assert.AreEqual(float4(1,2,3,4), float4(1,2,3,0));
        }

        private void Float3x3IsNotEqual1()
        {
            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(0,2,3,4,5,6,7,8,9));
        }

        private void Float3x3IsNotEqual2()
        {
            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,0,6,7,8,9));
        }

        private void Float3x3IsNotEqual3()
        {
            Assert.AreEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,0));
        }

        private void Float4x4IsNotEqual1()
        {
            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(0,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6));
        }

        private void Float4x4IsNotEqual2()
        {
            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,0,7,8,9,0,1,2,3,4,5,6));
        }

        private void Float4x4IsNotEqual3()
        {
            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,9,1,2,3,4,5,6));
        }

        private void Float4x4IsNotEqual4()
        {
            Assert.AreEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,0));
        }


        [Test]
        public void IntVectorNonEqualityTests()
        {
            Assert.AreNotEqual(int2(1,2), int2(9,2));
            Assert.AreNotEqual(int2(1,2), int2(1,9));
            Assert.Throws<AssertionFailedException>(Int2IsEqual);

            Assert.AreNotEqual(int3(1,2,3), int3(9,2,3));
            Assert.AreNotEqual(int3(1,2,3), int3(2,9,3));
            Assert.AreNotEqual(int3(1,2,3), int3(1,2,9));
            Assert.Throws<AssertionFailedException>(Int3IsEqual);

            Assert.AreNotEqual(int4(1,2,3,4), int4(0,2,3,4));
            Assert.AreNotEqual(int4(1,2,3,4), int4(1,0,3,4));
            Assert.AreNotEqual(int4(1,2,3,4), int4(1,2,0,4));
            Assert.AreNotEqual(int4(1,2,3,4), int4(1,2,3,0));
            Assert.Throws<AssertionFailedException>(Int4IsEqual);
        }

        private void Int2IsEqual()
        {
            Assert.AreNotEqual(int2(1,2), int2(1,2));
        }

        private void Int3IsEqual()
        {
            Assert.AreNotEqual(int3(1,2,3), int3(1,2,3));
        }

        private void Int4IsEqual()
        {
            Assert.AreNotEqual(int4(1,2,3,4), int4(1,2,3,4));
        }

        [Test]
        public void FloatVectorNonEqualityTests()
        {
            Assert.AreNotEqual(float2(1,2), float2(1,3));
            Assert.AreNotEqual(float2(1,2), float2(2,2));
            Assert.AreNotEqual(float2(1,2), float2(1,4), 1);
            Assert.Throws<AssertionFailedException>(Float2IsEqual);

            Assert.AreNotEqual(float3(1,2,3), float3(9,2,3));
            Assert.AreNotEqual(float3(1,2,3), float3(2,9,3));
            Assert.AreNotEqual(float3(1,2,3), float3(1,2,9));
            Assert.AreNotEqual(float3(1,2,3), float3(1,2,5), 1);
            Assert.Throws<AssertionFailedException>(Float3IsEqual);

            Assert.AreNotEqual(float4(1,2,3,4), float4(0,2,3,4));
            Assert.AreNotEqual(float4(1,2,3,4), float4(1,0,3,4));
            Assert.AreNotEqual(float4(1,2,3,4), float4(1,2,0,4));
            Assert.AreNotEqual(float4(1,2,3,4), float4(1,2,3,0));
            Assert.AreNotEqual(float4(1,2,3,4), float4(1,2,3,6), 1);
            Assert.Throws<AssertionFailedException>(Float4IsEqual);

            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(0,2,3,4,5,6,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,0,3,4,5,6,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,0,4,5,6,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,0,5,6,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,0,6,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,0,7,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,0,8,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,0,9));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,0));
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,11), 1);
            Assert.Throws<AssertionFailedException>(Float3x3IsEqual);

            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(0,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,0,3,4,5,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,0,4,5,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,0,5,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,0,6,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,0,7,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,0,8,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,0,9,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,0,0,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,9,1,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,0,2,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,0,3,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,0,4,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,0,5,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,0,6));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,0));
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,8), 1);
            Assert.Throws<AssertionFailedException>(Float4x4IsEqual);
        }

        private void Float2IsEqual()
        {
            Assert.AreNotEqual(float2(1,2), float2(1,2));
        }

        private void Float3IsEqual()
        {
            Assert.AreNotEqual(float3(1,2,3), float3(1,2,3));
        }

        private void Float4IsEqual()
        {
            Assert.AreNotEqual(float4(1,2,3,4), float4(1,2,3,4));
        }

        private void Float3x3IsEqual()
        {
            Assert.AreNotEqual(float3x3(1,2,3,4,5,6,7,8,9), float3x3(1,2,3,4,5,6,7,8,9));
        }

        private void Float4x4IsEqual()
        {
            Assert.AreNotEqual(float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6), float4x4(1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6));
        }

        [Test]
        public void CollectionTests()
        {
            Assert.Throws<AssertionFailedException>(CollectionTestFails1);
            Assert.Throws<AssertionFailedException>(CollectionTestFails2);
            Assert.Throws<AssertionFailedException>(CollectionTestFails3);
            Assert.Throws<AssertionFailedException>(CollectionTestFails4);
            Assert.Throws<AssertionFailedException>(CollectionTestFails5);
            Assert.AreCollectionsEqual(new List<int>(), new List<int>());
            Assert.AreCollectionsEqual(new List<int> {1}, new List<int> {1});
            Assert.AreCollectionsEqual(new List<int> {1,2,3}, new List<int> {1,2,3});
        }

        private void CollectionTestFails1()
        {
            Assert.AreCollectionsEqual(new List<int>(), new List<int> {1});
        }

        private void CollectionTestFails2()
        {
            Assert.AreCollectionsEqual(new List<int> {1}, new List<int>());
        }

        private void CollectionTestFails3()
        {
            Assert.AreCollectionsEqual(new List<int> {1,2}, new List<int> {1,3});
        }

        private void CollectionTestFails4()
        {
            Assert.AreCollectionsEqual(new List<int> {1}, new List<int> {1,2});
        }

        private void CollectionTestFails5()
        {
            Assert.AreCollectionsEqual(new List<int> {1,2}, new List<int> {1});
        }

        [Test]
        public void ArrayTests()
        {
            Assert.Throws<AssertionFailedException>(ArrayTestFails1);
            Assert.Throws<AssertionFailedException>(ArrayTestFails2);
            Assert.Throws<AssertionFailedException>(ArrayTestFails3);
            Assert.Throws<AssertionFailedException>(ArrayTestFails4);
            Assert.Throws<AssertionFailedException>(ArrayTestFails5);
            Assert.AreEqual(new int[0], new int[0]);
            Assert.AreEqual(new int[] {1}, new int[] {1});
            Assert.AreEqual(new int[] {1,2,3}, new int[] {1,2,3});
        }

        private void ArrayTestFails1()
        {
            Assert.AreEqual(new int[0], new int[] {1});
        }

        private void ArrayTestFails2()
        {
            Assert.AreEqual(new int[] {1}, new int[0]);
        }

        private void ArrayTestFails3()
        {
            Assert.AreEqual(new int[] {1,2}, new int[] {1,3});
        }

        private void ArrayTestFails4()
        {
            Assert.AreEqual(new int[] {1}, new int[] {1,2});
        }

        private void ArrayTestFails5()
        {
            Assert.AreEqual(new int[] {1,2}, new int[] {1});
        }

        [Test]
        public void FailTest()
        {
            Assert.Throws<AssertionFailedException>(Fails);
        }

        private void Fails()
        {
            Assert.Fail("manual fail message");
        }
    }

    public class SomeClass {}

    public class ClassWithoutEquals
    {
        int i = 42;
    }

    public class ClassWithEquals
    {
        int i = 42;

        public override bool Equals(object o)
        {
            return (o as ClassWithEquals).i == i; //Good enough
        }
    }

    public class CustomException : Exception
    {
        public CustomException(string message) : base (message)
        { }
    }

    public class OtherException : Exception
    {
    }
}
