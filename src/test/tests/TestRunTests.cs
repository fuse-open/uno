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

            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.WaitingForReady));
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeTestPassedEvent("foo")));
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeTestAssertedEvent("foo")));

            run.EventOccured(MakeReadyEvent(1));
            Assert.Throws<InvalidOperationException>(() => run.EventOccured(MakeReadyEvent(1)));
        }

        [Test]
        public void StartupSequenceIsCorrect()
        {
            var run = new TestRun(new Mock<ITestResultLogger>().Object);
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.NotStarted));
            run.Start();
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.WaitingForReady));
            run.EventOccured(MakeReadyEvent(1));
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.Ready));
            run.EventOccured(MakeTestStartedEvent("foo"));
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.Running));
            run.EventOccured(MakeTestPassedEvent("foo"));
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.Finished));
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
            Assert.That(run.CurrentState, Is.EqualTo(TestRun.State.Finished));

            var result = run.WaitUntilFinished();
            Assert.That(result.Select(test => test.Name), Is.EqualTo(new string[]{ "foo", "bar" }));
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
