using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.FuseJS;
using Uno.Collections;
using Uno.Compiler;
using Uno.Configuration;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.Build.Libraries
{
    public class BundleCache : DiskObject
    {
        // For use by UXNinja & tests in Fuse X
        public static SourceBundle GetBundle(Log log, Project project)
        {
            // Passing null to disable FuseJS transpiler.
            return new BundleCache(log, null, null).GetBundle(project);
        }

        readonly SearchPaths _sourcePaths = new SearchPaths();
        readonly SearchPaths _searchPaths = new SearchPaths();
        readonly Dictionary<string, string> _locks = new Dictionary<string, string>();
        readonly Dictionary<string, SourceBundle> _cache = new Dictionary<string, SourceBundle>();
        readonly ListDictionary<string, DirectoryInfo> _library = new ListDictionary<string, DirectoryInfo>();
        readonly LazyTranspiler _transpiler;

        public IEnumerable<string> SearchPaths => _sourcePaths.Concat(_searchPaths).Where(Directory.Exists);

        public BundleCache()
            : this(null, null, null)
        {
        }

        public BundleCache(Log log, UnoConfig config, LazyTranspiler transpiler)
            : base(log ?? Log.Null)
        {
            _transpiler = transpiler;

            if (config == null)
                config = UnoConfig.Current;

            foreach (var src in config.GetFullPathArray("SearchPaths.Sources", "Packages.SourcePaths"))
                _sourcePaths.AddOnce(Path.Combine(
                    File.Exists(src)
                        ? Path.GetDirectoryName(src)
                        : src,
                    "build"));

            foreach (var src in config.GetFullPathArray("SearchPaths", "Packages.SearchPaths"))
                _searchPaths.AddOnce(src);
        }

        public SourceBundle GetBundle(string name, string version = null)
        {
            return GetBundle(new LibraryReference(Source.Unknown, name, version));
        }

        public SourceBundle GetBundle(Project project)
        {
            return GetBundle(project.Source, project, true);
        }

        public List<DirectoryInfo> GetVersionDirectories(string library)
        {
            List<DirectoryInfo> result;
            if (!_library.TryGetValue(library, out result))
            {
                result = new List<DirectoryInfo>();
                _library.Add(library, result);

                foreach (var feed in SearchPaths)
                {
                    DirectoryInfo dir;
                    if (!GetLibraryDirectories(feed).TryGetValue(library, out dir))
                        continue;

                    var versions = dir.GetDirectories();
                    Array.Sort(versions, (left, right) => VersionRange.Compare(right.Name, left.Name));

                    foreach (var version in versions)
                        if (ManifestFile.Exists(version.FullName))
                            result.Add(version);
                }
            }

            return result;
        }

        public Dictionary<string, DirectoryInfo> GetLibraryDirectories(string directory)
        {
            var result = new Dictionary<string, DirectoryInfo>();

            foreach (var library in new DirectoryInfo(directory).GetDirectories())
                result[library.Name] = library;

            return result;
        }

        public IEnumerable<DirectoryInfo> EnumerateLibraries(string name)
        {
            return _sourcePaths.EnumerateLibraryDirectories(name).Concat(
                   _searchPaths.EnumerateLibraryDirectories(name));
        }

        public IEnumerable<DirectoryInfo> EnumerateVersions(string name, string version = null)
        {
            return _sourcePaths.EnumerateVersionDirectories(name, version).Concat(
                   _searchPaths.EnumerateVersionDirectories(name, version));
        }

        SourceBundle GetBundle(Source src, Project project, bool startup = false)
        {
            SourceBundle result;
            if (_cache.TryGetValue(project.Name, out result))
            {
                if (result == null)
                {
                    Log.Error(src, null, "Circular reference to " + project.Name.Quote());
                    return new SourceBundle(project.Name);
                }

                if (result.Source.FullPath != project.FullPath || startup)
                {
                    Log.Error(src, null, "Multiple projects or libraries with the name " + project.Name.Quote());
                    return new SourceBundle(project.Name);
                }

                result.Flags &= ~SourceBundleFlags.Startup;
                return result;
            }

            result = project.CreateBundle(startup);
            _cache[project.Name] = null;

            if (project.Name != "UnoCore")
                result.References.Add(GetBundle(new LibraryReference(project.Source, "UnoCore")));

            foreach (var r in project.LibraryReferences)
                result.References.Add(GetBundle(r));
            foreach (var r in project.ProjectReferences)
                result.References.Add(GetBundle(r.Source,
                    LoadProject(r.Source, r.GetFullPath(project.RootDirectory))));

            // Transpile FuseJS files
            foreach (var f in project.FuseJSFiles)
            {
                // We don't need to spend time on this in UXNinja, tests, etc.
                if (_transpiler == null)
                    continue;

                var name = f.NativePath;
                var nameWithoutExt = Path.ChangeExtension(name, null);
                var inputFile = Path.Combine(project.RootDirectory, name);
                var outputFile = Path.Combine(project.RootDirectory, ".uno", "fusejs", nameWithoutExt + ".js");

                result.AdditionalFiles.Add(f); // Invalidate the project when a FuseJS file changes
                result.BundleFiles.Add(new FileItem(outputFile.ToRelativePath(project.RootDirectory, true).NativeToUnix(), f.Condition));

                if (!File.Exists(inputFile))
                {
                    Log.Error(src, ErrorCode.E0000, "File not found: " + inputFile.ToRelativePath());
                    continue;
                }

                if (File.Exists(outputFile) &&
                    File.GetLastWriteTime(outputFile) >= File.GetLastWriteTime(inputFile))
                    continue;

                name = inputFile.ToRelativePath();
                Log.Verbose("Transpiling " + name);

                string output;
                if (_transpiler.TryTranspile(name, File.ReadAllText(inputFile), out output))
                {
                    using (var file = Disk.CreateText(outputFile))
                        file.Write(output);
                }
            }

            result.FlattenTransitiveReferences();
            _cache[project.Name] = result;
            return result;
        }

        Project LoadProject(Source src, string filename)
        {
            if (!File.Exists(filename))
            {
                Log.Error(src, ErrorCode.E0100, "Project " + filename.Quote() + " was not found");
                return new Project(filename);
            }

            return Project.Load(filename);
        }

        SourceBundle GetBundle(LibraryReference reference)
        {
            SourceBundle result;
            if (_cache.TryGetValue(reference.LibraryName, out result))
            {
                if (result == null)
                {
                    Log.Error(reference.Source, null, "Circular reference to " + reference.Quote());
                    return new SourceBundle(reference.LibraryName, reference.LibraryVersion);
                }

                result.Flags |= SourceBundleFlags.Cached;
                result.Flags &= ~SourceBundleFlags.Startup;
                return result;
            }

            var manifest = GetFile(reference);
            if (manifest == null)
            {
                Log.Error(reference.Source, ErrorCode.E0100, "Library " + reference.Quote() + " was not found");
                return new SourceBundle(reference.LibraryName, reference.LibraryVersion);
            }

            result = manifest.CreateBundle();
            _cache[reference.LibraryName] = null;

            foreach (var r in manifest.References)
                result.References.Add(GetBundle(r));

            _cache[reference.LibraryName] = result;
            return result;
        }

        ManifestFile GetFile(LibraryReference reference)
        {
            var name = reference.LibraryName;
            var versionRange = VersionRange.Parse(reference.LibraryVersion);

            if (_sourcePaths.Count > 0)
            {
                var dirs = _sourcePaths.GetOrderedVersionDirectories(name);

                foreach (var dir in dirs)
                    if (versionRange.IsCompatible(dir.Name))
                        return ManifestFile.Load(dir.FullName);

                // Fallback to "best" version.
                if (dirs.Length > 0)
                    return ManifestFile.Load(dirs[0].FullName);
            }

            string version;
            if (_locks.TryGetValue(name, out version))
            {
                foreach (var dir in _searchPaths.EnumerateLibraryDirectories(name))
                {
                    var versionDir = Path.Combine(dir.FullName, version);
                    if (ManifestFile.Exists(versionDir))
                        return ManifestFile.Load(versionDir);
                }
            }
            else
            {
                var dirs = _searchPaths.GetOrderedVersionDirectories(name).ToArray();

                foreach (var dir in dirs)
                    if (versionRange.IsCompatible(dir.Name))
                        return ManifestFile.Load(dir.FullName);

                // Fallback to "best" version.
                if (dirs.Length > 0)
                    return ManifestFile.Load(dirs[0].FullName);
            }

            return null;
        }
    }
}
