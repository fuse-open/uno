using System;
using System.IO;
using Uno.IO;

namespace Uno.Build.Stuff
{
    public static class Log
    {
        static Uno.Logging.Log _log = Uno.Logging.Log.Default;

        public static void Configure(Uno.Logging.Log log)
        {
            _log = log;
        }

        public static void Fatal(string format, params object[] args)
        {
            _log.WriteErrorLine("ERROR: " + string.Format(format, args));
        }

        public static void Error(string format, params object[] args)
        {
            _log.Error(string.Format(format, args));
        }

        public static void Warning(string format, params object[] args)
        {
            _log.Warning(string.Format(format, args));
        }

        public static void Event(IOEvent @event, string path)
        {
            _log.VeryVerbose(@event.ToString().ToLower() + " " + path.ToRelativePath(), ConsoleColor.DarkCyan);
        }

        public static void Verbose(string format, params object[] args)
        {
            _log.VeryVerbose(string.Format(format, args), ConsoleColor.DarkGray);
        }

        public static void WriteLine(string format, params object[] args)
        {
            _log.WriteLine(string.Format(format, args));
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            _log.WriteLine(string.Format(format, args), color);
        }
    }
}