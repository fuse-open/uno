using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.Diagnostics;
using Uno.IO;

namespace Uno.Logging
{
    public class Log : IDisposable
    {
        public static LogLevel DefaultLevel = GetEnvironmentLevel();
        public static bool DefaultTrace = GetEnvironmentTrace();

        public static readonly Log Null = new Log(TextWriter.Null);
        public static readonly Log Default = new Log();


        static LogLevel GetEnvironmentLevel()
        {
            var level = Environment.GetEnvironmentVariable("LOG_LEVEL");

            try
            {
                return !string.IsNullOrEmpty(level)
                    ? (LogLevel) Enum.Parse(typeof(LogLevel), level, true)
                    : LogLevel.Compact;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to parse LOG_LEVEL: " + e);
                return LogLevel.Verbose;
            }
        }

        static bool GetEnvironmentTrace()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOG_TRACE"));
        }

        public IErrorList Errors { get; } = ErrorList.Null;
        public LogWriter OutWriter { get; }
        public LogWriter ErrorWriter { get; }
        public LogLevel Level { get; set; } = DefaultLevel;
        public int MaxErrorCount { get; set; }
        public bool EnableAnimation { get; set; }
        public bool EnableTrace { get; set; } = DefaultTrace;
        public bool EnableExperimental { get; set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }
        public int WarningLevel { get; set; } = 2;
        public bool IsUltraVerbose => Level >= LogLevel.UltraVerbose;
        public bool IsVeryVerbose => Level >= LogLevel.VeryVerbose;
        public bool IsVerbose => Level > 0;
        public bool HasErrors => ErrorCount > 0;
        public bool HasWarnings => WarningCount > 0;
        public object Lock => _state;

        readonly LogState _state;
        readonly List<string> _errors = new List<string>();

        public Log(TextWriter outWriter = null, TextWriter errorWriter = null, LogState state = null)
        {
            if (outWriter == null)
                outWriter = Console.Out;
            if (errorWriter == null)
                errorWriter = outWriter == Console.Out
                                ? Console.Error
                                : outWriter;

            _state = state ?? new LogState();
            OutWriter = new LogWriter(_state, outWriter);
            ErrorWriter = new LogWriter(_state, errorWriter);
            EnableAnimation = outWriter == Console.Out && !Console.IsOutputRedirected;
        }

        public Log(IErrorList errors, TextWriter outWriter = null, TextWriter errorWriter = null, LogState state = null)
            : this(outWriter, errorWriter, state)
        {
            Errors = errors ?? ErrorList.Null;
        }

        public void Dispose()
        {
            _state.Dispose();
        }

        public Log GetQuieterLog(bool silent = false)
        {
            return silent || !IsVerbose
                //                v-- Silence STDOUT, but keep STDERR
                ? new Log(Errors, TextWriter.Null, ErrorWriter, _state) {EnableTrace = EnableTrace}
                : this;
        }

        public string GetErrorSummary(int startIndex = 0)
        {
            var sb = new StringBuilder();
            for (int i = startIndex; i < _errors.Count; i++)
                sb.AppendLine(
                    _errors[i]
                        .Split('\n')
                        .First()
                        .TrimEnd());
            return sb
                .ToString()
                .TrimEnd();
        }

        public void Error(Source src, object code, string msg)
        {
            var str = code?.ToString() ?? "E0000";
            Report(src, str, msg, ConsoleColor.Red);

            lock (Errors)
            {
                ErrorCount++;
                Errors.AddError(src, str, msg);

                if (MaxErrorCount > 0 && ErrorCount >= MaxErrorCount)
                {
                    MaxErrorCount = 0; // disable throwing more exceptions
                    throw new MaxErrorException();
                }
            }
        }

        public void FatalError(Source src, object code, string msg)
        {
            var str = code?.ToString() ?? "E0000";
            Report(src, str, msg, ConsoleColor.Red, true);

            lock (Errors)
            {
                ErrorCount++;
                Errors.AddFatalError(src, str, msg);
                // This method is used when an unexpected exception is caught,
                // and should not throw a new MaxErrorCountException
            }
        }

        public void Warning1(Source src, object code, string msg)
        {
            // Silence warnings from cached packages (unless -W3 was passed)
            if (src.Package.IsCached && WarningLevel < 3 || src.Package.IsVerified ||
                MaxErrorCount > 0 && WarningCount >= MaxErrorCount ||
                WarningLevel == 0)
                return;

            // Silence warnings from UX generated code (unless -W3 was passed)
            if (src.FullPath.EndsWith(".g.uno") && WarningLevel < 3)
                return;

            var str = code?.ToString() ?? "W0000";
            Report(src, str, msg, ConsoleColor.Yellow);

            lock (Errors)
            {
                WarningCount++;
                Errors.AddWarning(src, str, msg);
            }
        }
        
        // Warning level 2 is the default.
        public void Warning(Source src, object code, string msg)
        {
            if (WarningLevel >= 2)
                Warning1(src, code, msg);
        }
        
        public void Warning3(Source src, object code, string msg)
        {
            if (WarningLevel >= 3)
                Warning1(src, code, msg);
        }

        public void Message(Source src, object code, string msg)
        {
            var str = code?.ToString() ?? "M0000";
            Report(src, str, msg, ConsoleColor.DarkCyan);

            lock (Errors)
                Errors.AddMessage(src, str, msg);
        }

        public void Error(string line)
        {
            lock (Errors)
                ErrorCount++;
            WriteErrorLine("ERROR: " + line, ConsoleColor.Red);
        }

        public void Warning(string line)
        {
            WriteErrorLine("WARNING: " + line, ConsoleColor.Yellow);
        }

        public void Message(string line)
        {
            WriteLine(line, ConsoleColor.DarkGray);
        }

        public void Verbose(string line, ConsoleColor? color = null)
        {
            WriteLine(LogLevel.Verbose, line, color);
        }

        public void VeryVerbose(string line, ConsoleColor? color = null)
        {
            WriteLine(LogLevel.VeryVerbose, line, color ?? ConsoleColor.DarkGray);
        }

        public void UltraVerbose(string line, ConsoleColor? color = null)
        {
            WriteLine(LogLevel.UltraVerbose, line, color ?? ConsoleColor.DarkMagenta);
        }

        public void ProductHeader()
        {
            WriteLine(UnoVersion.ShortHeader);
            WriteLine(UnoVersion.Copyright);
            Skip();
        }

        public void H1(string line, ConsoleColor color = ConsoleColor.Green)
        {
            Skip();
            WriteLine(line, color);
            WriteLine(new string('-', line.Length), color);
        }

        public void H2(string line, ConsoleColor color = ConsoleColor.Green)
        {
            Skip();
            WriteLine(line, color);
        }

        public void Event(object @event, string path, string extra = null)
        {
            if (Level < LogLevel.UltraVerbose)
                return;

            path = path.IsValidPath() && Path.IsPathRooted(path)
                ? path.ToRelativePath()
                : path.TrimPath();
            if (!string.IsNullOrEmpty(extra))
                path += "; " + extra;

            WriteLine(@event.ToString().ToLowerInvariant() + " " + path, ConsoleColor.DarkCyan);
        }

        public void Reset()
        {
            ErrorCount = 0;
            WarningCount = 0;
            _errors.Clear();
        }

        public void DisableSkip()
        {
            if (!OutWriter.IsNull)
                _state.DisableSkip = true;
        }

        public void Skip(bool force = false)
        {
            if (force)
                _state.DisableSkip = false;
            if (_state.DeferLine)
                Flush();
            if (!OutWriter.IsNull)
                _state.SkipLine = true;
        }

        public void Flush()
        {
            lock (_state)
                _state.Flush(OutWriter.Inner);
        }

        public double Time => _state.Time;

        public void BuildCompleted(double startTime)
        {
            Skip(true);
            var verb = HasErrors ? "failed" : "completed";
            WriteLine($"Build {verb} in {Time - startTime:0.00} seconds");
            if (HasWarnings)
                WriteLine($"{WarningCount,5} warning".Plural(WarningCount), ConsoleColor.Yellow);
            if (HasErrors)
                WriteLine($"{ErrorCount,5} error".Plural(ErrorCount), ConsoleColor.Red);
        }

        // Nullable (ConsoleColor?) on second parameter makes Mono 4.X fail in overload resolution.
        public IDisposable StartAnimation(string line, ConsoleColor color = ConsoleColor.Green)
        {
            Skip();
            lock (_state)
                return _state.BeginDeferred(OutWriter.Inner, line.TrimEnd(), color, EnableAnimation);
        }

        public IDisposable StartAnimation(ConsoleColor? color = null)
        {
            lock (_state)
                return _state.BeginDeferred(OutWriter.Inner, null, color, EnableAnimation);
        }

        public IDisposable StartProfiler(object typeObject)
        {
            return new LogProfiler(this, typeObject);
        }

        public void Trace(Exception e)
        {
            if (!EnableTrace && Level < LogLevel.VeryVerbose)
                return;

            Skip();

            var lines = e.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
                Message(line);
        }

        public void WriteLine(LogLevel level, string line, ConsoleColor? color = null)
        {
            if (level <= Level)
                WriteLine(line, color);
        }

        public void WriteLine(object line, ConsoleColor? color = null)
        {
            Flush();
            Write(line, color);
            lock (_state)
                OutWriter.Inner.WriteLine();
        }

        public void WriteLine(string line, ConsoleColor? color = null)
        {
            WriteLine(line, color, OutWriter);
        }

        public void WriteErrorLine(string line, ConsoleColor? color = null)
        {
            WriteLine(line, color, ErrorWriter);
        }

        public void WriteLine()
        {
            Flush();
            lock (_state)
                OutWriter.Inner.WriteLine();
        }

        public void Write(string str, ConsoleColor? color = null)
        {
            lock (_state)
            {
                _state.SetColor(OutWriter.Inner, color);
                OutWriter.Inner.Write(str);
            }
        }

        public void Write(object obj, ConsoleColor? color = null)
        {
            lock (_state)
            {
                _state.SetColor(OutWriter.Inner, color);
                OutWriter.Inner.Write(obj);
            }
        }

        void Report(Source src, string code, string msg, ConsoleColor color, bool fatal = false)
        {
            var str = src + ": " + (fatal ? "FATAL " : "") + code + (msg.Length > 0 ? ": " + msg : "");

            if (color == ConsoleColor.Red)
                lock (_errors)
                    _errors.Add(str);

            WriteLine(str, color, ErrorWriter);
        }

        void WriteLine(string line, ConsoleColor? color, LogWriter writer)
        {
            if (writer.IsNull)
                return;

            lock (_state)
            {
                _state.Flush(writer.Inner);
                _state.SetColor(writer.Inner, color);
                writer.Inner.WriteLine(line.TrimEnd());
            }
        }
    }
}
