using System.Diagnostics;

namespace Uno.TestRunner.BasicTypes
{
    [DebuggerDisplay("{Name} {_status > TestStatus.NotRun}")]
    public class Test
    {
        public enum TestStatus
        {
            NotRun,
            Started,
            Passed,
            Ignored,
            Failed
        }

        public string Name { get; }
        public int Microseconds { get; private set; }
        public bool Ended => _status > TestStatus.Started;
        public bool Failed => _status == TestStatus.Failed;
        public bool WasIgnored => _status == TestStatus.Ignored;
        public Assertion Assertion { get; private set; }
        public string Exception { get; private set; }
        public string IgnoreReason { get; private set; }

        TestStatus _status;

        public Test(string name)
        {
            Name = name;
        }

        public void Asserted(Assertion assertion)
        {
            _status = TestStatus.Failed;
            Assertion = assertion;
        }

        public void Threw(string exception)
        {
            _status = TestStatus.Failed;
            Exception = exception;
        }

        public void Passed(int microseconds = -1)
        {
            Microseconds = microseconds;
            _status = TestStatus.Passed;
        }

        public void Started()
        {
            _status = TestStatus.Started;
        }

        public void Ignored(string reason)
        {
            _status = TestStatus.Ignored;
            IgnoreReason = reason;
        }
    }
}
