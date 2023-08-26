using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.UXL;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    public class SourceReader : DiskObject, IBuildInput
    {
        static readonly object AstKey = new object();
        static readonly object SourceFileCountKey = new object();

        readonly SourceBundle _bundle;
        readonly IFrontendEnvironment _env;
        readonly List<SourceBundle> _bundles = new List<SourceBundle>();
        readonly HashSet<string> _filenames = new HashSet<string>();

        public readonly AstCache AstCache;
        public readonly UxlCache UxlCache;

        public IReadOnlyList<SourceBundle> Bundles => _bundles;
        public SourceBundle Bundle => _bundle;
        public string CacheFile => Path.Combine(_bundle.CacheDirectory, "lib." + AstCache.MagicString);
        public bool CacheExists => File.Exists(CacheFile);

        public SourceReader(Log log, SourceBundle bundle, IFrontendEnvironment env)
            : base(log)
        {
            _bundle = bundle;
            _env = env;
            AstCache = new AstCache(log, _filenames);
            UxlCache = new UxlCache(log, _filenames);
            ResolveBundleOrder(bundle, new HashSet<SourceBundle>());
        }

        void ResolveBundleOrder(SourceBundle bundle, HashSet<SourceBundle> visited)
        {
            if (visited.Contains(bundle))
                return;

            visited.Add(bundle);

            // Add non-top-level bundles first
            foreach (var p in bundle.References)
                ResolveBundleOrder(p, visited);

            _bundles.Add(bundle);
        }

        public DateTime CacheTime
        {
            get
            {
                try
                {
                    return File.GetLastWriteTime(CacheFile);
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                    Log.Warning("Failed to get cache time for " + _bundle.Quote() + ": " + e.Message);
                    // Return something old
                    return new DateTime(2000, 1, 1);
                }
            }
        }

        public bool HasAnythingChangedSince(DateTime time, bool canCache = true)
        {
            try
            {
                foreach (var bundle in _bundles)
                {
                    if (!_env.Test(bundle.Source, bundle.BuildCondition))
                        continue;

                    if (bundle.IsCached && canCache)
                    {
                        // AST cache
                        if (File.GetLastWriteTime(
                                Path.Combine(
                                    bundle.CacheDirectory,
                                    "lib." + AstCache.MagicString)
                                ) >= time)
                            return true;
                    }
                    else
                    {
                        // Project file
                        if (File.GetLastWriteTime(bundle.Source.FullPath) >= time)
                            return true;
                        // Included files
                        foreach (var file in bundle.AllFiles)
                            if (File.GetLastWriteTime(Path.Combine(bundle.SourceDirectory, file.UnixPath)) >= time)
                                return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning("Checking timestamps: " + e.Message);
                Log.Trace(e);
                return true;
            }

            return false;
        }

        public List<AstDocument> ReadSourceFiles()
        {
            return ReadFilesParallel<AstDocument>(null, AddSourceFiles);
        }

        public List<UxlDocument> ReadExtensionsFiles(string backendName)
        {
            return ReadFilesParallel<UxlDocument>(backendName, AddExtensionsFiles);
        }

        List<T> ReadFilesParallel<T>(string arg, Action<SourceBundle, string, List<Task>, List<IEnumerable<T>>> addFiles)
        {
            var tasks = new List<Task>();
            var files = new List<IEnumerable<T>>();

            foreach (var bundle in _bundles)
                addFiles(bundle, arg, tasks, files);

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch
            {
                if (!Log.HasErrors)
                    throw;
            }

            var result = new List<T>();
            files.ForEach(result.AddRange);
            return result;
        }

        void AddSourceFiles(SourceBundle bundle, string arg, List<Task> tasks, List<IEnumerable<AstDocument>> files)
        {
            // 1) See if SourceBundle already has IL cached in memory, and skip the AST.
            //    This is normally used to speed up `uno doctor`.

            if (_env.CanCacheIL && bundle.Cache.ContainsKey(SourceBundle.ILKey))
            {
                Log.UltraVerbose("Using IL from memory cache in " + bundle);
                return;
            }

            // 2) See if SourceBundle already has AST cached in memory,
            //    without any new source files added by UX compiler

            var bundleSourceFileCount = !bundle.IsCached || _env.Test(bundle.Source, bundle.BuildCondition)
                ? bundle.SourceFiles.Count
                : 0;
            int cachedSourceFileCount;
            List<AstDocument> result;
            if (bundle.TryGetCache(AstKey, out result) &&
                bundle.TryGetCache(SourceFileCountKey, out cachedSourceFileCount) &&
                cachedSourceFileCount == bundleSourceFileCount)
            {
                Log.UltraVerbose("Using AST from memory cache in " + bundle);
                files.Add(result);
                return;
            }

            // 3) If no cache was available we must load from disk

            result = new List<AstDocument>();
            bundle.Cache[AstKey] = result;
            files.Add(result);

            if (bundle.IsCached)
            {
                if (!_env.Test(bundle.Source, bundle.BuildCondition))
                {
                    // Add empty namespaces when skipping a bundle.
                    // This is needed for not breaking any 'using <skipped-bundle>;' directives.

                    var root = new AstDocument(bundle.Source);

                    lock (result)
                        result.Add(root);

                    foreach (var ns in bundle.CachedNamespaces)
                    {
                        var parent = (AstNamespace) root;
                        foreach (var p in ns.Split('.'))
                        {
                            var ast = new AstNamespace(new AstIdentifier(bundle.Source, p));
                            parent.Namespaces.Add(ast);
                            parent = ast;
                        }
                    }

                    bundle.Cache[SourceFileCountKey] = 0;
                    return;
                }

                BeginTask(tasks,
                    () =>
                    {
                        try
                        {
                            AstCache.Deserialize(bundle, Path.Combine(bundle.CacheDirectory, "lib." + AstCache.MagicString), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error(bundle.Source, ErrorCode.E0000, "Failed to load AST cache: " + e.Message);
                        }
                    });
            }
            else
            {
                foreach (var rf in bundle.SourceFiles)
                    if (_env.Test(_bundle.Source, rf.Condition))
                        BeginTask(tasks, () => AstCache.Load(bundle, rf.UnixPath, result));
            }

            bundle.Cache[SourceFileCountKey] = bundle.SourceFiles.Count;
        }

        void AddExtensionsFiles(SourceBundle bundle, string backendName, List<Task> tasks, List<IEnumerable<UxlDocument>> files)
        {
            // 1) See if SourceBundle already has UXL cached in memory

            List<UxlDocument> result;
            if (bundle.TryGetCache(backendName, out result))
            {
                Log.UltraVerbose("Using UXL from memory cache in " + bundle);
                files.Add(result);
                return;
            }

            // 2) If no cache was available we must load from disk

            result = new List<UxlDocument>();
            bundle.Cache[backendName] = result;
            files.Add(result);

            if (bundle.IsCached)
            {
                if (backendName == null ||
                    !_env.Test(bundle.Source, bundle.BuildCondition) ||
                    !bundle.CachedExtensionsBackends.Contains(backendName))
                    return;

                BeginTask(tasks,
                    () =>
                    {
                        try
                        {
                            UxlCache.Deserialize(bundle, Path.Combine(bundle.CacheDirectory, backendName + "." + UxlCache.MagicString), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error(bundle.Source, ErrorCode.E0000, "Failed to load UXL cache: " + e.Message);
                        }
                    });
            }
            else
            {
                foreach (var rf in bundle.ExtensionsFiles)
                    if (_env.Test(_bundle.Source, rf.Condition))
                        BeginTask(tasks, () => UxlCache.Load(bundle, rf.UnixPath, result));
            }
        }

        void BeginTask(List<Task> tasks, Action task)
        {
            if (_env.Parallel)
                tasks.Add(Task.Factory.StartNew(task));
            else
                task();
        }

        public void ExportCache(string outputDir)
        {
            ExportCache(outputDir, new HashSet<string>(), new HashSet<string>());
        }

        public void ExportCache(string outputDir, HashSet<string> resultBackends, HashSet<string> resultNamespaces)
        {
            var ast = new List<AstDocument>();
            var uxl = new List<UxlDocument>();
            var uxlMap = new ListDictionary<UxlBackendType, UxlDocument>();

            foreach (var rf in _bundle.SourceFiles)
                if (_env.Test(_bundle.Source, rf.Condition))
                    AstCache.Load(_bundle, rf.UnixPath, ast);

            foreach (var rf in _bundle.ExtensionsFiles)
                if (_env.Test(_bundle.Source, rf.Condition))
                    UxlCache.Load(_bundle, rf.UnixPath, uxl);

            foreach (var e in uxl)
            {
                uxlMap.Add(e.Backend, e);
                resultBackends.Add(e.Backend.ToString());
            }

            WriteAst(ast, outputDir, "lib", AstSerializationFlags.OptimizeSources);

            foreach (var e in uxlMap)
                WriteUxl(e.Value, outputDir, e.Key.ToString());

            foreach (var ns in ast)
                FindNamespaces(ns.Namespaces, resultNamespaces);
        }

        void FindNamespaces(IEnumerable<AstNamespace> ast, HashSet<string> resultNamespaces, string prefix = null)
        {
            foreach (var ns in ast)
            {
                var name = prefix + ns.Name.Symbol;
                resultNamespaces.Add(name);
                FindNamespaces(ns.Namespaces, resultNamespaces, name + ".");
            }
        }

        void WriteAst(IEnumerable<AstDocument> ast, string outputDir, string basename, AstSerializationFlags flags)
        {
            Disk.CreateDirectory(outputDir);
            var filename = Path.Combine(outputDir, basename + "." + AstCache.MagicString);
            Log.Event(IOEvent.Write, filename);
            ast.Serialize(_bundle, filename, flags);
        }

        void WriteUxl(IEnumerable<UxlDocument> uxl, string outputDir, string basename)
        {
            Disk.CreateDirectory(outputDir);
            var filename = Path.Combine(outputDir, basename + "." + UxlCache.MagicString);
            Log.Event(IOEvent.Write, filename);
            uxl.Serialize(_bundle, filename);
        }
    }
}
