using System.Diagnostics;

namespace Uno.TestRunner.BasicTypes
{
    [DebuggerDisplay("{Name} {Status > TestStatus.NotRun}")]
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
        public TestStatus Status { get; private set; }
        public bool Ended => Status > TestStatus.Started;
        public bool Failed => Status == TestStatus.Failed;
        public Assertion Assertion { get; private set; }
        public string Exception { get; private set; }
        public string IgnoreReason { get; private set; }

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

        public void Passed(int microseconds = -1)
        {
            Microseconds = microseconds;
            Status = TestStatus.Passed;
        }

        public void Started()
        {
            Status = TestStatus.Started;
        }

        public void Ignored(string reason)
        {
            Status = TestStatus.Ignored;
            IgnoreReason = reason;
        }
    }
}
