using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.UXL;
using Uno.Compiler.Frontend.Analysis;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    public class SourceReader : DiskObject, IBuildInput
    {
        static readonly object AstKey = new object();
        static readonly object SourceFileCountKey = new object();

        readonly SourcePackage _upk;
        readonly IFrontendEnvironment _env;
        readonly List<SourcePackage> _packages = new List<SourcePackage>();
        readonly HashSet<string> _filenames = new HashSet<string>();

        public readonly AstCache AstCache;
        public readonly UxlCache UxlCache;

        public IReadOnlyList<SourcePackage> Packages => _packages;
        public SourcePackage Package => _upk;
        public string CacheFile => Path.Combine(_upk.CacheDirectory, "lib." + AstCache.MagicString);
        public bool CacheExists => File.Exists(CacheFile);

        public SourceReader(Log log, SourcePackage upk, IFrontendEnvironment env)
            : base(log)
        {
            _upk = upk;
            _env = env;
            AstCache = new AstCache(log, _filenames);
            UxlCache = new UxlCache(log, _filenames);
            ResolvePackageOrder(upk, new HashSet<SourcePackage>());
        }

        void ResolvePackageOrder(SourcePackage upk, HashSet<SourcePackage> visited)
        {
            if (visited.Contains(upk))
                return;

            visited.Add(upk);

            // Add non-top-level packages first
            foreach (var p in upk.References)
                ResolvePackageOrder(p, visited);

            _packages.Add(upk);
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
                    Log.Warning("Failed to get cache time for " + _upk.Quote() + ": " + e.Message);
                    // Return something old
                    return new DateTime(2000, 1, 1);
                }
            }
        }

        public bool HasAnythingChangedSince(DateTime time, bool canCache = true)
        {
            try
            {
                foreach (var upk in _packages)
                {
                    if (!_env.Test(upk.Source, upk.BuildCondition))
                        continue;

                    if (upk.IsCached && canCache)
                    {
                        // AST cache
                        if (File.GetLastWriteTime(
                                Path.Combine(
                                    upk.CacheDirectory,
                                    "lib." + AstCache.MagicString)
                                ) >= time)
                            return true;
                    }
                    else
                    {
                        // Project file
                        if (File.GetLastWriteTime(upk.Source.FullPath) >= time)
                            return true;
                        // Included files
                        foreach (var file in upk.AllFiles)
                            if (File.GetLastWriteTime(Path.Combine(upk.SourceDirectory, file.UnixPath)) >= time)
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

        List<T> ReadFilesParallel<T>(string arg, Action<SourcePackage, string, List<Task>, List<IEnumerable<T>>> addFiles)
        {
            var tasks = new List<Task>();
            var files = new List<IEnumerable<T>>();

            foreach (var upk in _packages)
                addFiles(upk, arg, tasks, files);

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

        void AddSourceFiles(SourcePackage upk, string arg, List<Task> tasks, List<IEnumerable<AstDocument>> files)
        {
            // 1) See if SourcePackage already has IL cached in memory, and skip the AST.
            //    This is normally used to speed up `uno doctor`.

            if (_env.CanCacheIL && upk.Cache.ContainsKey(SourcePackage.ILKey))
            {
                Log.UltraVerbose("Using IL from memory cache in " + upk);
                return;
            }

            // 2) See if SourcePackage already has AST cached in memory,
            //    without any new source files added by UX compiler

            var upkSourceFileCount = !upk.IsCached || _env.Test(upk.Source, upk.BuildCondition)
                ? upk.SourceFiles.Count
                : 0;
            int cachedSourceFileCount;
            List<AstDocument> result;
            if (upk.TryGetCache(AstKey, out result) &&
                upk.TryGetCache(SourceFileCountKey, out cachedSourceFileCount) &&
                cachedSourceFileCount == upkSourceFileCount)
            {
                Log.UltraVerbose("Using AST from memory cache in " + upk);
                files.Add(result);
                return;
            }

            // 3) If no cache was available we must load from disk

            result = new List<AstDocument>();
            upk.Cache[AstKey] = result;
            files.Add(result);

            if (upk.IsCached)
            {
                if (!_env.Test(upk.Source, upk.BuildCondition))
                {
                    // Add empty namespaces when skipping a package.
                    // This is needed for not breaking any 'using <skipped-package>;' directives.

                    var root = new AstDocument(upk.Source);

                    lock (result)
                        result.Add(root);

                    foreach (var ns in upk.CachedNamespaces)
                    {
                        var parent = (AstNamespace) root;
                        foreach (var p in ns.Split('.'))
                        {
                            var ast = new AstNamespace(new AstIdentifier(upk.Source, p));
                            parent.Namespaces.Add(ast);
                            parent = ast;
                        }
                    }

                    upk.Cache[SourceFileCountKey] = 0;
                    return;
                }

                BeginTask(tasks,
                    () =>
                    {
                        try
                        {
                            AstCache.Deserialize(upk, Path.Combine(upk.CacheDirectory, "lib." + AstCache.MagicString), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error(upk.Source, ErrorCode.E0000, "Failed to load AST cache: " + e.Message);
                        }
                    });
            }
            else
            {
                foreach (var rf in upk.SourceFiles)
                    if (_env.Test(_upk.Source, rf.Condition))
                        BeginTask(tasks, () => AstCache.Load(upk, rf.UnixPath, result));
            }

            upk.Cache[SourceFileCountKey] = upk.SourceFiles.Count;
        }

        void AddExtensionsFiles(SourcePackage upk, string backendName, List<Task> tasks, List<IEnumerable<UxlDocument>> files)
        {
            // 1) See if SourcePackage already has UXL cached in memory

            List<UxlDocument> result;
            if (upk.TryGetCache(backendName, out result))
            {
                Log.UltraVerbose("Using UXL from memory cache in " + upk);
                files.Add(result);
                return;
            }

            // 2) If no cache was available we must load from disk

            result = new List<UxlDocument>();
            upk.Cache[backendName] = result;
            files.Add(result);

            if (upk.IsCached)
            {
                if (backendName == null ||
                    !_env.Test(upk.Source, upk.BuildCondition) ||
                    !upk.CachedExtensionsBackends.Contains(backendName))
                    return;

                BeginTask(tasks,
                    () =>
                    {
                        try
                        {
                            UxlCache.Deserialize(upk, Path.Combine(upk.CacheDirectory, backendName + "." + UxlCache.MagicString), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error(upk.Source, ErrorCode.E0000, "Failed to load UXL cache: " + e.Message);
                        }
                    });
            }
            else
            {
                foreach (var rf in upk.ExtensionsFiles)
                    if (_env.Test(_upk.Source, rf.Condition))
                        BeginTask(tasks, () => UxlCache.Load(upk, rf.UnixPath, result));
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

            foreach (var rf in _upk.SourceFiles)
                if (_env.Test(_upk.Source, rf.Condition))
                    AstCache.Load(_upk, rf.UnixPath, ast);

            foreach (var rf in _upk.ExtensionsFiles)
                if (_env.Test(_upk.Source, rf.Condition))
                    UxlCache.Load(_upk, rf.UnixPath, uxl);

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
            ast.Serialize(_upk, filename, flags);
        }

        void WriteUxl(IEnumerable<UxlDocument> uxl, string outputDir, string basename)
        {
            Disk.CreateDirectory(outputDir);
            var filename = Path.Combine(outputDir, basename + "." + UxlCache.MagicString);
            Log.Event(IOEvent.Write, filename);
            uxl.Serialize(_upk, filename);
        }
    }
}
