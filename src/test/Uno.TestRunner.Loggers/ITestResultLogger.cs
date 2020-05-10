using System.Collections.Generic;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public interface ITestResultLogger
    {
        void TestPassed(Test test);
        void TestAsserted(Test test);
        void TestThrew(Test test);
        void TestIgnored(Test test);
        void InternalError(string message);
        void ProjectStarting(string project, string target);
        void ProjectEnded(List<Test> tests);
        void Log(string message);
        void Log(string message, params object[] formatItems);
        void Logw(string message);
        void Logw(string message, params object[] formatItems);
        void Trace(string message);
        void Trace(string message, params object[] formatItems);
    }
}
