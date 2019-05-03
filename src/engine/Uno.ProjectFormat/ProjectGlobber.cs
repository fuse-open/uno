using System;
using System.Collections.Generic;
using System.IO;
using Uno.IO;
using Uno.Logging;

namespace Uno.ProjectFormat
{
    class ProjectGlobber
    {
        internal static void FindItems(
            Project project,
            IEnumerable<ProjectReference> includes,
            IEnumerable<SourceValue> excludes,
            List<ProjectReference> result,
            Log log = null,
            bool throwOnError = true,
            bool excludeItems = false)
        {
            new ProjectGlobber(project, result, log)
                .FindItems(includes, excludes, throwOnError, excludeItems);
        }

        readonly Source _src;
        readonly Log _log;
        readonly Disk _disk;
        readonly string _root;
        readonly List<ProjectReference> _result;
        readonly GlobList _includePatterns = new GlobList();
        readonly GlobList _excludePatterns = new GlobList();
        readonly HashSet<string> _excludePaths = new HashSet<string>();

        ProjectGlobber(Project project, List<ProjectReference> result, Log log = null)
        {
            _src = project.Source;
            _root = project.RootDirectory;
            _result = result;
            _log = log ?? Log.Default;
            _disk = new Disk(_log);
            SkipFile(Path.Combine(project.RootDirectory, "node_modules"));
            SkipFile(project.BuildDirectory);
            SkipFile(project.CacheDirectory);
            SkipFile(project.FullPath);
        }

        void FindItems(IEnumerable<ProjectReference> includes, IEnumerable<SourceValue> excludes, bool throwOnError, bool excludeItems)
        {
            var startErrorCount = _log.ErrorCount;

            if (_result.Count > 0)
            {
                // See if result contains any invalid items that should be removed
                var oldItems = new List<ProjectReference>(_result);
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
                    if (!e.ProjectPath.IsValidPath())
                        continue;

                    var fullName = Path.Combine(_root, e.ProjectPath.UnixToNative());

                    if (IsSkipped(fullName))
                    {
                        _log.Warning("rm " + e.ProjectPath + " (duplicate)");
                        continue;
                    }

                    if (!File.Exists(fullName) && !Directory.Exists(fullName))
                    {
                        _log.Warning("rm " + e.ProjectPath + " (missing)");
                        continue;
                    }

                    if (IsExluded(fullName))
                    {
                        _log.WriteLine("rm " + e.ProjectPath + " (exclude)");
                        continue;
                    }

                    Add(e);
                }

                if (excludeItems)
                    _excludePatterns.Clear();
            }

            foreach (var e in includes)
            {
                var fullName = e.ProjectPath;
                if (e.ProjectPath.IsValidPath() &&
                    _disk.GetFullPath(_src, ref fullName))
                    Add(fullName, e.Source);
                else
                    _includePatterns.Add(_root, e.ProjectPath, e.Source);
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
                var fullName = Path.Combine(dir, e);
                if (MatchFile(fullName, out src))
                    Add(fullName, src);
            }

            foreach (var e in Directory.EnumerateDirectories(dir))
            {
                var fullName = Path.Combine(dir, e);
                if (MatchDirectory(fullName))
                    VisitDirectory(fullName);
            }

            PopDirectory(id);
        }

        bool MatchFile(string fullName, out Source result)
        {
            if (IsSkipped(fullName))
                return Fail(out result);

            var unixName = fullName.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName))
                    return Fail(out result);

            foreach (var e in _includePatterns)
                if (e.Item1.IsMatch(unixName))
                    return Success(e.Item2, out result);

            return Fail(out result);
        }

        bool MatchDirectory(string fullName)
        {
            if (IsSkipped(fullName))
                return false;

            var unixName = fullName.NativeToUnix();

            foreach (var e in _excludePatterns)
                if (e.Item1.IsMatch(unixName) || e.Item1.IsMatch(unixName + "/"))
                    return false;

            foreach (var e in _includePatterns)
            {
                try
                {
                    if (e.Item1.IsMatch(unixName) || e.Item1.IsMatch(unixName + "/"))
                        return true;
                }
                catch
                {
                }
            }
            
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

        void Add(ProjectReference item)
        {
            _result.Add(item);
        }

        void Add(string fullName, Source src)
        {
            var name = fullName.ToRelativePath(_root, true);
            _log.Event(IOEvent.Include, name);
            var item = new ProjectReference(src, name.NativeToUnix());
            _result.Add(item);
        }

        void SkipFile(string fullName)
        {
            _excludePaths.Add(fullName.ToUpperInvariant());
        }

        bool IsSkipped(string fullName)
        {
            return _excludePaths.Contains(fullName.ToUpperInvariant());
        }

        static bool Success<A>(A a, out A aa)
        {
            aa = a;
            return true;
        }

        static bool Fail<A>(out A aa)
        {
            aa = default(A);
            return false;
        }
    }
}
