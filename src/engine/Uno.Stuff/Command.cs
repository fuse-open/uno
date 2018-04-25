using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Stuff.Core;
using Stuff.Format;

namespace Stuff
{
    public abstract class Command
    {
        public const int DefaultWidth = 23;
        int _width = DefaultWidth;
        int _indent;

        public virtual string Name        => null;
        public virtual string Description => null;

        public abstract void Help(IEnumerable<string> args);
        public abstract void Execute(IEnumerable<string> args);

        protected void WriteUsage(params string[] usages)
        {
            var first = true;
            foreach (var line in usages)
            {
                Write(first ? "Usage: " : "  or   ");
                Write(Root + " " + (
                        Name != null
                            ? Name + " "
                            : ""
                    ));

                var i = 0;
                var sb = new StringBuilder();
                while (i < line.Length && (
                        char.IsLetter(line[i]) ||
                        char.IsWhiteSpace(line[i])
                    ))
                    sb.Append(line[i++]);

                if (sb.Length > 0)
                    Log.Write(ConsoleColor.Blue, sb.ToString());

                if (i < line.Length)
                    Log.Write(line[i] == '-'
                            ? ConsoleColor.Magenta
                            : ConsoleColor.DarkYellow,
                        line.Substring(i));

                Console.WriteLine();
                first = false;
            }

            if (Description != null)
                WriteLine("\n" + Description + ".");
        }

        protected void WriteHead(string line, int width = DefaultWidth, int indent = 2)
        {
            _width = width;
            _indent = indent;
            Console.WriteLine();
            Log.WriteLine(ConsoleColor.Green, line);
        }

        protected void WriteRow(string option, string description = null, bool optional = false)
        {
            var head = new string(' ', _indent) + option.PadRight(_width);

            if (option.StartsWith("-"))
                Log.Write(ConsoleColor.Magenta, head);
            else if (IsCommand(option))
                Log.Write(ConsoleColor.Blue, head);
            else
                Write(head);

            if (!string.IsNullOrEmpty(description))
            {
                Write(" ");
                description = description
                    .Replace("\n", "\n   " + new string(' ', _width));

                var i = description.LastIndexOf('(');
                if (i != -1 && description.LastOrDefault() == ')' && (
                        i == 0 || description[i - 1] == ' '
                    ))
                {
                    Write(description.Substring(0, i));
                    Log.Write(ConsoleColor.DarkYellow, description.Substring(i));
                }
                else
                    Write(description);
            }

            if (optional)
                Log.Write(ConsoleColor.DarkYellow, " [optional]");

            Console.WriteLine();
        }

        protected void WriteDefines()
        {
            WriteHead("Common defines");
            Log.WriteLine(string.Join(" ", StuffFile.DefaultConstants.Keys));

            WriteHead("System defines");
            Log.WriteLine(string.Join(" ", StuffFile.DefaultDefines));
        }

        protected void WriteEnvironment()
        {
            WriteHead("Stuff environment", 16, 0);
            WriteRow("STUFF_CACHE", DownloadCache.StuffDirectory.Replace(
                                        Path.GetTempPath().TrimEnd('/', '\\'),
                                        PlatformDetection.IsWindows
                                            ? "%TEMP%"
                                            : "$TEMP") + " (garbage collected)");
        }

        protected void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        protected void Write(string value)
        {
            Console.Write(value);
        }

        static string _root;
        public static string Root
        {
            get { return _root ?? (_root = ExeName); }
            set { _root = value; }
        }

        public static string ExeName =>
            Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location).Replace('-', ' ');

        public static string ExeVersion
        {
            get
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                var versionParts = new List<string>(fileVersionInfo.FileVersion.Split('.'));

                while (versionParts.Count > 2 && versionParts[versionParts.Count - 1] == "0")
                    versionParts.RemoveAt(versionParts.Count - 1);

                return string.Join(".", versionParts);
            }
        }

        bool IsCommand(string str)
        {
            foreach (var c in str)
                if (!char.IsLower(c) && c != '-')
                    return false;
            return true;
        }
    }
}
