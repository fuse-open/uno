using Uno;
using Uno.Diagnostics;
using Uno.Text;

namespace Uno.Testing
{
    class RemoteRunner : AbstractRunner
    {
        ITestRunnerMessageDispatcher _dispatcher;

        private const string _prefix = "unotests://";
        private string _currentTest;
        private double _startTime;

        internal RemoteRunner(Registry registry, ITestRunnerMessageDispatcher dispatcher) : base(registry)
        {
            _dispatcher = dispatcher;
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
                + "&testName=" + EscapeDataString(_currentTest);
            Get(query);
            _startTime = Clock.GetSeconds();
        }

        private override void TestPassed()
        {
            int us = (int)(1000000.0 * (Clock.GetSeconds() - _startTime));
            var query = _prefix + "?event=testPassed"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&us=" + us;
            Get(query);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void TestIgnored(string reason)
        {
            var query = _prefix + "?event=testIgnored"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&reason=" + EscapeDataString(reason);
            Get(query);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void AssertionFailed(AssertionFailedException e)
        {
            var query = _prefix + "?event=testAsserted"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&filename=" + EscapeDataString(e.FileName)
                + "&line=" + e.Line
                + "&membername=" + EscapeDataString(e.MemberName)
                + "&expected=" + EscapeDataString(e.Expected.ToString())
                + "&actual=" + EscapeDataString(e.Actual.ToString());
            Get(query);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void ExceptionThrown(Exception e)
        {
            var query = _prefix + "?event=testThrew"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&message=" + EscapeDataString(e.ToString());
            Get(query);
            _currentTest = null;
            SheduleNextTest();
        }

        static int sequenceId = 0;
        private void Get(string query)
        {
            query += "&sequenceId=" + sequenceId++;
            _dispatcher.Get(query);
        }

        static string EscapeDataString(string stringToEscape)
        {
            var bytes = Utf8.GetBytes(stringToEscape);

            var count = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                count++;
                if (EscapeDataSymbol(bytes[i]))
                    count += 2;
            }

            var result = new char[count];
            var index = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var symbol = bytes[i];
                if (EscapeDataSymbol(symbol))
                {
                    result[index++] = '%';
                    result[index++] = GetHexFromNumber(symbol>>4 & 15);
                    result[index++] = GetHexFromNumber(symbol & 15);
                }
                else
                {
                    result[index++] = (char)symbol;
                }
            }
            return new string(result);
        }

        static bool EscapeDataSymbol(byte symbol)
        {
            if (symbol >= 128)
                return true;

            if (char.IsLetter((char)symbol) || char.IsDigit((char)symbol))
                return false;
            switch ((char)symbol)
            {
                case '-':
                case '_':
                case '.':
                case '~':
                    return false;
            }
            return true;
        }

        static char GetHexFromNumber(int value)
        {
            if (value > 9)
                return (char)((byte)'A' + value - 10);
            return (char)((byte)'0' + value);
        }
    }
}
