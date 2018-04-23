using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    interface DelegatesTestInterface
    {
        void interfaceMethod1();
        void interfaceMethod2();
    }

    struct DelegatesTestStruct
    {
        int foo;

        public int GetFoo()
        {
            return foo;
        }
    }

    public class Delegates
        : DelegatesTestInterface
    {
        ////////////////////////////////////////////////////////////////////////
        // Invocation count tests
        ////////////////////////////////////////////////////////////////////////

        private static ulong instanceMethod1Counter_ = 0;
        private static ulong instanceMethod2Counter_ = 0;
        private static ulong classMethod1Counter_ = 0;
        private static ulong classMethod2Counter_ = 0;
        private static ulong interfaceMethod1Counter_ = 0;
        private static ulong interfaceMethod2Counter_ = 0;

        private void instanceMethod1() { ++instanceMethod1Counter_; }
        private void instanceMethod2() { ++instanceMethod2Counter_; }

        private static void classMethod1() { ++classMethod1Counter_; }
        private static void classMethod2() { ++classMethod2Counter_; }

        public void interfaceMethod1() { ++interfaceMethod1Counter_; }
        public void interfaceMethod2() { ++interfaceMethod2Counter_; }

        delegate void Delegate();

        private void checkDelegate(Delegate testDelegate,
            ulong instanceMethod1Calls, ulong instanceMethod2Calls,
            ulong classMethod1Calls, ulong classMethod2Calls,
            ulong interfaceMethod1Calls = 0, ulong interfaceMethod2Calls = 0)
        {
            ulong beforeInstanceMethod1Count = instanceMethod1Counter_;
            ulong beforeInstanceMethod2Count = instanceMethod2Counter_;
            ulong beforeClassMethod1Count = classMethod1Counter_;
            ulong beforeClassMethod2Count = classMethod2Counter_;
            ulong beforeInterfaceMethod1Count = interfaceMethod1Counter_;
            ulong beforeInterfaceMethod2Count = interfaceMethod2Counter_;

            if (testDelegate != null)
                testDelegate();

            ulong afterInstanceMethod1Count = instanceMethod1Counter_;
            ulong afterInstanceMethod2Count = instanceMethod2Counter_;
            ulong afterClassMethod1Count = classMethod1Counter_;
            ulong afterClassMethod2Count = classMethod2Counter_;
            ulong afterInterfaceMethod1Count = interfaceMethod1Counter_;
            ulong afterInterfaceMethod2Count = interfaceMethod2Counter_;

            Assert.AreEqual(instanceMethod1Calls,
                afterInstanceMethod1Count - beforeInstanceMethod1Count);
            Assert.AreEqual(instanceMethod2Calls,
                afterInstanceMethod2Count - beforeInstanceMethod2Count);

            Assert.AreEqual(classMethod1Calls,
                afterClassMethod1Count - beforeClassMethod1Count);
            Assert.AreEqual(classMethod2Calls,
                afterClassMethod2Count - beforeClassMethod2Count);

            Assert.AreEqual(interfaceMethod1Calls,
                afterInterfaceMethod1Count - beforeInterfaceMethod1Count);
            Assert.AreEqual(interfaceMethod2Calls,
                afterInterfaceMethod2Count - beforeInterfaceMethod2Count);
        }

        [Test]
        public void SelfTest()
        {
            checkDelegate(null, 0, 0, 0, 0);
            checkDelegate(instanceMethod1, 1, 0, 0, 0);
            checkDelegate(instanceMethod2, 0, 1, 0, 0);
            checkDelegate(classMethod1, 0, 0, 1, 0);
            checkDelegate(classMethod2, 0, 0, 0, 1);
        }

        [Test]
        public void DelegateIsAssignable()
        {
            Delegate testDelegate;

            testDelegate = instanceMethod1;
            checkDelegate(testDelegate, 1, 0, 0, 0);

            testDelegate = classMethod1;
            checkDelegate(testDelegate, 0, 0, 1, 0);

            testDelegate = instanceMethod2;
            checkDelegate(testDelegate, 0, 1, 0, 0);
        }

        [Test]
        public void DelegateIsComposable()
        {
            Delegate testDelegate = classMethod1;

            testDelegate += instanceMethod1;
            checkDelegate(testDelegate, 1, 0, 1, 0);
        }

        [Test]
        public void DelegateCanComposeWithNull()
        {
            Delegate testDelegate = classMethod1;

            testDelegate += null;
            checkDelegate(testDelegate, 0, 0, 1, 0);
        }

        [Test]
        public void ComposingDelegatesKeepsOriginalsIntact()
        {
            Delegate testDelegate1;
            Delegate testDelegate2;

            testDelegate1 = instanceMethod1;
            testDelegate2 = instanceMethod2;

            Delegate dummyDelegate = testDelegate1 + testDelegate2;
            dummyDelegate();

            checkDelegate(testDelegate1, 1, 0, 0, 0);
            checkDelegate(testDelegate2, 0, 1, 0, 0);
        }

        [Test]
        public void DelegateCanComposeWithSelf()
        {
            Delegate testDelegate = instanceMethod1;
            checkDelegate(testDelegate + testDelegate, 2, 0, 0, 0);
        }

        [Test]
        public void CanRemoveNullFromDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate -= null;
            checkDelegate(testDelegate, 1, 0, 0, 0);
        }

        [Test]
        public void CanRemoveDelegateFromNull()
        {
            Delegate testDelegate = null;
            testDelegate -= instanceMethod1;
            checkDelegate(testDelegate, 0, 0, 0, 0);
        }

        [Test]
        public void DelegateCanBeRemovedFromSelf()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate -= testDelegate;
            checkDelegate(testDelegate, 0, 0, 0, 0);
        }

        [Test]
        public void NotSubscribedFunctionCanBeRemovedFromDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate -= classMethod1;
            checkDelegate(testDelegate, 1, 0, 0, 0);
        }

        [Test]
        public void CanUnsubscribeNullFromMultiCastDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate += instanceMethod2;

            testDelegate -= null;
            checkDelegate(testDelegate, 1, 1, 0, 0);
        }

        [Test]
        public void CanUnsubscribeFirstDelegateFromMultiCastDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate += instanceMethod2;
            testDelegate += classMethod1;

            testDelegate -= instanceMethod1;
            checkDelegate(testDelegate, 0, 1, 1, 0);
        }

        [Test]
        public void CanUnsubscribeMiddleDelegateFromMultiCastDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate += instanceMethod2;
            testDelegate += classMethod1;

            testDelegate -= instanceMethod2;
            checkDelegate(testDelegate, 1, 0, 1, 0);
        }

        [Test]
        public void CanUnsubscribeLastDelegateFromMultiCastDelegate()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate += instanceMethod2;
            testDelegate += classMethod1;

            testDelegate -= classMethod1;
            checkDelegate(testDelegate, 1, 1, 0, 0);
        }

        [Test]
        public void RemovingDelegatesKeepsOriginalIntact()
        {
            Delegate testDelegate1 = instanceMethod1;
            testDelegate1 += instanceMethod2;
            testDelegate1 += classMethod1;

            Delegate testDelegate2 = instanceMethod2;

            Delegate dummyDelegate = testDelegate1 - testDelegate2;
            dummyDelegate();

            checkDelegate(testDelegate1, 1, 1, 1, 0);
            checkDelegate(testDelegate2, 0, 1, 0, 0);
        }

        [Test]
        public void MultiCastDelegateCanRemoveFromSelf()
        {
            Delegate testDelegate1 = instanceMethod1;
            testDelegate1 += instanceMethod2;
            testDelegate1 += classMethod1;

            checkDelegate(testDelegate1 - testDelegate1, 0, 0, 0, 0);
        }

        [Test]
        public void PartialListMatchesInMultiCastDelegate()
        {
            Delegate testDelegate1 = instanceMethod1;
            testDelegate1 += instanceMethod2;
            testDelegate1 += classMethod1;

            Delegate testDelegate2 = instanceMethod1;
            testDelegate2 += instanceMethod2;

            checkDelegate(testDelegate1 - testDelegate2, 0, 0, 1, 0);
        }

        [Test]
        public void OutOfOrderInvocationListsDoNotMatch()
        {
            Delegate testDelegate1 = instanceMethod1;
            testDelegate1 += instanceMethod2;
            testDelegate1 += classMethod1;

            Delegate testDelegate2 = instanceMethod1;
            testDelegate2 += classMethod1;
            testDelegate2 += instanceMethod2;

            checkDelegate(testDelegate1 - testDelegate2, 1, 1, 1, 0);
        }

        private DelegatesTestInterface GetInterface()
        {
            return this as DelegatesTestInterface;
        }

        [Test]
        public void InterfaceMethodsConsideredDistinctInDelegateOperations()
        {
            Delegate testDelegate1 = GetInterface().interfaceMethod1;
            Delegate testDelegate2 = GetInterface().interfaceMethod2;

            checkDelegate(testDelegate1, 0, 0, 0, 0, 1, 0);
            checkDelegate(testDelegate2, 0, 0, 0, 0, 0, 1);
            checkDelegate(testDelegate1 - testDelegate2, 0, 0, 0, 0, 1, 0);
        }

        [Test]
        public void MultiCastDelegatesCanCompose()
        {
            Delegate testDelegate1 = instanceMethod1;
            testDelegate1 += classMethod1;
            testDelegate1 += classMethod2;

            Delegate testDelegate2 = instanceMethod2;
            testDelegate2 += classMethod2;

            checkDelegate(testDelegate1 + testDelegate2, 1, 1, 1, 2);
        }

        [Test]
        public void MultiCastDelegateCanComposeWithSelf()
        {
            Delegate testDelegate = instanceMethod1;
            testDelegate += instanceMethod2;
            testDelegate += classMethod2;

            checkDelegate(testDelegate + testDelegate, 2, 2, 0, 2);
        }

        ////////////////////////////////////////////////////////////////////////
        // Invocation order tests
        ////////////////////////////////////////////////////////////////////////

        delegate void LongModifier(ref ulong value);

        private void SubtractOne(ref ulong value) { value -= 1; }
        private void MultiplyByThree(ref ulong value) { value *= 3; }
        private void SquareUp(ref ulong value) { value *= value; }

        [Test]
        public void DelegatesAreCalledInAddedOrder()
        {
            LongModifier testDelegate = SubtractOne;
            testDelegate += MultiplyByThree;
            testDelegate += SquareUp;

            ulong value = 8;

            testDelegate(ref value);
            Assert.AreEqual(Math.Pow((8 - 1) * 3, 2), value);
        }

        [Test]
        public void AddThenRemoveMultiCastDelegateDoesNotAffectCallOrder()
        {
            LongModifier multiDelegate = SubtractOne;
            multiDelegate += MultiplyByThree;
            multiDelegate += SquareUp;

            LongModifier testDelegate = multiDelegate;
            testDelegate += multiDelegate;
            testDelegate -= multiDelegate;

            ulong value = 8;

            testDelegate(ref value);
            Assert.AreEqual(Math.Pow((8 - 1) * 3, 2), value);
        }

        [Test]
        public void RemovingMultiCastDelegateRemovesFullSequentialMatch()
        {
            LongModifier multiDelegate = SubtractOne;
            multiDelegate += MultiplyByThree;
            multiDelegate += SquareUp;

            LongModifier testDelegate = multiDelegate;
            testDelegate += MultiplyByThree;
            testDelegate += SubtractOne;

            ulong value = 8;

            (testDelegate - multiDelegate)(ref value);
            Assert.AreEqual(8 * 3 - 1, value);
        }

        [Test]
        public void RemovingMultiCastDelegateRemovesLastMatch()
        {
            LongModifier multiDelegate = SubtractOne;
            multiDelegate += MultiplyByThree;
            multiDelegate += SquareUp;

            LongModifier testDelegate = multiDelegate;
            testDelegate += MultiplyByThree;
            testDelegate += multiDelegate;

            ulong value = 8;

            (testDelegate - multiDelegate)(ref value);
            Assert.AreEqual(Math.Pow((8 - 1) * 3, 2) * 3, value);
        }

        ////////////////////////////////////////////////////////////////////////
        // Delegate equality
        ////////////////////////////////////////////////////////////////////////

        private sealed class NotEqualityComparable
        {
            public void instanceMethod() { }

            public override bool Equals(object obj)
            {
                throw new InvalidOperationException();
            }

            public override int GetHashCode() { return base.GetHashCode(); }
        }

        delegate String ToStringDelegate();

        [Test]
        public void DelegatesToSameInstanceAndFunctionAreEqual()
        {
            Delegate testDelegate1 = instanceMethod1;
            Delegate testDelegate2 = instanceMethod1;
            Assert.AreEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToSameInstanceAndDifferentFunctionsAreNotEqual()
        {
            Delegate testDelegate1 = instanceMethod1;
            Delegate testDelegate2 = classMethod1;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToInterfaceMethodFromSingleInstanceAreEqual()
        {
            Delegate testDelegate1 = GetInterface().interfaceMethod1;
            Delegate testDelegate2 = GetInterface().interfaceMethod1;
            Assert.AreEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToDifferentInterfaceMethodsFromSingleInstanceAreNotEqual()
        {
            Delegate testDelegate1 = GetInterface().interfaceMethod1;
            Delegate testDelegate2 = GetInterface().interfaceMethod2;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToIndependentlyBoxedValueAreNotEqual()
        {
            ToStringDelegate testDelegate1 = 64.ToString;
            ToStringDelegate testDelegate2 = 64.ToString;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToIndependentlyAndExplicitlyBoxedValueAreNotEqual()
        {
            object o1 = 64;
            object o2 = 64;
            ToStringDelegate testDelegate1 = o1.ToString;
            ToStringDelegate testDelegate2 = o2.ToString;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToIndependentlyBoxedVariableAreNotEqual()
        {
            int unboxed = 64;
            ToStringDelegate testDelegate1 = unboxed.ToString;
            ToStringDelegate testDelegate2 = unboxed.ToString;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void ObjectEqualsIsNotCalledWhenComparingDelegates()
        {
            Delegate testDelegate1 = (new NotEqualityComparable()).instanceMethod;
            Delegate testDelegate2 = (new NotEqualityComparable()).instanceMethod;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToValueMethodsAreNotEqual()
        {
            Func<int> testDelegate1 = new DelegatesTestStruct().GetFoo;
            Func<int> testDelegate2 = new DelegatesTestStruct().GetFoo;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }

        [Test]
        public void DelegatesToGenericValueMethodsAreNotEqual()
        {
            DelegatesToGenericValueMethodsAreNotEqualInternal(64);
            DelegatesToGenericValueMethodsAreNotEqualInternal(new DelegatesTestStruct());
        }

        public void DelegatesToGenericValueMethodsAreNotEqualInternal<T>(T value)
        {
            ToStringDelegate testDelegate1 = value.ToString;
            ToStringDelegate testDelegate2 = value.ToString;
            Assert.AreNotEqual(testDelegate1, testDelegate2);
        }
    }
}
