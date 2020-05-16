using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Minimatch;
using Uno.IO;

namespace Uno.ProjectFormat
{
    public static class ProjectFiles
    {
        public static List<string> GetProjectFiles(this ICollection<string> patterns, bool recursive = false)
        {
            var result = new List<string>();

            foreach (var p in patterns)
            {
                var pattern = p ?? "";

                if (recursive)
                    pattern = pattern.NativeToUnix().Trailing('/') + "**";

                if (pattern.IsValidPath() && (
                        File.Exists(pattern) ||
                        Directory.Exists(pattern)
                    ))
                {
                    result.Add(GetProjectFile(pattern));
                    continue;
                }

                if (pattern.IndexOf('*') == -1)
                    throw new FileNotFoundException("No such file or directory: " + pattern);

                if (pattern.IndexOf("..", StringComparison.Ordinal) != -1)
                    throw new Exception("'..' is not supported in glob pattern: " + p);

                while (pattern.StartsWith("./") || pattern.StartsWith(".\\"))
                    pattern = pattern.Substring(2);

                var glob = new Minimatcher(pattern, new Options { IgnoreCase = true, AllowWindowsPaths = true });
                var root = Directory.GetCurrentDirectory();

                foreach (var e in Directory.EnumerateFiles(
                            root, "*.unoproj",
                            pattern.Contains("**")
                                ? SearchOption.AllDirectories
                                : SearchOption.TopDirectoryOnly))
                    if (glob.IsMatch(e.Substring(root.Length + 1)))
                        result.Add(e);
            }

            if (result.Count == 0)
                throw new FileNotFoundException("No project".Plural(patterns) + " matching: " + string.Join(", ", patterns));

            return result;
        }

        public static string GetProjectFile(this string path)
        {
            path = path.ToFullPath();

            if (File.Exists(path))
            {
                switch (Path.GetExtension(path.ToUpperInvariant()))
                {
                    case ".UNO":
                    case ".UNOTRACE":
                    case ".UXL":
                    case ".UX":
                        for (var dir = path; ; )
                        {
                            dir = Path.GetDirectoryName(dir);

                            if (dir == null)
                                throw new FileNotFoundException("No projects were found in a parent directory of " + path.Quote());

                            var proj = GetProjectInDirectory(dir);
                            if (proj != null) return proj;
                        }
                    default:
                        return path;
                }
            }

            if (Directory.Exists(path))
            {
                var proj = GetProjectInDirectory(path);
                if (proj != null) return proj;

                throw new FileNotFoundException("No projects were found in " + path.Quote());
            }

            throw new FileNotFoundException("No such file or directory: " + path);
        }

        static string GetProjectInDirectory(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*.unoproj", SearchOption.TopDirectoryOnly).ToArray();

            switch (files.Length)
            {
                case 1:
                    return files[0];
                case 0:
                    return null;
                default:
                    throw new InvalidOperationException("More than one project were found in directory " + dir.Quote());
            }
        }

        public static void Add(this List<PackageReference> list, string package)
        {
            list.Add(new PackageReference(Source.Unknown, package));
        }

        public static void Add(this List<IncludeItem> list, string pattern)
        {
            list.Add(new IncludeItem(Source.Unknown, IncludeItemType.Glob, pattern));
        }
    }
}
