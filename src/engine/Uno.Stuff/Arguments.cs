using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Stuff.Format;

namespace Stuff
{
    public static class Arguments
    {
        static readonly bool[] Invalid = GetInvalid();

        static bool[] GetInvalid()
        {
            var list = new List<bool>();
            foreach (var c in Path.GetInvalidPathChars().Concat(new[] {'*'}))
            {
                int i = c;
                while (i >= list.Count)
                    list.Add(false);
                list[i] = true;
            }

            return list.ToArray();
        }

        public static bool IsValidPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            for (int i = 0; i < path.Length; i++)
                if (path[i] < Invalid.Length && Invalid[path[i]])
                    return false;

            return true;
        }

        public static List<string> GetFiles(this IEnumerable<string> args, string pattern)
        {
            var array = args.ToArray();
            var result = new List<string>();
            CheckArguments(array);

            if (!array.Any())
                array = new[] { "." };

            foreach (var arg in array)
                if (File.Exists(arg))
                    result.Add(Path.GetFullPath(arg));
                else if (Directory.Exists(arg))
                    foreach (var file in Directory.EnumerateFiles(arg, pattern, SearchOption.TopDirectoryOnly))
                        result.Add(Path.GetFullPath(file));
                else if (arg.IndexOf('*') != -1)
                    foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), arg, SearchOption.TopDirectoryOnly))
                        result.Add(Path.GetFullPath(file));
                else
                    throw new FileNotFoundException("No such file or directory: " + arg);

            if (result.Count == 0)
                throw new ArgumentException("No input files");

            return result;
        }

        public static string GetSingleArgument(this IEnumerable<string> args, string defValue = null)
        {
            var array = args.ToArray();
            CheckArguments(array);

            if (array.Length == 1)
                return array[0];

            if (array.Length == 0 && defValue != null)
                return defValue;

            throw new ArgumentException("Expected one argument, but " + (
                    array.Length == 0
                        ? "none"
                        : array.Length + " (" + string.Join(", ", array) + ")"
                ) + " were given");
        }

        public static void CheckArguments(this IEnumerable<string> args)
        {
            List<string> invalid = null;

            foreach (var s in args)
            {
                if ((!s.IsValidPath() || !File.Exists(s)) && s.StartsWith("-"))
                {
                    if (invalid == null)
                        invalid = new List<string>();

                    invalid.Add(s);
                }
            }

            if (invalid != null)
                throw new ArgumentException("Invalid argument(s): " + string.Join(", ", invalid));
        }

        public static string Relative(this string filename, string parentDir = null, bool forceRelative = false)
        {
            if (string.IsNullOrEmpty(parentDir))
            {
                try
                {
                    parentDir = Directory.GetCurrentDirectory();
                }
                catch (FileNotFoundException)
                {
                    // GetCurrentDirectory() can throw on macOS if there's no current directory.
                    // If that happens, we just don't know, but this doesn't have to be fatal.
                    parentDir = "";
                }
            }

            if (PlatformDetection.IsWindows)
            {
                // Convert drive letters to uppercase to avoid bugs
                if (filename.Length > 1 && filename[1] == ':' && char.IsLower(filename[0]))
                    filename = char.ToUpper(filename[0]) + filename.Substring(1);
                if (parentDir.Length > 1 && parentDir[1] == ':' && char.IsLower(parentDir[0]))
                    parentDir = char.ToUpper(parentDir[0]) + parentDir.Substring(1);
            }

            if (filename == parentDir)
                return "";

            parentDir = parentDir.TrimEnd(Path.DirectorySeparatorChar);
            if (parentDir.Length > 1 &&
                filename.Length > parentDir.Length &&
                filename.ToUpperInvariant().StartsWith((parentDir + Path.DirectorySeparatorChar).ToUpperInvariant(), 
                    StringComparison.InvariantCulture))
                return filename.Substring(parentDir.Length + 1);

            if (parentDir.IndexOf(Path.DirectorySeparatorChar) == -1 ||
                filename.IndexOf(Path.DirectorySeparatorChar) == -1)
                return filename;

            var dirParts = parentDir.Split(Path.DirectorySeparatorChar);
            var fileParts = filename.Split(Path.DirectorySeparatorChar);

            int s = 0;
            while (s < Math.Min(dirParts.Length, fileParts.Length - 1) && dirParts[s] == fileParts[s])
                s++;

            var sb = new StringBuilder();

            for (int i = s; i < dirParts.Length; i++)
                sb.Append(".." + Path.DirectorySeparatorChar);

            for (int i = s; i < fileParts.Length - 1; i++)
                sb.Append(fileParts[i] + Path.DirectorySeparatorChar);

            // Keep trailing slash if specified
            if (!string.IsNullOrEmpty(fileParts.Last()))
                sb.Append(fileParts.Last());

            var result = sb.ToString();
            // Don't return a longer than input filename
            return forceRelative || result.Length < filename.Length
                    ? result
                    : filename;
        }

        public static string Quote(this string str)
        {
            return string.IsNullOrEmpty(str) ||
                str.Any(c => !c.IsKey() && c != '+' && c != '\\')
                    ? "'" + str + "'"
                    : str;
        }
    }
}
