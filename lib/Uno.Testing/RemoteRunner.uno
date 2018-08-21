using Uno;
using Uno.Diagnostics;
using Uno.Net.Http;
using Uno.Testing.Assert;

namespace Uno.Testing
{
    class RemoteRunner : AbstractRunner
    {
        ITestRunnerMessageDispatcher _dispatcher;

        private readonly string _prefix;
        private string _currentTest;
        private double _startTime;

        internal RemoteRunner(Registry registry, string prefix, ITestRunnerMessageDispatcher dispatcher) : base(registry)
        {
            _prefix = prefix;
            _dispatcher = dispatcher;
        }

        public RemoteRunner(Registry registry, string prefix) : this(registry, prefix, new HttpMessageDispatcher())
        {
        }

        private override void Start()
        {
            _dispatcher.Start();
            var query = _prefix + "?event=ready"
            + "&testCount=" + TestCount;
            Get(query);
        }

        private override void Stop()
        {
            _dispatcher.Stop();
        }

        private override void TestStarting(string name)
        {
            _currentTest = name;
            var query = _prefix + "?event=testStarted"
            + "&testName=" + Uri.EscapeDataString(_currentTest);
            Get(query);
            _startTime = Clock.GetSeconds();
        }

        private override void TestPassed()
        {
            int us = (int)(1000000.0 * (Clock.GetSeconds() - _startTime));
            string q = _prefix + "?event=testPassed"
            + "&testName=" + Uri.EscapeDataString(_currentTest)
            + "&us=" + us;
            Get(q);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void TestIgnored(string reason)
        {
            string q = _prefix + "?event=testIgnored"
            + "&testName=" + Uri.EscapeDataString(_currentTest)
            + "&reason=" + Uri.EscapeDataString(reason);
            Get(q);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void AssertionFailed(AssertionFailedException e)
        {
            string q = _prefix + "?event=testAsserted"
            + "&testName=" + Uri.EscapeDataString(_currentTest)
            + "&filename=" + Uri.EscapeDataString(e.FileName)
            + "&line=" + e.Line
            + "&membername=" + Uri.EscapeDataString(e.MemberName)
            + "&expected=" + Uri.EscapeDataString(e.Expected.ToString())
            + "&actual=" + Uri.EscapeDataString(e.Actual.ToString());
            Get(q);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void ExceptionThrown(Exception e)
        {
            string q = _prefix + "?event=testThrew"
            + "&testName=" + Uri.EscapeDataString(_currentTest)
            + "&message=" + Uri.EscapeDataString(e.ToString());
            Get(q);
            _currentTest = null;
            SheduleNextTest();
        }

        static int sequenceId = 0;
        private void Get(string query)
        {
            query += "&sequenceId=" + sequenceId++;
            _dispatcher.Get(query);
        }
    }
}
