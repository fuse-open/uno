using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.IO;

namespace Uno.CLI
{
    public static class Arguments
    {
        public static List<string> Cut(this string line)
        {
            var parts = new List<string>();

            foreach (var p in line.Split(null))
                if (!string.IsNullOrEmpty(p))
                    parts.Add(p);

            return parts;
        }

        public static List<string> Grep(this string output, string str)
        {
            var matches = new List<string>();

            foreach (var line in output.Split('\n'))
                if (line != null && line.IndexOf(str, StringComparison.InvariantCulture) != -1)
                    matches.Add(line);

            return matches;
        }

        public static string SplitCommand(this string command, out string args)
        {
            var i = command.UnquotedIndexOf(' ');

            if (i == -1)
            {
                args = null;
                return command;
            }

            args = command.Substring(i + 1).Trim();
            return command.Substring(0, i).Trim('"', '\'');
        }

        public static string SplitCommand(this string command, string workingDir, out string args)
        {
            var filename = command.SplitCommand(out args).TrimPath().UnixToNative();
            var fullName = System.IO.Path.Combine(workingDir, filename);
            return File.Exists(fullName)
                ? fullName
                : filename;
        }

        public static string JoinArguments(this IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(Extensions.QuoteSpace));
        }

        public static void CheckArguments(this IEnumerable<string> args)
        {
            List<string> invalid = null;

            foreach (var s in args)
            {
                if ((!s.IsValidPath() || !File.Exists(s)) && s.StartsWith('-'))
                {
                    if (invalid == null)
                        invalid = new List<string>();

                    invalid.Add(s);
                }
            }

            if (invalid != null)
                throw new ArgumentException("Invalid argument".Plural(invalid) + ": " + string.Join(", ", invalid));
        }

        public static string Path(this IEnumerable<string> args, string defValue = null)
        {
            var array = args.Paths(defValue);

            if (array.Length == 1)
                return array[0];

            throw new ArgumentException("Expected one argument, but " + (
                    array.Length == 0
                        ? "none"
                        : array.Length + " (" + string.Join(", ", array) + ")"
                ) + " were given");
        }

        public static string[] Paths(this IEnumerable<string> args, string defValue = null)
        {
            var array = args.ToArray();
            CheckArguments(array);

            for (int i = 0; i < array.Length; i++)
                array[i] = ParseString(array[i], "...");

            return array.Length == 0 && defValue != null
                ? new[] { defValue }
                : array;
        }

        public static List<string> Files(this IEnumerable<string> args, string pattern = null)
        {
            var array = args.ToArray();
            var result = new List<string>();
            CheckArguments(array);

            if (!array.Any())
                array = new[] { "." };

            foreach (var arg in array)
                if (File.Exists(arg))
                    result.Add(arg.ToFullPath());
                else if (pattern != null && Directory.Exists(arg))
                    foreach (var file in Directory.EnumerateFiles(arg, pattern, SearchOption.TopDirectoryOnly))
                        result.Add(file.ToFullPath());
                else if (arg.IndexOf('*') != -1)
                    foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), arg, SearchOption.TopDirectoryOnly))
                        result.Add(file.ToFullPath());
                else
                    throw new FileNotFoundException("No such file or directory: " + arg);

            if (result.Count == 0)
                throw new ArgumentException("No input files");

            return result;
        }

        public static void ParseProperty(this string x, string name, Dictionary<string, string> result)
        {
            if (string.IsNullOrEmpty(x))
                return;

            var assignIndex = x.UnquotedIndexOf('=');

            if (assignIndex == -1)
                assignIndex = x.UnquotedIndexOf(':');

            if (assignIndex != -1)
                result[x.Substring(0, assignIndex).Trim()] = x.Substring(assignIndex + 1).ParseString(name);
            else
                result[x.Trim()] = null;
        }

        public static double ParseDouble(this string x, string name)
        {
            try
            {
                return double.Parse(x);
            }
            catch (Exception e)
            {
                throw new FormatException(name + ": " + e.Message, e);
            }
        }

        public static int ParseInt(this string x, string name)
        {
            try
            {
                return int.Parse(x);
            }
            catch (Exception e)
            {
                throw new FormatException(name + ": " + e.Message, e);
            }
        }

        public static T ParseEnum<T>(this string x, string name)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), x, true);
            }
            catch (Exception e)
            {
                throw new FormatException(name + ": " + e.Message, e);
            }
        }

        public static string ParseString(this string x, string name)
        {
            if (x == null)
                return null;

            x = x.Trim();
            return x.Length > 1 &&
                x[0] == x[x.Length - 1] && (
                    x[0] == '"' ||
                    x[0] == '\'')
                ? x.Substring(1, x.Length - 2)
                : x;
        }
    }
}
