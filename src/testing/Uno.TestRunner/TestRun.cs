using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Uno.TestRunner.BasicTypes;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public class TestRun : IDisposable
    {
        private readonly object _lock = new object();
        readonly ITestResultLogger _logger;
        private readonly List<Test> _tests = new List<Test>();
        private int _testCount = -1;
        private State _currentState = State.NotStarted;
        private readonly ManualResetEvent _isFinished = new ManualResetEvent(false);
        private readonly List<Exception> _errors = new List<Exception>();

        public TestRun(ITestResultLogger logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            if (CurrentState != State.NotStarted)
                throw new InvalidOperationException("Unexpected 'ready'-event");

            _currentState = State.WaitingForReady;
        }

        public State CurrentState => _currentState;

        private Test CurrentTest
        {
            get
            {
                if (CurrentState < State.Running)
                    return null;

                return _tests.Last();
            }
        }

        public void Update()
        {
            lock (_lock)
            {
                CheckForErrors();
            }
        }

        public List<Test> WaitUntilFinished()
        {
            while (!_isFinished.WaitOne())
                Update();

            return _tests;
        }

        private void CheckForErrors()
        {
            if (_errors.Count > 0)
                throw new AggregateException(_errors);
        }

        public void EventOccured(NameValueCollection eventDetails)
        {
            lock (_lock)
            {
                var eventType = eventDetails.Get("event");
                if (eventType == "ready")
                {
                    if (CurrentState != State.WaitingForReady)
                        throw new InvalidOperationException("Unexpected 'ready'-event");

                    _testCount = int.Parse(eventDetails.Get("testCount"));

                    _currentState = State.Ready;
                    return;
                }

                if (CurrentState < State.Ready)
                    throw new InvalidOperationException(string.Format("Unexpected '{0}'-event", eventType));

                var testName = eventDetails.Get("testName");
                var us = eventDetails["us"] != null ? int.Parse(eventDetails["us"]) : -1;

                switch (eventType)
                {
                    case "testStarted":
                        if (CurrentState != State.Ready)
                            throw new InvalidOperationException("Unexpected 'testStarted'-event");

                        var test = new Test(testName);
                        _tests.Add(test);
                        _currentState = State.Running;

                        CurrentTest.Started();
                        break;

                    case "testPassed":
                        if (CurrentState != State.Running)
                            throw new InvalidOperationException("Unexpected 'testPassed'-event");

                        if (!CurrentTest.Name.Equals(testName))
                            throw new InvalidOperationException("Event for wrong test");

                        CurrentTest.Passed(us);
                        _logger.TestPassed(CurrentTest);

                        _currentState = State.Ready;
                        break;

                    case "testIgnored":
                        if (CurrentState != State.Running)
                            throw new InvalidOperationException("Unexpected 'testIgnored'-event");

                        if (!CurrentTest.Name.Equals(testName))
                            throw new InvalidOperationException("Event for wrong test");

                        CurrentTest.Ignored(eventDetails.Get("reason"));
                        _logger.TestIgnored(CurrentTest);

                        _currentState = State.Ready;
                        break;

                    case "testAsserted":
                        if (CurrentState != State.Running)
                            throw new InvalidOperationException("Unexpected 'testAsserted'-event");

                        if (!CurrentTest.Name.Equals(testName))
                            throw new InvalidOperationException("Event for wrong test");

                        CurrentTest.Asserted(Assertion.From(eventDetails));
                        _logger.TestAsserted(CurrentTest);

                        _currentState = State.Ready;
                        break;

                    case "testThrew":
                        if (CurrentState != State.Running)
                            throw new InvalidOperationException("Unexpected 'testThrew'-event");

                        if (!CurrentTest.Name.Equals(testName))
                            throw new InvalidOperationException("Event for wrong test");

                        CurrentTest.Threw(eventDetails.Get("message"));
                        _logger.TestThrew(CurrentTest);

                        _currentState = State.Ready;
                        break;

                    case "internalError":
                        _logger.InternalError(eventDetails.Get("message"));
                        break;

                    default:
                        throw new Exception(string.Format("Internal Error: Unknown event '{0}' (name={1})", eventType, testName));
                }

                if (_tests.Count == _testCount && _currentState == State.Ready)
                    End();
            }
        }

        private void End()
        {
            _currentState = State.Finished;
            _isFinished.Set();
        }

        public void ErrorOccured(Exception e)
        {
            lock (_lock)
            {
                _logger.InternalError(e.Message);
                _errors.Add(e);
            }
        }

        public enum State
        {
            NotStarted,
            WaitingForReady,
            Ready,
            Running,
            Finished,
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _isFinished.Dispose();
        }
    }
}
