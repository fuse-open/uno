using Uno.Compiler.ExportTargetInterop;
using System;

namespace Uno.Diagnostics
{
    /** An enum representing the severity of a log message. */
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }

    /** A class for writing to the log. */
    public static class Log
    {
        extern(PREVIEW)
        static Action<LogLevel, string> _handler;

        extern(PREVIEW)
        public static void SetHandler(Action<LogLevel, string> handler)
        {
            _handler = handler;
        }

        /** Writes a debug message to the log. */
        public static void Debug(string message)
        {
            WriteLine(LogLevel.Debug, message);
        }

        /** Writes a debug message to the log. */
        public static void Debug(string format, params object[] args)
        {
            WriteLine(LogLevel.Debug, format, args);
        }

        /** Writes an informational message to the log. */
        public static void Information(string message)
        {
            WriteLine(LogLevel.Information, message);
        }

        /** Writes an informational message to the log. */
        public static void Information(string format, params object[] args)
        {
            WriteLine(LogLevel.Information, format, args);
        }

        /** Writes a warning message to the log. */
        public static void Warning(string message)
        {
            WriteLine(LogLevel.Warning, message);
        }

        /** Writes a warning message to the log. */
        public static void Warning(string format, params object[] args)
        {
            WriteLine(LogLevel.Warning, format, args);
        }

        /** Writes an error message to the log. */
        public static void Error(string message)
        {
            WriteLine(LogLevel.Error, message);
        }

        /** Writes an error message to the log. */
        public static void Error(string format, params object[] args)
        {
            WriteLine(LogLevel.Error, format, args);
        }

        /** Writes a fatal error message to the log. */
        public static void Fatal(string message)
        {
            WriteLine(LogLevel.Fatal, message);
        }

        /** Writes a fatal error message to the log. */
        public static void Fatal(string format, params object[] args)
        {
            WriteLine(LogLevel.Fatal, format, args);
        }

        /** Writes a message to the log. */
        public static void WriteLine(LogLevel level, string format, params object[] args)
        {
            WriteLine(level, string.Format(format, args));
        }

        /** Writes a message to the log. */
        public static void WriteLine(LogLevel level, string message)
        {
            if defined(PREVIEW)
                if (_handler != null)
                    _handler(level, message);

            if defined(CPLUSPLUS)
            @{
                uCString cstr($1);
                uLog($0, "%s", cstr.Ptr);
            @}
            else if defined(DOTNET)
            {
                if (level == 0)
                    Console.WriteLine(message);
                else if ((int) level < LogLevel.Warning)
                    Console.Out.WriteLine(level + ": " + message);
                else
                    Console.Error.WriteLine(level + ": " + message);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
