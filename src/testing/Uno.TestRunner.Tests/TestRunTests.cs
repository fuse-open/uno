using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner.Tests
{
    class TestRunTests
    {
        private readonly TimeSpan _arbitraryLongTimeout = TimeSpan.FromSeconds(10);
        private TimeSpan _veryShortTimeout = TimeSpan.FromTicks(1);

        [Test]
        public void ThrowsOnIncorrectState()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _arbitraryLongTimeout, _arbitraryLongTimeout);
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeReadyEvent(1)));
            run.Start();

            Assert.AreEqual(TestRun.State.WaitingForReady, run.CurrentState);
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeTestPassedEvent("foo")));
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeTestAssertedEvent("foo")));

            run.EventOccured(MakeReadyEvent(1));
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeReadyEvent(1)));
        }

        [Test]
        public void StartupSequenceIsCorrect()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _arbitraryLongTimeout, _arbitraryLongTimeout);
            Assert.AreEqual(TestRun.State.NotStarted, run.CurrentState);
            run.Start();
            Assert.AreEqual(TestRun.State.WaitingForReady, run.CurrentState);
            run.EventOccured(MakeReadyEvent(1));
            Assert.AreEqual(TestRun.State.Ready, run.CurrentState);
            run.EventOccured(MakeTestStartedEvent("foo"));
            Assert.AreEqual(TestRun.State.Running, run.CurrentState);
            run.EventOccured(MakeTestPassedEvent("foo"));
            Assert.AreEqual(TestRun.State.Finished, run.CurrentState);
        }

        [Test]
        public void RunsAllTestsInCorrectOrder()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _arbitraryLongTimeout, _arbitraryLongTimeout);
            run.Start();
            run.EventOccured(MakeReadyEvent(2));
            run.EventOccured(MakeTestStartedEvent("foo"));
            run.EventOccured(MakeTestPassedEvent("foo"));
            run.EventOccured(MakeTestStartedEvent("bar"));
            run.EventOccured(MakeTestAssertedEvent("bar"));
            Assert.AreEqual(TestRun.State.Finished, run.CurrentState);

            var result = run.WaitUntilFinished();
            Assert.AreEqual(new string[]{ "foo", "bar" }, result.Select(test => test.Name));
        }

        [Test]
        public void CancelsTheRestWhenATestTimesOut()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _veryShortTimeout, _arbitraryLongTimeout);
            run.Start();
            run.EventOccured(MakeReadyEvent(3));
            run.EventOccured(MakeTestStartedEvent("foo"));
            Thread.Sleep(2);
            var tests = run.WaitUntilFinished();
            Assert.IsTrue(tests[0].HasTimedOut);
            Assert.IsFalse(tests[1].HasTimedOut);
            Assert.IsFalse(tests[2].HasTimedOut);
            Assert.IsTrue(tests.TrueForAll(t=>t.Failed));
        }

        [Test]
        public void StoredErrorsAreThrownFromWaitUntilFinished()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _veryShortTimeout, _arbitraryLongTimeout);
            run.ErrorOccured(new Exception("foo"));
            var e = Assert.Throws<AggregateException>(() => run.WaitUntilFinished());
            Assert.AreEqual(1, e.InnerExceptions.Count);
            Assert.AreEqual("foo", e.InnerExceptions[0].Message);
        }

        [Test]
        public void ThrowsIfItNeverHearsFromUnoExe()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object, _arbitraryLongTimeout, TimeSpan.FromTicks(1));
            run.Start();
            Assert.Throws<Exception>(() => run.WaitUntilFinished());
        }

        private static NameValueCollection MakeReadyEvent(int testCount)
        {
            return new NameValueCollection{{"event","ready"}, {"testCount", testCount.ToString()}};
        }

        private static NameValueCollection MakeTestStartedEvent(string name)
        {
            return new NameValueCollection {{"event", "testStarted"}, {"testName", name}};
        }

        private static NameValueCollection MakeTestPassedEvent(string name)
        {
            return new NameValueCollection{{"event","testPassed"}, {"testName", name}};
        }

        private static NameValueCollection MakeTestAssertedEvent(string name)
        {
            return new NameValueCollection{{"event","testAsserted"}, {"testName", name}};
        }
    }
}
