using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Uno.Logging;

namespace Uno.TestRunner
{
    class DebugLogTestFilter : TextWriter
    {
        static readonly Regex TestChunkRegex = new Regex(@"\{(?<len>\d+)\|(?<chunk>[^\|]*)\}(?<endchar>[\\\;])", RegexOptions.Compiled);
        private readonly TextWriter _wrapped;
        private readonly TestRun _testRun;
        private readonly StringBuilder _currentTestLine = new StringBuilder();
        private readonly StringBuilder _currentMessage = new StringBuilder();
        private string _lastMatch;
        private bool _silenceNextEmptyLine;

        public DebugLogTestFilter(TextWriter wrapped, TestRun testRun)
        {
            if (wrapped == null) throw new ArgumentNullException(nameof(wrapped));
            if (testRun == null) throw new ArgumentNullException(nameof(testRun));
            _wrapped = wrapped;
            _testRun = testRun;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseChar(char c)
        {
            _currentTestLine.Append(c);
            if (c == '\n')
                TestLineTerminated();
        }

        private void TestLineTerminated()
        {
            
            var line = _currentTestLine.ToString();
            var match = TestChunkRegex.Match(line.Replace("(lldb) ", ""));
            int verifyUriLen;

            // The format of each test line from debug log is |<chunk length>|<chunk>|<endchar>
            // Uri got (made up) scheme `unotests://`, and the length is prepended to
            // verify that no characters have been inserted in the middle of the uri,
            // which unfortunately sometimes happen when using ios-deploy. 
            if (match.Success &&
                int.TryParse(match.Groups["len"].Value, out verifyUriLen) &&
                match.Groups["chunk"].Value.Length == verifyUriLen)
            {
                if (match.Value != _lastMatch)
                {
                    _currentMessage.Append(match.Groups["chunk"]);
                    if (match.Groups["endchar"].Value == ";")
                    {
                        var uri = new Uri(_currentMessage.ToString());
                        _testRun.EventOccured(HttpUtility.ParseQueryString(uri.Query));
                        _currentMessage.Clear();
                    }
                    _lastMatch = match.Value;
                }

                if (Log.Default.Level >= LogLevel.VeryVerbose)
                {
                    _wrapped.Write(line);
                }
                else
                {
                    // We always seem to get an empty extra line following
                    // every output from log on ios-deploy.
                    //
                    // This is not critical, but quite annoying, so we'll
                    // avoid printing those. (except when using -vv)
                    _silenceNextEmptyLine = true;
                }
            }
            else
            {
                if (line != "\n" || !_silenceNextEmptyLine)
                    _wrapped.Write(line);

                _silenceNextEmptyLine = false;
            }
            _currentTestLine.Clear();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                ParseChar(buffer[index + i]);
            }
        }

        public sealed override void Write(char value)
        {
            ParseChar(value);
        }

        public override Encoding Encoding => _wrapped.Encoding;
    }
}