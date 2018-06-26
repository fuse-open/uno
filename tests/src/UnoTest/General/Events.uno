using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Events
    {
        public Events()
        {
            _testValue1 = 0;
            _testValue2 = 0;
            _staticTestValue1 = 0;
            StaticAction1 = null;
            StaticAction2 = null;
        }

        event Action Action1;
        event Action Action2;
        event TestDelegate DelegateAction;

        static event Action StaticAction1;
        static event Action StaticAction2;
        static event TestDelegate StaticDelegateAction;

        delegate void TestDelegate();

        int _testValue1;
        void TestMethod1()
        {
            _testValue1++;
        }

        int _testValue2;
        void TestMethod2()
        {
            _testValue2++;
        }

        static int _staticTestValue1;
        static void StaticTestMethod1()
        {
            _staticTestValue1++;
        }

        [Test]
        public void OneMethodCalledTest()
        {
            Action1 += TestMethod1;
            Action1();

            Assert.AreEqual(1, _testValue1);

            Action1 -= TestMethod1;

            Assert.Throws<NullReferenceException>(Action1);
            Assert.AreEqual(1, _testValue1);
        }

        [Test]
        public void FewMethodsCalledTest()
        {
            Action1 += TestMethod1;
            Action1 += StaticTestMethod1;
            Action1();

            Assert.AreEqual(1, _testValue1);
            Assert.AreEqual(1, _staticTestValue1);

            Action1 += TestMethod2;
            Action1();

            Assert.AreEqual(2, _testValue1);
            Assert.AreEqual(1, _testValue2);
            Assert.AreEqual(2, _staticTestValue1);
        }

        [Test]
        public void SameMethodSeveralTimesTest()
        {
            TestDelegate testDelegate = TestMethod2;
            DelegateAction += testDelegate;
            DelegateAction += testDelegate;
            DelegateAction();

            Assert.AreEqual(2, _testValue2);
        }

        [Test]
        public void SameMethodSeveralTimesWithDeletedTest()
        {
            TestDelegate testDelegate = TestMethod1;
            StaticDelegateAction += testDelegate;
            StaticDelegateAction += testDelegate;
            StaticDelegateAction += testDelegate;
            StaticDelegateAction -= testDelegate;
            StaticDelegateAction();

            Assert.AreEqual(2, _testValue1);
        }

        [Test]
        public void AssignClearEventTest()
        {
            TestDelegate testDelegate = TestMethod1;
            DelegateAction += testDelegate;
            DelegateAction += StaticTestMethod1;
            DelegateAction = TestMethod2;
            DelegateAction();

            Assert.AreEqual(0, _testValue1);
            Assert.AreEqual(1, _testValue2);
            Assert.AreEqual(0, _staticTestValue1);
        }

        [Test]
        public void CompareTest()
        {
            Action1 += TestMethod1;
            Action2 += TestMethod1;
            Action1 += TestMethod2;
            Action2 += TestMethod2;

            Assert.IsTrue(Action1 == Action2);
        }

        [Test]
        public void CompareOrderTest()
        {
            StaticAction1 += TestMethod2;
            StaticAction1 += TestMethod1;
            StaticAction2 += TestMethod1;
            StaticAction2 += TestMethod2;

            Assert.IsFalse(StaticAction1 == StaticAction2);
        }

        [Test]
        public void CombineDelegatesTest()
        {
            TestDelegate testDelegate1 = TestMethod1;
            TestDelegate testDelegate2 = TestMethod2;
            testDelegate1 = (TestDelegate)Delegate.Combine((Delegate)testDelegate1, (Delegate)testDelegate2);
            DelegateAction += testDelegate1;
            DelegateAction();

            Assert.AreEqual(1, _testValue1);
            Assert.AreEqual(1, _testValue2);
        }

        [Test]
        public void CombineEventsTest()
        {
            Action1 += TestMethod1;
            Action1 += StaticTestMethod1;
            Action2 += Action1;
            Action2 += TestMethod1;
            Action2();

            Assert.AreEqual(2, _testValue1);
            Assert.AreEqual(0, _testValue2);
            Assert.AreEqual(1, _staticTestValue1);
        }

        [Test]
        public void DetachingEventsTest()
        {
            Action1 += TestMethod1;
            Action1 += StaticTestMethod1;
            Action2 += TestMethod1;
            Action2 += TestMethod2;
            Action2 -= Action1;
            Action2();

            Assert.AreEqual(1, _testValue1);
            Assert.AreEqual(1, _testValue2);
            Assert.AreEqual(0, _staticTestValue1);
        }
    }
}
