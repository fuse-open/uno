// This file was generated based on lib/UnoCore/Source/Uno/Diagnostics/Debug.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Diagnostics
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Debug
    {
        public static AssertionHandler _assertionHandler;
        public static LogHandler _logHandler;
        public static string _indentStr;

        static Debug()
        {
            Debug._indentStr = "";
        }

        [global::System.ObsoleteAttribute()]
        public static void SetAssertionHandler(AssertionHandler handler)
        {
            Debug._assertionHandler = handler;
        }

        public static void Assert(bool value, string expression, string filename, int line, object[] operands)
        {
            if (Debug._assertionHandler != null)
                Debug._assertionHandler(value, expression, filename, line, operands);

            if (!value)
                Debug.EmitLog(((((("Assertion Failed: '" + expression) + "' in ") + filename) + "(") + (object)line) + ")", DebugMessageType.Error);
        }

        [global::System.ObsoleteAttribute()]
        public static void SetLogHandler(LogHandler handler)
        {
            Debug._logHandler = handler;
        }

        public static void Log(string message, DebugMessageType type, string filename, int line)
        {
            Debug.EmitLog(message, type);
        }

        public static void Log(object message, DebugMessageType type, string filename, int line)
        {
            Debug.EmitLog((message ?? string.Empty).ToString(), type);
        }

        public static void Log(string message, DebugMessageType type)
        {
            Debug.EmitLog(message, type);
        }

        public static void Log(object message, DebugMessageType type)
        {
            Debug.EmitLog(message.ToString(), type);
        }

        [global::System.ObsoleteAttribute()]
        public static void IndentLog()
        {
            Debug._indentStr = Debug._indentStr + "\t";
        }

        [global::System.ObsoleteAttribute()]
        public static void UnindentLog()
        {
            Debug._indentStr = Debug._indentStr.Substring(0, Debug._indentStr.Length - 1);
        }

        public static void EmitLog(string message, DebugMessageType type)
        {
            if (Debug._logHandler != null)
                Debug._logHandler(Debug._indentStr + message, type);

            if (type == DebugMessageType.Debug)
                global::System.Console.WriteLine(message);
            else if ((int)type < 2)
                global::System.Console.Out.WriteLine(((object)type + ": ") + message);
            else
                global::System.Console.Error.WriteLine(((object)type + ": ") + message);
        }
    }
}
