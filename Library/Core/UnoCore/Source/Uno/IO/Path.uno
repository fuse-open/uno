using Uno.Compiler.ExportTargetInterop;
using Uno.Text;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.Path")]
    public static class Path
    {
        public static readonly char DirectorySeparatorChar = GetDirectorySeparatorChar();
        public static readonly char AltDirectorySeparatorChar = '/';
        public static readonly char PathSeparator = GetPathSeparator();
        public static readonly char VolumeSeparatorChar = GetVolumeSeparatorChar();

        static readonly char[] DirectorySeparatorChars = GetDirectorySeparatorChars();

        static bool IsDirectorySeparator(char ch)
        {
            return ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar;
        }

        static string CombineInternal(string a, string b)
        {
            if (b.Length == 0)
                return a;

            if (a.Length == 0)
                return b;

            if (IsPathRooted(b))
                return b;

            if (IsDirectorySeparator(a[a.Length - 1]))
                return a + b;

            return a + DirectorySeparatorChar + b;
        }

        public static string Combine(string path1, string path2)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1));
            if (path2 == null)
                throw new ArgumentNullException(nameof(path2));

            return CombineInternal(path1, path2);
        }

        public static string Combine(string path1, string path2, string path3)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1));
            if (path2 == null)
                throw new ArgumentNullException(nameof(path2));
            if (path3 == null)
                throw new ArgumentNullException(nameof(path3));

            return CombineInternal(CombineInternal(path1, path2), path3);
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1));
            if (path2 == null)
                throw new ArgumentNullException(nameof(path2));
            if (path3 == null)
                throw new ArgumentNullException(nameof(path3));
            if (path4 == null)
                throw new ArgumentNullException(nameof(path4));

            return CombineInternal(CombineInternal(CombineInternal(path1, path2), path3), path4);
        }

        public static string Combine(params string[] parts)
        {
            if (parts == null)
                throw new ArgumentNullException(nameof(parts));

            string result = "";

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part == null)
                    throw new ArgumentNullException(nameof(parts));

                result = Path.CombineInternal(result, part);
            }

            return result;
        }

        static string NormalizePathSeparators(string path)
        {
            int length = path.Length;
            int pos = 0;

            var sb = new StringBuilder();
            for (; pos < length; ++pos)
            {
                char ch = path[pos];
                if (!IsDirectorySeparator(ch))
                {
                    sb.Append(ch);
                    continue;
                }

                // collapse consecutive directory separators
                while ((pos + 1) < length && IsDirectorySeparator(path[pos + 1]))
                    pos++;
                sb.Append(DirectorySeparatorChar);
            }

            return sb.ToString();
        }

        public static string GetDirectoryName(string path)
        {
            if (path == null)
                return null;

            path = NormalizePathSeparators(path);

            var lastDirectorySeparator = path.LastIndexOfAny(DirectorySeparatorChars);
            if (lastDirectorySeparator < 0)
                return string.Empty;

            if (lastDirectorySeparator == 0)
            {
                if (path.Length == 1)
                    return null;

                lastDirectorySeparator = 1;
            }

            if defined(WIN32)
            {
                if (lastDirectorySeparator == 2 && path[1] == VolumeSeparatorChar)
                    return null;
            }

            return path.Substring(0, lastDirectorySeparator);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            path = GetFileName(path);
            if (path == null)
                return null;

            var pos = path.LastIndexOf('.');
            if (pos < 0)
                return path;

            return path.Substring(0, pos);
        }

        public static string GetFileName(string path)
        {
            if (path == null)
                return null;

            var lastDirectorySeparator = path.LastIndexOfAny(DirectorySeparatorChars);
            if (lastDirectorySeparator >= 0)
                return path.Substring(lastDirectorySeparator + 1);

            return path;
        }

        public static string GetExtension(string filename)
        {
            if (filename == null)
                return null;

            for (int i = filename.Length - 1; i >= 0; i--)
            {
                var ch = filename[i];
                if (IsDirectorySeparator(ch) || ch == VolumeSeparatorChar)
                    break;

                if (ch == '.')
                    return filename.Substring(i);
            }

            return "";
        }

        public static string GetFullPath(string filename)
        {
            return IsPathRooted(filename)
                ? filename
                : Path.Combine(Directory.GetCurrentDirectory(), filename);
        }

        public static bool HasExtension(string filename)
        {
            if (filename == null)
                return false;

            for (int i = filename.Length - 1; i >= 0; i--)
            {
                var ch = filename[i];
                if (IsDirectorySeparator(ch) || ch == VolumeSeparatorChar)
                    return false;

                if (ch == '.')
                    return true;
            }

            return false;
        }

        public static bool IsPathRooted(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (IsDirectorySeparator(path[0]))
                return true;

            if defined(WIN32)
                return path.Length > 1 && path[1] == VolumeSeparatorChar;
            else
                return false;
        }

        [extern(APPLE) Foreign(Language.ObjC)]
        [extern(WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static string GetTempPath()
        {
            if defined(WIN32)
            @{
                WCHAR path[4096];
                GetTempPathW(sizeof(path), path);
                return uString::Utf16((const char16_t*) path);
            @}
            else if defined(APPLE)
            @{
                return NSTemporaryDirectory();
            @}
            else if defined(CPLUSPLUS)
            @{
                const char* tmpdir = getenv("TMPDIR");
                if (!tmpdir || !strlen(tmpdir)) tmpdir = getenv("TEMP");
                if (!tmpdir || !strlen(tmpdir)) tmpdir = getenv("TEMPDIR");
                if (!tmpdir || !strlen(tmpdir)) tmpdir = getenv("TMP");
                if (!tmpdir || !strlen(tmpdir)) U_THROW_IOE("GetTempPath() failed");
                return uString::Utf8(tmpdir);
            @}
            else
                throw new NotImplementedException();
        }

        static char GetDirectorySeparatorChar()
        {
            return defined(WIN32)
                ? '\\'
                : '/';
        }

        static char[] GetDirectorySeparatorChars()
        {
            return defined(WIN32)
                ? new char[] { '\\', '/' }
                : new char[] {'/' };
        }

        static char GetPathSeparator()
        {
            return defined(WIN32)
                ? ';'
                : ':';
        }

        static char GetVolumeSeparatorChar()
        {
            return defined(WIN32)
                ? ':'
                : '/';
        }
    }
}
