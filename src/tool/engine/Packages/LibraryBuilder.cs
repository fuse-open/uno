using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Collections;
using Uno.Configuration;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.Build.Packages
{
    public class LibraryBuilder : DiskObject
    {
        public bool Clean;
        public bool Express;
        public bool RebuildAll;
        public bool SilentBuild;
        public bool CanCache = true;
        public bool RebuiltListIsSourcePaths;
        public string Version;
        public BuildConfiguration? Configuration;
        public List<string> RebuildList;

        readonly ListDictionary<string, LibraryProject> _libMap = new ListDictionary<string, LibraryProject>();
        readonly HashSet<string> _dirty = new HashSet<string>();

        public LibraryBuilder(Log log)
            : base(log)
        {
            Express = Log.EnableExperimental;
        }

        public HashSet<string> GetSourceDirectories(UnoConfig config = null)
        {
            var sourceDirectories = new HashSet<string>();

            if (RebuiltListIsSourcePaths)
            {
                sourceDirectories.AddRange(RebuildList);
                return sourceDirectories;
            }

            var configSourcePaths = (config ?? UnoConfig.Current).GetFullPathArray("Packages.SourcePaths", "PackageSourcePaths");

            if (configSourcePaths.Length == 0)
            {
                Log.VeryVerbose("'Packages.SourcePaths' was not found in .unoconfig");
                return sourceDirectories;
            }

            foreach (var source in configSourcePaths)
            {
                if (!Directory.Exists(source))
                {
                    Log.VeryVerbose("Source directory " + source.ToRelativePath().Quote() + " was not found");
                    continue;
                }

                sourceDirectories.Add(source);
            }

            return sourceDirectories;
        }

        public void Build(UnoConfig config = null)
        {
            var startTime = Log.Time;
            var sourceDirectories = GetSourceDirectories(config);

            if (sourceDirectories == null)
                return;

            if (RebuildAll && Clean)
                foreach (var source in sourceDirectories)
                    Disk.DeleteDirectory(Path.Combine(
                                File.Exists(source)
                                    ? Path.GetDirectoryName(Path.GetFullPath(source))
                                    : source,
                                "build"));

            var allProjects = LoadProjects(sourceDirectories);
            var buildProjects = GetBuildList(allProjects);
            var packageCache = CanCache
                                    ? new PackageCache(Log, config)
                                    : null;

            try
            {
                var failed = new List<LibraryProject>();
                Log.Message("Found " + allProjects.Count + " projects, " + buildProjects.Count + " are out of date");

                for (var i = 0; i < buildProjects.Count; i++)
                {
                    Log.DisableSkip();
                    using (Log.StartAnimation("Building " + (i + 1) + "/" + buildProjects.Count + ": " + buildProjects[i].Project.Name, ConsoleColor.Cyan))
                        Build(buildProjects[i], packageCache, failed);
                }

                if (failed.Count == 1)
                    throw new Exception(failed[0] + ": Build failed");
                else if (failed.Count > 0)
                    throw new Exception(failed.Count + " projects failed to build:\n" + string.Join("\n", failed));
            }
            finally
            {
                Log.Message($"Completed in {Log.Time - startTime:0.00} seconds");
                packageCache?.Dispose();
            }
        }

        void Build(LibraryProject lib, PackageCache packageCache, List<LibraryProject> failed)
        {
            var buildLog = Log.GetQuieterLog(SilentBuild);

            if (Clean)
            {
                new ProjectCleaner(buildLog).Clean(lib.Project);
                Disk.DeleteDirectory(lib.PackageDirectory);
            }
            else if (Directory.Exists(lib.PackageDirectory))
            {
                // Remove old versions
                foreach (var dir in Directory.EnumerateDirectories(lib.PackageDirectory))
                    if (dir != lib.VersionDirectory)
                        Disk.DeleteDirectory(dir, true);

                Disk.DeleteDirectory(Path.Combine(lib.CacheDirectory));
            }

            var fail = TryGetFailedReference(lib, failed);
            if (fail != null)
            {
                Log.Message("Skipping because " + fail.Quote() + " failed to build");
                failed.Add(lib);
                return;
            }

            var result = new ProjectBuilder(
                    buildLog,
                    BuildTargets.Package,
                    new BuildOptions
                    {
                        Configuration = GetConfiguration(lib),
                        OutputDirectory = lib.VersionDirectory,
                        PackageCache = packageCache,
                        Force = true
                    })
                .Build(lib.Project);

            if (result.ErrorCount > 0)
                failed.Add(lib);
        }

        LibraryProject TryGetFailedReference(LibraryProject lib, List<LibraryProject> failed)
        {
            if (failed.Count > 0)
                foreach (var e in lib.References)
                    foreach (var dependency in _libMap.GetList(e))
                        if (failed.Contains(dependency))
                            return dependency;

            return null;
        }

        BuildConfiguration GetConfiguration(LibraryProject lib)
        {
            if (Configuration != null)
                return Configuration.Value;

            try
            {
                if (File.Exists(lib.ConfigFile))
                    return lib.Configuration;
            }
            catch (Exception e)
            {
                Log.Trace(e);
                Log.Warning(lib.ConfigFile.ToRelativePath() + ": " + e.Message);
            }

            return BuildConfiguration.Debug;
        }

        List<LibraryProject> LoadProjects(IEnumerable<string> sourceDirectories)
        {
            var visited = new HashSet<string>();
            var result = new List<LibraryProject>();

            foreach (var source in sourceDirectories)
            {
                if (visited.Contains(source))
                    continue;

                visited.Add(source);

                if (File.Exists(source))
                {
                    LoadProject(source, Path.GetDirectoryName(Path.GetFullPath(source)), result);
                    continue;
                }

                foreach (var dir in Directory.EnumerateDirectories(source))
                    LoadProjects(dir, source, result);

                LoadProjects(source, source, result);
            }

            return result;
        }

        void LoadProjects(string dir, string rootDir, List<LibraryProject> result)
        {
            foreach (var file in Directory.EnumerateFiles(dir, "*.unoproj"))
                LoadProject(file, rootDir, result);
        }

        void LoadProject(string file, string rootDir, List<LibraryProject> result)
        {
            try
            {
                var project = Project.Load(file);

                if (!string.IsNullOrEmpty(Version))
                    project.Version = Version;

                result.Add(new LibraryProject(project, rootDir));
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load " + file.ToRelativePath() + ": " + e.Message, e);
            }
        }

        List<LibraryProject> GetBuildList(IReadOnlyList<LibraryProject> all)
        {
            var list = new List<LibraryProject>();
            var added = new HashSet<LibraryProject>();

            foreach (var lib in all)
                _libMap.Add(lib.ToUpperInvariant(), lib);

            var dirty = new HashSet<LibraryProject>(EnumerateDirty(all));

            foreach (var lib in dirty)
                AddDependenciesFirst(lib, list, added, dirty);

            if (RebuildList != null && !RebuiltListIsSourcePaths)
                foreach (var p in RebuildList)
                    if (!_libMap.ContainsKey(p.ToUpperInvariant()))
                        Log.Warning("Package " + p.Quote() + " was not found");

            return list;
        }

        void AddDependenciesFirst(LibraryProject lib, List<LibraryProject> list, HashSet<LibraryProject> added, HashSet<LibraryProject> dirty)
        {
            if (added.Contains(lib))
                return;

            added.Add(lib);

            // Only build dirty packages to avoid doing too much work
            if (!dirty.Contains(lib))
                return;

            foreach (var e in lib.References)
                foreach (var dependency in _libMap.GetList(e))
                    AddDependenciesFirst(dependency, list, added, dirty);

            list.Add(lib);
        }

        IEnumerable<LibraryProject> EnumerateDirty(IReadOnlyList<LibraryProject> all)
        {
            if (RebuildAll)
                return all;

            if (RebuildList != null && !RebuiltListIsSourcePaths)
                foreach (var p in RebuildList)
                    _dirty.Add(p.ToUpperInvariant());

            if (Express)
                return all.Where(IsDirty);

            foreach (var p in all.Where(IsDirty))
                _dirty.Add(p.ToUpperInvariant());

            for (int dirtyCount = 1; dirtyCount != 0; )
            {
                dirtyCount = 0;

                foreach (var p in all)
                {
                    if (!_dirty.Contains(p.ToUpperInvariant()) &&
                        HasDirtyDependency(p))
                    {
                        _dirty.Add(p.ToUpperInvariant());
                        dirtyCount++;
                    }
                }
            }

            return all.Where(p => _dirty.Contains(p.ToUpperInvariant()));
        }

        bool IsDirty(LibraryProject lib)
        {
            try
            {
                // Marked by command-line & EnumerateDirty()
                if (_dirty.Contains(lib.ToUpperInvariant()))
                    return true;

                // Check if a build with a different version number exists, possibly
                // the project is already built using 'uno doctor --version=X.Y.Z'.
                LibraryProject existing;
                if (string.IsNullOrEmpty(Version) && lib.TryGetExistingBuild(out existing))
                    // Test the existing build and maybe we don't need to built it again.
                    lib = existing;

                if (!File.Exists(lib.PackageFile))
                {
                    Log.Event(IOEvent.Build, lib.Project.Name, "package not found");
                    return true;
                }

                if (Directory.EnumerateDirectories(lib.PackageDirectory).Count() > 1)
                {
                    Log.Event(IOEvent.Build, lib.Project.Name, "old version(s) found");
                    return true;
                }

                if (Configuration != null && (
                        !File.Exists(lib.ConfigFile) ||
                        File.ReadAllText(lib.ConfigFile).Trim() != Configuration.ToString()
                    ))
                {
                    Log.Event(IOEvent.Build, lib.Project.Name, "dirty config");
                    return true;
                }

                if (File.GetLastWriteTime(lib.Project.FullPath) > lib.LastBuildTime)
                {
                    Log.Event(IOEvent.Build, lib.Project.Name, "dirty project");
                    return true;
                }

                foreach (var e in lib.Project.GetFlattenedItems())
                {
                    var file = Path.Combine(lib.Project.RootDirectory, e.Value);
                    if (File.Exists(file) && File.GetLastWriteTime(file) > lib.LastBuildTime)
                    {
                        Log.Event(IOEvent.Build, lib.Project.Name, "dirty file: " + e.Value);
                        return true;
                    }
                }

                // Don't waste time on dependencies in express mode
                if (Express)
                    return false;

                foreach (var e in lib.References)
                {
                    foreach (var dependency in _libMap.GetList(e))
                    {
                        if (dependency.LastBuildTime > lib.LastBuildTime)
                        {
                            Log.Event(IOEvent.Build, lib.Project.Name, "dirty dependency: " + dependency.Project.Name);
                            Log.UltraVerbose("dependency: " + dependency.LastBuildTime + ", lib: " + lib.LastBuildTime);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Event(IOEvent.Build, lib.Project.Name, "exception: " + e.Message);
                return true;
            }

            return false;
        }

        bool HasDirtyDependency(LibraryProject lib)
        {
            try
            {
                foreach (var e in lib.References)
                {
                    if (_dirty.Contains(e))
                    {
                        Log.Event(IOEvent.Build, lib.Project.Name, "dirty reference: " + e);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Event(IOEvent.Build, lib.Project.Name, "exception: " + e.Message);
                return true;
            }

            return false;
        }
    }
}
