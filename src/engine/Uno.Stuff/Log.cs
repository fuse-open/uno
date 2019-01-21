using System;
using System.IO;
using Uno.IO;

namespace Stuff
{
    public static class Log
    {
        public static TextWriter OutWriter = Console.Out;
        public static TextWriter ErrorWriter = Console.Error;
        public static bool EnableVerbose;

        public static void Configure(bool verbose, TextWriter outWriter, TextWriter errorWriter)
        {
            EnableVerbose = verbose;
            OutWriter = outWriter;
            ErrorWriter = errorWriter;
        }

        public static void Fatal(string format, params object[] args)
        {
            WriteLine(Console.Error, "ERROR: " + format, args);
        }

        public static void Error(string format, params object[] args)
        {
            WriteLine(Console.Error, ConsoleColor.Red, "ERROR: " + format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            WriteLine(Console.Error, ConsoleColor.Yellow, "WARNING: " + format, args);
        }

        public static void Event(IOEvent @event, string path)
        {
            if (EnableVerbose)
                WriteLine(OutWriter, ConsoleColor.DarkCyan, @event.ToString().ToLower() + " " + path.ToRelativePath());
        }

        public static void Verbose(string format, params object[] args)
        {
            if (EnableVerbose)
                WriteLine(OutWriter, ConsoleColor.DarkGray, format, args);
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(OutWriter, format, args);
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            WriteLine(OutWriter, color, format, args);
        }

        static void WriteLine(TextWriter writer, string format, params object[] args)
        {
            lock (writer)
                writer.WriteLine(format, args);
        }

        static void WriteLine(TextWriter writer, ConsoleColor color, string format, params object[] args)
        {
            lock (writer)
            {
                try
                {
                    Console.ForegroundColor = color;
                    writer.WriteLine(format, args);
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }

        public static void Write(ConsoleColor color, string format, params object[] args)
        {
            lock (OutWriter)
            {
                try
                {
                    Console.ForegroundColor = color;
                    OutWriter.Write(format, args);
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }
    }
}