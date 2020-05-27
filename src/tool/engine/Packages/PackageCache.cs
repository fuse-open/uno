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

namespace Uno.Build.Packages
{
    public class PackageCache : DiskObject, IDisposable
    {
        // For use by UXNinja & tests in Fuse Studio
        public static SourcePackage GetPackage(Log log, Project project)
        {
            // Passing false to disable FuseJS transpiler.
            using (var cache = new PackageCache(log, project.Config, false))
                return cache.GetPackage(project);
        }

        readonly PackageSearchPaths _sourcePaths = new PackageSearchPaths();
        readonly PackageSearchPaths _searchPaths = new PackageSearchPaths();
        readonly Dictionary<string, string> _locks = new Dictionary<string, string>();
        readonly Dictionary<string, SourcePackage> _cache = new Dictionary<string, SourcePackage>();
        readonly ListDictionary<string, DirectoryInfo> _library = new ListDictionary<string, DirectoryInfo>();
        readonly UnoConfig _config;
        readonly bool _enableTranspiler;
        Transpiler _transpiler;

        public IEnumerable<string> SearchPaths => _sourcePaths.Concat(_searchPaths).Where(Directory.Exists);

        public PackageCache()
            : this(null, null, false)
        {
        }

        public PackageCache(Log log, UnoConfig config, bool enableTranspiler = true)
            : base(log ?? Log.Null)
        {
            if (config == null)
                config = UnoConfig.Current;

            _config = config;
            _enableTranspiler = enableTranspiler;

            foreach (var src in config.GetFullPathArray("Packages.SourcePaths"))
                _sourcePaths.AddOnce(Path.Combine(
                    File.Exists(src)
                        ? Path.GetDirectoryName(src)
                        : src,
                    "build"));

            foreach (var src in config.GetFullPathArray("Packages.SearchPaths"))
                _searchPaths.AddOnce(src);
        }

        public void Dispose()
        {
            _transpiler?.Dispose();
            _transpiler = null;
        }

        public SourcePackage GetPackage(string name, string version = null)
        {
            return GetPackage(new PackageReference(Source.Unknown, name, version));
        }

        public SourcePackage GetPackage(Project project)
        {
            return GetPackage(project.Source, project, true);
        }

        public List<DirectoryInfo> GetVersionDirectories(string package)
        {
            List<DirectoryInfo> result;
            if (!_library.TryGetValue(package, out result))
            {
                result = new List<DirectoryInfo>();
                _library.Add(package, result);

                foreach (var feed in SearchPaths)
                {
                    DirectoryInfo dir;
                    if (!GetPackageDirectories(feed).TryGetValue(package, out dir))
                        continue;

                    var versions = dir.GetDirectories();
                    Array.Sort(versions, (left, right) => VersionRange.Compare(right.Name, left.Name));

                    foreach (var version in versions)
                        if (PackageFile.Exists(version.FullName))
                            result.Add(version);
                }
            }

            return result;
        }

        public Dictionary<string, DirectoryInfo> GetPackageDirectories(string directory)
        {
            var result = new Dictionary<string, DirectoryInfo>();

            foreach (var package in new DirectoryInfo(directory).GetDirectories())
                result[package.Name] = package;

            return result;
        }

        public IEnumerable<DirectoryInfo> EnumeratePackages(string name)
        {
            return _sourcePaths.EnumeratePackageDirectories(name).Concat(
                   _searchPaths.EnumeratePackageDirectories(name));
        }

        public IEnumerable<DirectoryInfo> EnumerateVersions(string name, string version = null)
        {
            return _sourcePaths.EnumerateVersionDirectories(name, version).Concat(
                   _searchPaths.EnumerateVersionDirectories(name, version));
        }

        SourcePackage GetPackage(Source src, Project project, bool startup = false)
        {
            SourcePackage result;
            if (_cache.TryGetValue(project.Name, out result))
            {
                if (result == null)
                {
                    Log.Error(src, null, "Circular reference to " + project.Name.Quote());
                    return new SourcePackage(project.Name);
                }

                if (result.Source.FullPath != project.FullPath || startup)
                {
                    Log.Error(src, null, "Multiple projects or packages with the name " + project.Name.Quote());
                    return new SourcePackage(project.Name);
                }

                result.Flags &= ~SourcePackageFlags.Startup;
                return result;
            }

            result = project.CreateSourcePackage(startup);
            _cache[project.Name] = null;

            if (project.UnoCoreReference)
                result.References.Add(GetPackage(new PackageReference(project.Source, "UnoCore")));

            foreach (var r in project.PackageReferences)
                result.References.Add(GetPackage(r));
            foreach (var r in project.ProjectReferences)
                result.References.Add(GetPackage(r.Source, 
                    LoadProject(r.Source, r.GetFullPath(project.RootDirectory))));

            // Transpile FuseJS files
            foreach (var f in project.FuseJSFiles)
            {
                // We don't need to spend time on this in UXNinja, tests, etc.
                if (!_enableTranspiler)
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

                // Ensure Transpiler is initialized before starting a task
                if (_transpiler == null)
                    _transpiler = new Transpiler(Log, _config);

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

        SourcePackage GetPackage(PackageReference reference)
        {
            SourcePackage result;
            if (_cache.TryGetValue(reference.PackageName, out result))
            {
                if (result == null)
                {
                    Log.Error(reference.Source, null, "Circular reference to " + reference.Quote());
                    return new SourcePackage(reference.PackageName, reference.PackageVersion);
                }

                result.Flags |= SourcePackageFlags.Cached;
                result.Flags &= ~SourcePackageFlags.Startup;
                return result;
            }

            var package = GetFile(reference);
            if (package == null)
            {
                Log.Error(reference.Source, ErrorCode.E0100, "Package " + reference.Quote() + " was not found");
                return new SourcePackage(reference.PackageName, reference.PackageVersion);
            }

            result = package.CreateSourcePackage();
            _cache[reference.PackageName] = null;

            foreach (var r in package.References)
                result.References.Add(GetPackage(r));

            _cache[reference.PackageName] = result;
            return result;
        }

        PackageFile GetFile(PackageReference reference)
        {
            var name = reference.PackageName;
            var versionRange = VersionRange.Parse(reference.PackageVersion);

            if (_sourcePaths.Count > 0)
            {
                var dirs = _sourcePaths.GetOrderedVersionDirectories(name);

                foreach (var dir in dirs)
                    if (versionRange.IsCompatible(dir.Name))
                        return PackageFile.Load(dir.FullName);

                // Fallback to "best" version.
                if (dirs.Length > 0)
                    return PackageFile.Load(dirs[0].FullName);
            }

            string version;
            if (_locks.TryGetValue(name, out version))
            {
                foreach (var dir in _searchPaths.EnumeratePackageDirectories(name))
                {
                    var versionDir = Path.Combine(dir.FullName, version);
                    if (PackageFile.Exists(versionDir))
                        return PackageFile.Load(versionDir);
                }
            }
            else
            {
                var dirs = _searchPaths.GetOrderedVersionDirectories(name).ToArray();

                foreach (var dir in dirs)
                    if (versionRange.IsCompatible(dir.Name))
                        return PackageFile.Load(dir.FullName);

                // Fallback to "best" version.
                if (dirs.Length > 0)
                    return PackageFile.Load(dirs[0].FullName);
            }

            return null;
        }
    }
}
