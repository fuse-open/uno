using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public class ConsoleLogger : AbstractLogger
    {
        private int _microseconds;
        private readonly Stopwatch _watch = new Stopwatch();
        private readonly IWriter _writer;

        public ConsoleLogger(IWriter writer, bool trace=false) : base(trace)
        {
            _writer = writer;
        }

        public override void TestPassed(Test test)
        {
            ColorHelper.SetForeground(ConsoleColor.Green);
            if (test.Microseconds != -1)
            {
                Write("OK:      {0} ({1:#,0} μs)", test.Name, test.Microseconds);
                _microseconds += test.Microseconds;
            }
            else
                Write("OK:      {0}", test.Name);
            ColorHelper.SetDefault();
        }

        public override void TestAsserted(Test test)
        {
            ColorHelper.SetForeground(ConsoleColor.Red);
            Write("FAILED:  {0}", test.Name);
            ColorHelper.SetDefault();
            Write("         Assertion failed in '{0}', at {1}:{2}", test.Assertion.Membername, test.Assertion.Filename, test.Assertion.Line);
            Write("           Expected: {0}", test.Assertion.Expected);
            Write("           But got : {0}", test.Assertion.Actual);
        }

        public override void TestThrew(Test test)
        {
            ColorHelper.SetForeground(ConsoleColor.Red);
            Write("FAILED:  {0}", test.Name);
            ColorHelper.SetDefault();
            Write("         Exception was thrown: {0}", test.Exception);
        }

        public override void TestIgnored(Test test)
        {
            ColorHelper.SetForeground(ConsoleColor.Yellow);
            Write("IGNORED: {0}", test.Name);
            ColorHelper.SetDefault();

            if (!string.IsNullOrEmpty(test.IgnoreReason))
                Write("         " + test.IgnoreReason);
        }

        public override void InternalError(string message)
        {
            ColorHelper.SetForeground(ConsoleColor.Red);
            Write("INTERNAL ERROR: " + message);
            ColorHelper.SetDefault();
        }

        public override void ProjectStarting(string projectName, string targetName)
        {
            base.ProjectStarting(projectName, targetName);
            ColorHelper.SetForeground(ConsoleColor.Cyan);
            Write("Starting project '{0}', target '{1}'", projectName, targetName.ToLower());
            ColorHelper.SetDefault();
            _microseconds = 0;
            _watch.Restart();
        }

        public override void ProjectEnded(List<Test> tests)
        {
            Write();
            ColorHelper.SetForeground(ConsoleColor.Cyan);
            if (_microseconds != 0)
                Write("Built & ran {0} tests in {1:0.00} seconds ({2:#,0} μs)", tests.Count(t => t.Ended), _watch.Elapsed.TotalSeconds, _microseconds);
            else
                Write("Built & ran {0} tests in {1:0.00} seconds", tests.Count(t => t.Ended), _watch.Elapsed.TotalSeconds);

            int notRun = tests.Count(t => t.Ended == false);
            if (notRun> 0)
            {
                ColorHelper.SetForeground(ConsoleColor.Red);
                Write("Not run: {0}", notRun);
            }
            int failures = tests.Count(t => t.Failed);
            if (failures > 0)
            {
                ColorHelper.SetForeground(ConsoleColor.Red);
                Write("Failures: {0}", failures);
            }
            int ignores = tests.Count(t => t.WasIgnored);
            if (ignores > 0)
            {
                ColorHelper.SetForeground(ConsoleColor.Yellow);
                Write("Ignored:  {0}", ignores);
            }
            ColorHelper.SetDefault();
        }

        public override void Log(string message)
        {
            ColorHelper.SetDefault();
            Write(message);
        }

        public override void Logw(string message)
        {
            ColorHelper.SetForeground(ConsoleColor.Yellow);
            Write(message);
            ColorHelper.SetDefault();
        }

        protected virtual void Write(string format = "", params object[] args)
        {
            _writer.Write(format, args);
        }
    }
}
