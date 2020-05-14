using System.Collections.Generic;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public abstract class AbstractLogger : ITestResultLogger
    {
        protected string ProjectName = "";
        protected string TargetName = "";
        private bool _trace;

        public abstract void TestPassed(Test test);
        public abstract void TestAsserted(Test test);
        public abstract void TestThrew(Test test);
        public abstract void TestIgnored(Test test);
        public abstract void InternalError(string message);

        public AbstractLogger(bool trace = false)
        {
            _trace = trace;
        }

        public abstract void ProjectEnded(List<Test> tests);
        public virtual void ProjectStarting(string projectName, string targetName)
        {
            ProjectName = projectName;
            TargetName = targetName;
        }

        public abstract void Log(string message);
        public abstract void Logw(string message);

        public void Log(string message, params object[] formatItems)
        {
            Log(string.Format(message, formatItems));
        }

        public void Logw(string message, params object[] formatItems)
        {
            Logw(string.Format(message, formatItems));
        }

        public void Trace(string message)
        {
            if (_trace)
            {
               Log(message);
            }
        }

        public void Trace(string message, params object[] formatItems)
        {
            if (_trace)
            {
                Trace(string.Format(message, formatItems));
            }
        }
    }
}
