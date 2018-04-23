using System;
using System.Diagnostics;

namespace Uno.TestRunner.BasicTypes
{
    [DebuggerDisplay("{Name} {_started!=null}")]
    public class Test
    {
        public enum TestStatus
        {
            NotRun,
            Passed,
            Ignored,
            Failed
        };

        public string Name { get; }
        public int Microseconds { get; private set; }
        public TestStatus Status { get; private set; }
        public bool Ended => Status != TestStatus.NotRun;
        public bool Failed => Status == TestStatus.Failed;
        public bool HasTimedOut { get; private set; }
        public Assertion Assertion { get; private set; }
        public string Exception { get; private set; }
        public string IgnoreReason { get; private set; }
        private DateTime? _started;

        public Test(string name)
        {
            Name = name;
        }

        public void Asserted(Assertion assertion)
        {
            Status = TestStatus.Failed;
            Assertion = assertion;
        }

        public void Threw(string exception)
        {
            Status = TestStatus.Failed;
            Exception = exception;
        }

        public void TimedOut(double seconds)
        {
            Status = TestStatus.Failed;
            Exception = string.Format("Test timed out after {0} seconds", seconds);
            HasTimedOut = true;
        }

        public void Passed(int microseconds = -1)
        {
            Microseconds = microseconds;
            Status = TestStatus.Passed;
        }

        public void Started()
        {
            _started = DateTime.Now;
        }

        public void Ignored(string reason)
        {
            Status = TestStatus.Ignored;
            IgnoreReason = reason;
        }

        public TimeSpan Duration
        {
            get
            {
                if (_started == null)
                {
                    return new TimeSpan(0);
                }
                return DateTime.Now - _started.Value;
            }
        }
    }
}
