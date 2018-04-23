using System.Collections.Generic;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public class TeamCityResultLogger : AbstractLogger
    {
        private readonly IWriter _writer;

        public TeamCityResultLogger(IWriter writer, bool trace=false) : base(trace)
        {
            _writer = writer;
        }

        public override void TestPassed(Test test)
        {
            PretendTestStarted(test);
            TestFinished(test);
        }

        private void TestFinished(Test test)
        {
            WriteToTeamCity("testFinished name='{0}.{1}'", ProjectName, test.Name);
        }

        public override void TestAsserted(Test test)
        {
            PretendTestStarted(test);
            WriteToTeamCity("testFailed type='comparisonFailure' name='{0}.{1}' details='{2}:{3}' expected='{4}' actual='{5}'", ProjectName , test.Name, Escape(test.Assertion.Filename), test.Assertion.Line, Escape(test.Assertion.Expected), Escape(test.Assertion.Actual));
            TestFinished(test);
        }

        public override void TestThrew(Test test)
        {
            PretendTestStarted(test);
            WriteToTeamCity("testFailed name='{0}.{1}' details='{2}'", ProjectName, test.Name, Escape(test.Exception));
            TestFinished(test);
        }

        public override void TestIgnored(Test test)
        {
            WriteToTeamCity("testIgnored name='{0}.{1}' message='{2}'", ProjectName, test.Name, Escape(test.IgnoreReason));
        }

        public override void InternalError(string message)
        {
            Error(message);
        }

        public override void ProjectStarting(string projectName, string targetName)
        {
            base.ProjectStarting(projectName, targetName);
            WriteToTeamCity("testSuiteStarted name='{0}'", ProjectName);

        }

        public override void ProjectEnded(List<Test> tests)
        {
            WriteToTeamCity("testSuiteFinished name='{0}'", ProjectName);
        }

        void PretendTestStarted(Test test)
        {
            WriteToTeamCity("testStarted name='{0}.{1}' captureStandardOutput='false'", ProjectName, test.Name);
        }

        public override void Log(string message)
        {
            WriteToTeamCity("message text='{0}'", Escape(message));
        }

        public override void Logw(string message)
        {
            WriteToTeamCity("message text='{0}'", Escape(message));
        }

        private void Error(string message)
        {
            WriteToTeamCity("message text='{0}' status='ERROR'", Escape(message));
        }

        void WriteToTeamCity(string format, params object[] args)
        {
            _writer.Write("##teamcity[" + format + "]", args);
        }


        static string Escape(string value)
        {
            if (value == null)
                return string.Empty;

            return value.Replace("|", "||")
                        .Replace("'", "|'")
                        .Replace("\r", "|r")
                        .Replace("\n", "|n")
                        .Replace("[", "|[")
                        .Replace("]", "|]")
                        .Replace("\u2029", "|p")
                        .Replace("\u2028", "|l")
                        .Replace("\u0085", "|x");
        }
    }
}
