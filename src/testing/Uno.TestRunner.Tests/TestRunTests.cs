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
        [Test]
        public void ThrowsOnIncorrectState()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object);
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
            var run = new TestRun(new Mock<ITestResultLogger>().Object);
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
            var run = new TestRun(new Mock<ITestResultLogger>().Object);
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
