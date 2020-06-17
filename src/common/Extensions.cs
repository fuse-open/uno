using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Uno
{
    public static class Extensions
    {
        public static char First(this string str)
        {
            return str[0];
        }

        public static char Last(this string str)
        {
            return str[str.Length - 1];
        }

        public static T First<T>(this T[] list)
        {
            return list[0];
        }

        public static T Last<T>(this T[] list)
        {
            return list[list.Length - 1];
        }

        public static T First<T>(this IReadOnlyList<T> list)
        {
            return list[0];
        }

        public static T Last<T>(this IReadOnlyList<T> list)
        {
            return list[list.Count - 1];
        }

        public static T First<T>(this List<T> list)
        {
            return list[0];
        }

        public static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static T RemoveLast<T>(this List<T> list)
        {
            var index = list.Count - 1;
            var result = list[index];
            list.RemoveAt(index);
            return result;
        }

        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (var e in items)
                set.Add(e);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> items)
        {
            foreach (var e in items)
                self.Add(e.Key, e.Value);
        }

        public static bool StartsWith(this string str, char a, char b = '\0')
        {
            if (str.Length > 0)
            {
                var first = str[0];
                if (a == first || b == first)
                    return true;
            }

            return false;
        }

        public static bool EndsWith(this string str, char a, char b = '\0')
        {
            if (str.Length > 0)
            {
                var last = str[str.Length - 1];
                if (a == last || b == last)
                    return true;
            }

            return false;
        }

        public static bool EndsWith(this string str, string a, string b)
        {
            return str.EndsWith(a) || str.EndsWith(b);
        }

        public static string[] PathSplit(this string str)
        {
            var parts = str.Split(':');

            // Workaround for Windows drive letter
            if (parts.Length > 1 && parts[0].Length == 1 && parts[1].StartsWith('/', '\\'))
            {
                var oldParts = parts;
                parts = new string[oldParts.Length - 1];
                parts[0] = oldParts[0] + ":" + oldParts[1];
                for (int i = 2; i < oldParts.Length; i++)
                    parts[i - 1] = oldParts[i];
            }

            return parts;
        }

        public static string GetPathComponent(this string str, int offset)
        {
            var parts = str.Split(Path.DirectorySeparatorChar);
            return offset < 0
                ? parts[parts.Length + offset]
                : parts[offset];
        }

        public static string Trailing(this string str, char c)
        {
            return string.IsNullOrEmpty(str) || str.EndsWith(c)
                ? str
                : str + c.ToString(CultureInfo.InvariantCulture);
        }

        public static string Truncate(this string str, int length)
        {
            return str != null && str.Length > length
                ? str.Substring(0, length)
                : str;
        }

        public static void AppendWhen(this StringBuilder sb, bool cond, char arg)
        {
            if (cond) sb.Append(arg);
        }

        public static void AppendWhen(this StringBuilder sb, bool cond, object arg)
        {
            if (cond) sb.Append(arg);
        }

        public static void AppendWhen(this StringBuilder sb, bool cond, string arg)
        {
            if (cond) sb.Append(arg);
        }

        public static void CommaWhen(this StringBuilder sb, bool cond)
        {
            if (cond) sb.Append(", ");
        }

        public static string NativeToUnix(this string str)
        {
            return Path.DirectorySeparatorChar != '/' && str != null
                ? str.Replace(Path.DirectorySeparatorChar, '/')
                : str;
        }

        public static string UnixToNative(this string str)
        {
            return Path.DirectorySeparatorChar != '/' && str != null
                ? str.Replace('/', Path.DirectorySeparatorChar)
                : str;
        }

        public static string UnixBaseName(this string f)
        {
            return f.Substring(f.LastIndexOf('/') + 1);
        }

        public static string UnixDirectoryName(this string f)
        {
            return f.Substring(0, Math.Max(0, f.LastIndexOf('/')));
        }

        public static string Printable(this string str)
        {
            return str.FirstLine().Trim().Quote();
        }

        public static string FirstLine(this string str)
        {
            var i = str.IndexOf('\n');
            return i != -1
                ? str.Substring(0, i)
                : str;
        }

        public static string Plural<T>(this string str, ICollection<T> collection)
        {
            return collection.Count == 1
                ? str
                : str + "s";
        }

        public static string Plural(this string str, int count)
        {
            return count == 1
                ? str
                : str + "s";
        }

        public static string Quote(this object obj, object arg = null)
        {
            return ("" + obj + arg).Quote();
        }

        public static string Quote(this string str)
        {
            return str == null
                ? "(null)"
                : str.IndexOf(' ') != -1 ||
                        str.Length == 0 ||
                        NeedsQuote(str[0]) ||
                        NeedsQuote(str[str.Length - 1]) ||
                        !char.IsUpper(str[0]) && str.IsIdentifier()
                    ? "'" + str + "'"
                    : str;
        }

        static bool NeedsQuote(char c)
        {
            // Avoid quoting full paths and qualified generic types
            return c != '/' && c != '>' && !char.IsLetterOrDigit(c);
        }

        public static string QuoteSpace(this string str)
        {
            var hasNonAlphaNumeric = false;
            foreach (var c in str ?? "")
                switch (c)
                {
                    // See if character is allowed in non-quoted string
                    case '.':
                    case '-':
                    case '_':
                    case '/':
                    case ':':
                    case '\\':
                        continue;
                    default:
                        if (c >= 'a' && c <= 'z' ||
                            c >= 'A' && c <= 'Z' ||
                            c >= '0' && c <= '9')
                            continue;

                        hasNonAlphaNumeric = true;
                        goto RETURN;
                }

        RETURN:
            return hasNonAlphaNumeric || string.IsNullOrEmpty(str)
                ? "\"" + (str ?? "").Replace("\"", "\\\"") + "\""
                : str;
        }

        public static string EscapeSpace(this string str)
        {
            if (str.IndexOf(' ') == -1)
                return str;

            var sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (c == ' ' || c == '\'')
                {
                    var bc = 0;
                    for (int j = i - 1; j >= 0 && str[j] == '\\'; j--)
                        bc++;
                    if ((bc & 2) == 0)
                        sb.Append('\\');
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        public static string EscapeCommand(this string str)
        {
            var q = '\0';
            var sb = new StringBuilder();

            foreach (var c in str.NativeToUnix())
            {
                switch (c)
                {
                    case '"':
                    case '\'':
                    {
                        if (q == c)
                            q = '\0';
                        else if (q == '\0')
                            q = c;
                        break;
                    }
                    case ' ':
                    {
                        if (q != '\0')
                            sb.Append('\\');
                        sb.Append(c);
                        break;
                    }
                    default:
                    {
                        sb.Append(c);
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        public static int UnquotedIndexOf(this string str, char c, int start = 0)
        {
            char q = '\0';

            for (int i = start; i < str.Length; i++)
            {
                if (str[i] == c && q == '\0')
                    return i;

                switch (str[i])
                {
                    case '\n':
                        // Reset quote mode on newline
                        q = '\0';
                        break;
                    case '"':
                    case '\'':
                    {
                        // Count backslashes
                        int bsCount = 0;
                        for (int bi = i - 1; bi >= 0 && str[bi] == '\\'; bi--)
                            bsCount++;

                        // Escape backslashes and set quote mode
                        if ((bsCount & 1) == 0)
                        {
                            if (q == str[i])
                                q = '\0';
                            else if (q == '\0')
                                q = str[i];
                        }

                        break;
                    }
                }
            }

            return -1;
        }

        static bool IsNegativeZero(double v)
        {
            return v == 0.0 && double.IsNegativeInfinity(1.0 / v);
        }

        public static string ToLiteral(this float f, bool suffix = true, bool minify = false)
        {
            if (float.IsNaN(f))
                return suffix ? "(.0f/.0f)" : "(.0/.0)";
            if (float.IsPositiveInfinity(f))
                return suffix ? "(1.f/0.f)" : "(1./0.)";
            if (float.IsNegativeInfinity(f))
                return suffix ? "(-1.f/0.f)" : "(-1./0.)";
            if (IsNegativeZero(f))
                return suffix ? "-0.f" : "-0.";

            var s = f.ToString("r", CultureInfo.InvariantCulture).ToLower();

            if (s.IndexOf('.') == -1 && s.IndexOf('e') == -1)
                s += ".0";

            if (minify)
            {
                if (s.StartsWith("0.", StringComparison.InvariantCulture))
                    s = s.Substring(1);
                else if (s.EndsWith(".0", StringComparison.InvariantCulture))
                    s = s.Substring(0, s.Length - 1);
            }

            if (suffix)
                s += "f";

            return s;
        }

        public static string ToLiteral(this double f)
        {
            if (double.IsNaN(f))
                return "(.0/.0)";
            if (double.IsPositiveInfinity(f))
                return "(1./0.)";
            if (double.IsNegativeInfinity(f))
                return "(-1./0.)";
            if (IsNegativeZero(f))
                return "-0.";

            var s = f.ToString("r", CultureInfo.InvariantCulture).ToLower();

            if (s.IndexOf('.') == -1 && s.IndexOf('e') == -1)
                s += ".0";

            return s;
        }

        public static string Escape(this char c, char q)
        {
            switch (c)
            {
                case '\'': return c == q ? "\\'" : "'";
                case '\"': return c == q ? "\\\"" : "\"";
                case '\\': return "\\\\";
                case '\0': return "\\0";
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
                case ' ': return " ";
                default: return
                    char.IsLetterOrDigit(c) ||
                    char.IsPunctuation(c) ||
                    char.IsSymbol(c)
                        ? c.ToString(CultureInfo.InvariantCulture)
                        : $"\\u{(int)c:X4}";
            }
        }

        public static string ToLiteral(this char c)
        {
            return "'" + c.Escape('\'') + "'";
        }

        public static string ToLiteral(this string str)
        {
            if (str == null)
                return "null";

            var sb = new StringBuilder();
            sb.Append("\"");

            foreach (var c in str)
                sb.Append(c.Escape('"'));

            sb.Append("\"");
            return sb.ToString();
        }

        public static string ToLiteral(this bool c)
        {
            return c ? "true" : "false";
        }

        public static string ToLiteral(this int c)
        {
            return c.ToString();
        }

        public static string ToLiteral(this Enum e)
        {
            return e.Equals(Enum.ToObject(e.GetType(), 0))
                // Return empty string when value is 0
                ? ""
                // Convert string to look better in disasm ('Public, Static' => 'public static')
                : e.ToString().ToLower().Replace(", ", " ");
        }

        public static string ToLiteral(this Enum e, bool space)
        {
            var str = e.ToLiteral();
            return space && str.Length > 0 ? str + " " : str;
        }

        public static string ToLiteral(this object obj)
        {
            if (obj is bool)
                return ((bool) obj).ToLiteral();
            if (obj is float)
                return ((float) obj).ToLiteral();
            if (obj is double)
                return ((double) obj).ToLiteral();
            if (obj is char)
                return ((char) obj).ToLiteral();
            if (obj == null || obj is string)
                return ((string) obj).ToLiteral();
            if (obj is Enum)
                return ((Enum) obj).ToLiteral();

            return obj.ToString();
        }

        public static string MagicString(this uint magic)
        {
            var a = (char)(magic >> 0 & 0xff);
            var b = (char)(magic >> 8 & 0xff);
            var c = (char)(magic >> 16 & 0xff);
            var d = (int)(magic >> 24 & 0xff);
            return (a.ToString() + b + c + d).ToLowerInvariant();
        }

        public static bool IsIdentifier(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsNumber(str[0]))
                return false;

            foreach (var c in str)
                if (c != '_' && !char.IsLetterOrDigit(c))
                    return false;

            return true;
        }

        public static string ToIdentifier(this string str, bool qualified = false)
        {
            if (str.IsIdentifier())
                return str;

            if (string.IsNullOrEmpty(str))
                return "_";

            if (!qualified && str.StartsWith('.'))
                return str.Substring(1).ToIdentifier() + "_";

            var sb = new StringBuilder();

            if (str.Length > 0 && char.IsDigit(str[0]))
                sb.Append('_');

            foreach (char c in str)
                if (char.IsLetterOrDigit(c) || c == '_' ||
                    qualified && c == '.')
                    sb.Append(c);

            if (sb.Length == 0 ||
                qualified && sb[sb.Length - 1] == '.')
                sb.Append('_');

            return sb.ToString();
        }

        public static int FindIdentifier(this string str, out string identifier, int start = 0)
        {
            // Set start to next letter or underscore
            while (start < str.Length && !char.IsLetter(str[start]) && str[start] != '_')
                start++;

            if (start < str.Length)
            {
                // Set end to next non-identifier character
                int end = start + 1;
                while (end < str.Length && (char.IsLetterOrDigit(str[end]) || str[end] == '_'))
                    end++;

                identifier = str.Substring(start, end - start);
                return start;
            }

            identifier = null;
            return -1;
        }

        public static string ReplaceWord(this string input, string from, string to)
        {
            return input == null
                    ? null :
                input == from
                    ? to
                    : Regex.Replace(input, "\\b" + Regex.Escape(from) + "\\b", to);
        }

        public static int NullableHashCode<T>(this T? self) where T : struct
        {
            return self == null
                ? -1
                : self.GetHashCode();
        }

        public static int NullableHashCode<T>(this T self) where T : class
        {
            return self?.GetHashCode() ?? -1;
        }
    }
}
