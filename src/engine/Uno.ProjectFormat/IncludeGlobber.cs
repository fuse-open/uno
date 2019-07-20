using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Configuration.Format;
using Uno.IO;
using Uno.Logging;

namespace Uno.ProjectFormat
{
    class IncludeGlobber
    {
        internal static void FindItems(
            Project project,
            IEnumerable<IncludeItem> includes,
            IEnumerable<SourceValue> excludes,
            List<IncludeItem> result,
            Log log = null,
            bool throwOnError = true,
            bool excludeItems = false)
        {
            new IncludeGlobber(project, result, log)
                .FindItems(includes, excludes, throwOnError, excludeItems);
        }

        readonly Source _src;
        readonly Log _log;
        readonly Disk _disk;
        readonly string _root;
        readonly List<IncludeItem> _result;
        readonly GlobList _includePatterns = new GlobList();
        readonly GlobList _excludePatterns = new GlobList();
        readonly HashSet<string> _excludePaths = new HashSet<string>();

        IncludeGlobber(Project project, List<IncludeItem> result, Log log = null)
        {
            _src = project.Source;
            _root = project.RootDirectory;
            _result = result;
            _log = log ?? Log.Default;
            _disk = new Disk(_log);
            SkipFile(Path.Combine(project.RootDirectory, "node_modules"));
            SkipFile(project.BuildDirectory);
            SkipFile(project.CacheDirectory);
        }

        void FindItems(IEnumerable<IncludeItem> includes, IEnumerable<SourceValue> excludes, bool throwOnError, bool excludeItems)
        {
            var startErrorCount = _log.ErrorCount;

            if (_result.Count > 0)
            {
                // See if result contains any invalid items that should be removed
                var oldItems = new List<IncludeItem>(_result);
                _result.Clear();

                if (excludeItems)
                {
                    foreach (var e in excludes)
                        _excludePatterns.Add(_root, e.String, e.Source);

                    ExcludeDirectory(_root);
                }

                foreach (var e in oldItems)
                {
                    // Keep glob patterns
                    if (!e.Value.IsValidPath())
                        continue;

                    var fullName = Path.Combine(_root, e.Value.UnixToNative());

                    if (IsSkipped(fullName))
                    {
                        _log.Warning("rm " + e.Value + " (duplicate)");
                        continue;
                    }

                    if (!File.Exists(fullName) && !Directory.Exists(fullName))
                    {
                        _log.Warning("rm " + e.Value + " (missing)");
                        continue;
                    }

                    if (IsExluded(fullName))
                    {
                        _log.WriteLine("rm " + e.Value + " (exclude)");
                        continue;
                    }

                    Add(fullName, e);
                }

                if (excludeItems)
                    _excludePatterns.Clear();
            }

            foreach (var e in includes)
            {
                var fullName = e.Value;
                if (e.Value.IsValidPath() &&
                    _disk.GetFullPath(_src, ref fullName, PathFlags.AllowNonExistingPath))
                    Add(fullName, e.Type, e.Source, e.Condition);
                else
                    _includePatterns.Add(_root, e.Value, e.Source, e.Type, e.Condition);
            }

            foreach (var e in excludes)
            {
                if (e.String.IsValidPath())
                {
                    var fullName = Path.Combine(_root, e.String.UnixToNative());
                    if (Directory.Exists(fullName) || File.Exists(fullName))
                        SkipFile(fullName);
                }

                _excludePatterns.Add(_root, e.String, e.Source);
            }

            if (_includePatterns.Count > 0)
                VisitDirectory(_root);

            if (throwOnError && _log.ErrorCount > startErrorCount)
                throw new FormatException("Failed to load project because of missing files");
        }

        bool IsExluded(string fullName)
        {
            var unixName = fullName.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName))
                    return true;

            return false;
        }

        void ExcludeDirectory(string dir)
        {
            var unixName = dir.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName))
                    return;

            PushDirectory(dir);

            foreach (var e in Directory.EnumerateDirectories(dir))
                ExcludeDirectory(Path.Combine(dir, e));
        }

        void VisitDirectory(string dir)
        {
            var id = PushDirectory(dir);

            foreach (var e in Directory.EnumerateFiles(dir))
            {
                Source src;
                IncludeItemType type;
                string cond;
                var fullName = Path.Combine(dir, e);
                if (MatchFile(fullName, out src, out type, out cond))
                    Add(fullName, type, src, cond);
            }

            foreach (var e in Directory.EnumerateDirectories(dir))
            {
                var fullName = Path.Combine(dir, e);
                if (MatchDirectory(fullName))
                    VisitDirectory(fullName);
            }

            PopDirectory(id);
        }

        bool MatchFile(string fullName, out Source result, out IncludeItemType type, out string cond)
        {
            switch (Path.GetExtension(fullName).ToUpperInvariant())
            {
                case ".UNOCONFIG":
                case ".UNOPROJ":
                case ".UNOSLN":
                    return Fail(out result, out type, out cond);
            }

            if (IsSkipped(fullName))
                return Fail(out result, out type, out cond);

            var unixName = fullName.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName))
                    return Fail(out result, out type, out cond);

            foreach (var e in _includePatterns)
                if (e.Item1.IsMatch(unixName))
                    return Success(e.Item2, e.Item3, e.Item4, out result, out type, out cond);

            return Fail(out result, out type, out cond);
        }

        bool MatchDirectory(string fullName)
        {
            if (IsSkipped(fullName) ||
                Directory.EnumerateFiles(fullName, "*.unoproj", SearchOption.TopDirectoryOnly).Count() != 0)
                return false;

            var unixName = fullName.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName) || e.Item1.IsMatch(unixName + "/"))
                    return false;

            foreach (var e in _includePatterns)
                if (e.Item1.IsMatch(unixName) || e.Item1.IsMatch(unixName + "/"))
                    return true;

            // Skip dot directories unless explicit include pattern was specified
            return !Path.GetFileName(fullName).StartsWith('.');
        }

        int PushDirectory(string dir)
        {
            var retval = _excludePatterns.Count;

            // Ignore paths listed in .unoignore file
            var ignoreFile = Path.Combine(dir, ".unoignore");
            if (File.Exists(ignoreFile))
            {
                _log.Event(IOEvent.Read, ignoreFile);
                foreach (var f in File.ReadAllLines(ignoreFile))
                    _excludePatterns.Add(dir, f);
            }

            return retval;
        }

        void PopDirectory(int val)
        {
            if (val < _excludePatterns.Count)
                _excludePatterns.RemoveRange(val, _excludePatterns.Count - val);
        }

        void Add(string fullName, IncludeItem item)
        {
            _result.Add(item);

            // Ignore added files to avoid duplicates
            if (item.Type != IncludeItemType.Folder)
                SkipFile(fullName);

            // Ignore unpacked stuff directories
            if (item.Type == IncludeItemType.Stuff && File.Exists(fullName))
                SkipStuff(fullName);
        }

        void Add(string fullName, IncludeItemType type, Source src, string cond)
        {
            var name = fullName.ToRelativePath(_root, true);
            if (type == IncludeItemType.Glob)
                type = GetIncludeItemType(name);

            _log.Event(IOEvent.Include, name, "type: " + type);
            var item = new IncludeItem(src, type, name.NativeToUnix(), cond);
            _result.Add(item);

            // Ignore added files to avoid duplicates
            if (item.Type != IncludeItemType.Folder)
                SkipFile(fullName);

            // Ignore unpacked stuff directories
            if (type == IncludeItemType.Stuff && File.Exists(fullName))
                SkipStuff(fullName);
        }

        void SkipStuff(string fullName)
        {
            var parentDir = Path.GetDirectoryName(fullName);

            // Ignore directories listed in *.stuff files
            foreach (var e in StuffObject.Load(fullName, StuffFlags.AcceptAll))
            {
                var stuffPath = Path.Combine(parentDir, e.Key.UnixToNative());
                _log.Event(IOEvent.Ignore, stuffPath, "stuff directory");
                SkipFile(stuffPath);
            }
        }

        void SkipFile(string fullName)
        {
            _excludePaths.Add(fullName.ToUpperInvariant());
        }

        bool IsSkipped(string fullName)
        {
            return _excludePaths.Contains(fullName.ToUpperInvariant());
        }

        static IncludeItemType GetIncludeItemType(string name)
        {
            switch (Path.GetExtension(name).ToUpperInvariant())
            {
                case ".UNO":
                    return IncludeItemType.Source;
                case ".UX":
                    return IncludeItemType.UX;
                case ".UXL":
                    return IncludeItemType.Extensions;
                case ".STUFF":
                    return IncludeItemType.Stuff;
                case ".TS":
                    return IncludeItemType.FuseJS;
                default:
                    return IncludeItemType.File;
            }
        }

        static bool Success<A, B, C>(A a, B b, C c, out A aa, out B bb, out C cc)
        {
            aa = a;
            bb = b;
            cc = c;
            return true;
        }

        static bool Fail<A, B, C>(out A aa, out B bb, out C cc)
        {
            aa = default(A);
            bb = default(B);
            cc = default(C);
            return false;
        }
    }
}