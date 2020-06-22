using Uno.Compiler.ExportTargetInterop;
using System;

namespace Uno.Diagnostics
{
    public enum DebugMessageType
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }

    public delegate void AssertionHandler(bool value, string expression, string filename, int line, params object[] operands);

    public delegate void LogHandler(string message, DebugMessageType type);

    public static class Debug
    {
        // TODO: Deprecated
        static AssertionHandler _assertionHandler;

        [Obsolete]
        public static void SetAssertionHandler(AssertionHandler handler)
        {
            _assertionHandler = handler;
        }

        // TODO: Deprecated
        public static void Assert(bool value, string expression, string filename, int line, params object[] operands)
        {
            if (_assertionHandler != null)
            {
                _assertionHandler(value, expression, filename, line, operands);
            }
            if (!value)
            {
                EmitLog("Assertion Failed: '" + expression + "' in " + filename + "(" + line + ")", DebugMessageType.Error);
            }
        }

        static LogHandler _logHandler;

        [Obsolete]
        public static void SetLogHandler(LogHandler handler)
        {
            _logHandler = handler;
        }

        public static void Log(string message, DebugMessageType type, string filename, int line)
        {
            EmitLog(message, type);
        }

        public static void Log(object message, DebugMessageType type, string filename, int line)
        {
            EmitLog((message ?? string.Empty).ToString(), type);
        }

        public static void Log(string message, DebugMessageType type = 0)
        {
            EmitLog(message, type);
        }

        public static void Log(object message, DebugMessageType type = 0)
        {
            EmitLog(message.ToString(), type);
        }

        static string _indentStr = "";

        [Obsolete]
        public static void IndentLog()
        {
            _indentStr += "\t";
        }

        [Obsolete]
        public static void UnindentLog()
        {
            _indentStr = _indentStr.Substring( 0, _indentStr.Length - 1 );
        }

        static void EmitLog(string message, DebugMessageType type)
        {
            if (_logHandler != null)
                _logHandler(_indentStr + message, type);

            if defined(CPLUSPLUS)
            @{
                uCString cstr($0);
                uLog($1, "%s", cstr.Ptr);
            @}
            else if defined(DOTNET)
            {
                if (type == 0)
                    Console.WriteLine(message);
                else if ((int) type < DebugMessageType.Warning)
                    Console.Out.WriteLine(type + ": " + message);
                else
                    Console.Error.WriteLine(type + ": " + message);
            }
            else
                build_error;
        }
    }
}
