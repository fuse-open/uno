using System;
using System.Collections.Generic;
using System.Text;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.CLI
{
    public abstract class Command
    {
        public static readonly Log Log = Log.Default;
        public static readonly Disk Disk = Disk.Default;
        public static readonly Shell Shell = Shell.Default;

        public const int DefaultIndent = 2;
        public const int DefaultWidth = 20;
        int _indent = DefaultIndent;
        int _width = DefaultWidth;

        public virtual string Name => null;
        public virtual string Description => null;
        public virtual bool IsExperimental => false;

        public int? ExitCode { get; protected set; }

        public abstract void Execute(IEnumerable<string> args);

        public virtual void Help(IEnumerable<string> args)
        {
            Help();
        }

        public virtual void Help()
        {
            throw new NotImplementedException();
        }

        protected List<string> GetAllArguments(IEnumerable<string> args, bool globalFlags, params string[] extra)
        {
            var all = new List<string>(args);

            if (globalFlags)
            {
                for (LogLevel i = 0; i < Log.Level; i++)
                    all.Add("-v");
                if (Log.EnableExperimental)
                    all.Add("-x");
            }

            all.AddRange(extra);
            return all;
        }

        protected List<string> GetAllArguments(IEnumerable<string> args, params string[] extra)
        {
            return GetAllArguments(args, true, extra);
        }

        protected void WriteUsage(params string[] usages)
        {
            var first = true;
            foreach (var line in usages)
            {
                Write(first ? "Usage: " : "  or   ");
                Write("uno " + (
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
                    Log.Write(sb, ConsoleColor.Blue);

                if (i < line.Length)
                {
                    first = true;
                    foreach (var arg in line
                                .Substring(i)
                                .Split(' '))
                    {
                        if (!first)
                            Write(" ");
                        Log.Write(arg,
                            arg.StartsWith('-')
                                ? ConsoleColor.Magenta
                                : ConsoleColor.Yellow);
                        first = false;
                    }
                }

                Log.WriteLine();
                first = false;
            }

            if (Description != null) 
                WriteLine("\n" + Description.Trailing('.'));
        }

        protected void WriteHead(string line, int width = DefaultWidth, int indent = DefaultIndent)
        {
            _width = width;
            _indent = indent;
            Log.H2(line);
        }

        protected void WriteRow(string option, string description = null, bool optional = false, bool parse = true)
        {
            if (string.IsNullOrEmpty(description) && option.IndexOf('\n') != -1)
            {
                foreach (var line in option.Split('\n'))
                    WriteRow(line);

                return;
            }

            Log.Flush();
            Log.Write(new string(' ', _indent));

            if (!parse)
                Log.Write(option.PadRight(_width));
            else if (option.TrimStart().StartsWith('-') || option.IndexOf('=') != -1)
            {
                var w = _width;
                for (var n = option.IndexOf(',');
                         n != -1 && n < w - 2;
                         n = option.IndexOf(','))
                {
                    Log.Write(option.Substring(0, n), ConsoleColor.Magenta);
                    Log.Write(option.Substring(n, 2));
                    n += 2;
                    option = option.Substring(n);
                    w -= n;
                }

                var j = option.IndexOf(':') + 1;
                var i = option.IndexOf('=', j) + 1;
                if (i == 0)
                    i = option.IndexOf('<');
                if (i > j)
                {
                    if (j > 0)
                    {
                        Log.Write(option.Substring(0, j), ConsoleColor.Magenta);
                        Log.Write(option.Substring(j, i - j - 1), ConsoleColor.Blue);
                        Log.Write(option.Substring(i - 1, 1), ConsoleColor.Magenta);
                    }
                    else
                        Log.Write(option.Substring(0, i), ConsoleColor.Magenta);

                    Log.Write(option.Substring(i), ConsoleColor.Blue);

                    var diff = w - option.Length;
                    if (diff > 0)
                        Log.Write(new string(' ', diff));
                }
                else
                    Log.Write(option.PadRight(w), ConsoleColor.Magenta);
            }
            else if (option.StartsWith("uno", StringComparison.InvariantCulture))
            {
                var i = option.IndexOf('-');
                if (i > 0)
                {
                    Log.Write(option.Substring(0, i));
                    Log.Write(option.Substring(i), ConsoleColor.Magenta);

                    var diff = _width - option.Length;
                    if (diff > 0)
                        Log.Write(new string(' ', diff));
                }
                else
                    Log.Write(option.PadRight(_width));
            }
            else if (option.StartsWith("* ", StringComparison.InvariantCulture))
            {
                Log.Write(option.Substring(0, 2), ConsoleColor.Red);
                Log.Write(option.Substring(2).PadRight(_width - 2));
            }
            else if (IsCommand(option))
                Log.Write(option.PadRight(_width), ConsoleColor.Blue);
            else
                Log.Write(option.PadRight(_width));

            if (!string.IsNullOrEmpty(description))
            {
                Log.Write("  ");
                if (description.IndexOf('\n') != -1)
                    Log.Write(description.Replace("\n", "\n    " + new string(' ', _width)));
                else if (!parse)
                    Log.Write(description);
                else
                {
                    var i = description.LastIndexOf('(');
                    if (i <= 0 || description[i - 1] != ' ')
                        i = description.LastIndexOf('[');

                    if (!optional && i > 0)
                    {
                        Log.Write(description.Substring(0, i));
                        Log.Write(description.Substring(i), ConsoleColor.DarkGray);
                    }
                    else
                        Log.Write(description);
                }

                if (optional)
                    Log.Write(" [optional]", ConsoleColor.DarkGray);
            }

            Log.WriteLine();
        }

        protected void WriteRow(object option, object description = null, bool optional = false, bool parse = true)
        {
            WriteRow(option?.ToString(), description?.ToString(), optional, parse);
        }

        protected void WriteLine(object line)
        {
            Log.WriteLine(line);
        }

        protected void WriteLine(string line)
        {
            Log.WriteLine(line);
        }

        protected void Write(string str)
        {
            Log.Write(str);
        }

        protected void WriteProductInfo()
        {
            WriteLine(UnoVersion.LongHeader);
            WriteLine(UnoVersion.Copyright);

            WriteHead("Product", 10, 0);
            WriteRow("Commit",      UnoVersion.CommitSha);
            WriteRow("Version",     UnoVersion.FileVersion.FileVersion);

            WriteHead("Environment", 10, 0);
            WriteRow("OSVersion",   Environment.OSVersion.VersionString);
            WriteRow("Version",     Environment.Version);

            // Mono version
            if (MonoInfo.IsRunningMono)
            {
                WriteHead("Mono", 10, 0);
                WriteRow("Path",    MonoInfo.GetPath());
                WriteRow("Version", MonoInfo.GetVersion());
            }
        }

        protected bool Confirm(string question)
        {
            Log.WriteLine(question, ConsoleColor.Cyan);
            for (;;)
            {
                Log.Write("    [Y/N]: ", ConsoleColor.DarkGray);
                switch (Console.ReadLine()?.Trim().ToUpperInvariant())
                {
                    case "Y": return true;
                    case "N": return false;
                    case null: return false;
                }
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
