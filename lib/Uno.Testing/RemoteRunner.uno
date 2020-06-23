using Uno;
using Uno.Diagnostics;
using Uno.Text;

namespace Uno.Testing
{
    class RemoteRunner : AbstractRunner
    {
        private const string _prefix = "unotests://";
        private string _currentTest;
        private double _startTime;

        internal RemoteRunner(Registry registry) : base(registry)
        {
        }

        private override void Start()
        {
            var uri = _prefix + "?event=ready"
                + "&testCount=" + TestCount;
            Get(uri);
        }

        private override void Stop()
        {
            Platform.Internal.Unsafe.Quit();
        }

        private override void TestStarting(string name)
        {
            _currentTest = name;
            var uri = _prefix + "?event=testStarted"
                + "&testName=" + EscapeDataString(_currentTest);
            Get(uri);
            _startTime = Clock.GetSeconds();
        }

        private override void TestPassed()
        {
            int us = (int)(1000000.0 * (Clock.GetSeconds() - _startTime));
            var uri = _prefix + "?event=testPassed"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&us=" + us;
            Get(uri);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void TestIgnored(string reason)
        {
            var uri = _prefix + "?event=testIgnored"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&reason=" + EscapeDataString(reason);
            Get(uri);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void AssertionFailed(AssertionFailedException e)
        {
            var uri = _prefix + "?event=testAsserted"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&filename=" + EscapeDataString(e.FileName)
                + "&line=" + e.Line
                + "&membername=" + EscapeDataString(e.MemberName)
                + "&expected=" + EscapeDataString(e.Expected.ToString())
                + "&actual=" + EscapeDataString(e.Actual.ToString());
            Get(uri);
            _currentTest = null;
            SheduleNextTest();
        }

        private override void ExceptionThrown(Exception e)
        {
            var uri = _prefix + "?event=testThrew"
                + "&testName=" + EscapeDataString(_currentTest)
                + "&message=" + EscapeDataString(e.ToString());
            Get(uri);
            _currentTest = null;
            SheduleNextTest();
        }

        private void Get(string uri)
        {
            var maxChunkLen = 120;

            for (int i = 0; i < uri.Length; i += maxChunkLen)
            {
                // We have to chunk up the message, as there is a max length on Android
                // for each log line.
                var chunkLen = Math.Min(uri.Length - i, maxChunkLen);
                var chunk = uri.Substring(i, chunkLen);
                var isLast = i + chunkLen >= uri.Length;
                var output = "{" + chunk.Length + "|" + chunk + "}" + (isLast ? ";" : "\\");

                Debug.Log(output);

                if defined(iOS)
                {
                    // HACK:
                    //
                    // When running on iOS with ios-deploy the debug output stream
                    // sometimes gets interrupted by lldb, causing errors.
                    //
                    // To remedy this problem we send the line twice, and ignore
                    // duplicates on the receiving side.
                    //
                    // This won't FIX the problem completely, at least not in theory.
                    // Ideally we should fix this permanently in ios-deploy, but hopefully
                    // this workaround will be sufficient in the meantime.
                    Debug.Log(output + " (retransmit)");
                }
            }
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
