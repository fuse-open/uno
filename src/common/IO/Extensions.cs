using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Uno.Diagnostics;

namespace Uno.IO
{
    public static class Extensions
    {
        static readonly bool[] InvalidPathChars = GetInvalidPathChars();

        static bool[] GetInvalidPathChars()
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
                if (path[i] < InvalidPathChars.Length && InvalidPathChars[path[i]])
                    return false;

            return true;
        }

        public static bool IsFullPath(this string path)
        {
            return path.IsValidPath() && Path.IsPathRooted(path);
        }

        public static string ToFullPath(this string filename)
        {
            return Path.GetFullPath(filename);
        }

        public static string ToFullPath(this string filename, string parentDir)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            if (filename.IsValidPath() && Path.IsPathRooted(filename))
                return Path.GetFullPath(filename);

            return Path.GetFullPath(Path.Combine(parentDir, filename));
        }

        public static string GetFullPath(this SourceValue filename)
        {
            if (!filename.String.IsValidPath())
                return filename.String;

            return filename.Source.IsUnknown
                ? Path.GetFullPath(filename.String)
                : filename.String.ToFullPath(Path.GetDirectoryName(filename.Source.FullPath));
        }

        public static string ToRelativePath(this string filename, string parentDir = null, bool forceRelative = false)
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

            // Don't return a longer than input filename (characters or dir parts)
            return forceRelative || result.Length < filename.Length
                                        && (dirParts.Length - s) <= s
                    ? result
                    : filename;
        }

        public static string TrimPath(this string path)
        {
            while (path.StartsWith("./") || path.StartsWith(".\\"))
                path = path.Substring(2);
            while (path.EndsWith("/.") || path.EndsWith("\\."))
                path = path.Substring(0, path.Length - 2);
            return path;
        }

        public static string GetNormalizedFilename(this string filename)
        {
            return filename.GetNormalizedBasename() + Path.GetExtension(filename ?? "").ToLower();
        }

        public static string GetNormalizedBasename(this string filename)
        {
            var hash = filename.GetHashCode().ToString("x8");
            filename = Path.GetFileNameWithoutExtension(filename).ToLower();

            // Remove any old hashes from filename
            for (int i = filename.Length - 1; i > 0; i--)
            {
                if (filename[i] == '-')
                {
                    filename = filename.Substring(0, i);
                    break;
                }

                if (!filename[i].IsHexDigit())
                    break;
            }

            filename = filename + "-" + hash;

            if (filename.Length > 28)
                filename = filename.Substring(0, Math.Max(8, 19)) + "-" + hash;

            // Make sure filename only consists of 'a-z', '0-9', '.' or '-'
            foreach (var c in filename)
            {
                if (!c.IsAnsiLetterDigitDotOrDash())
                {
                    var sb = new StringBuilder();

                    foreach (var d in filename)
                        sb.Append(d.IsAnsiLetterDigitDotOrDash() ? d : '_');

                    return sb.ToString();
                }
            }

            return filename;
        }

        static bool IsAnsiLetterDigitDotOrDash(this char c)
        {
            return c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '.' || c == '-';
        }

        static bool IsHexDigit(this char c)
        {
            return c >= 'a' && c <= 'f' || c >= '0' && c <= '9';
        }
    }
}
