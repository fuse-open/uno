using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NuGet.Packaging;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.Build.Packages
{
    public class UpkBuilder : DiskObject
    {
        const BuildConfiguration Configuration = BuildConfiguration.Release;

        readonly BuildTarget _target;
        readonly LibraryBuilder _libBuilder;

        public string BuildDirectory;
        public string OutputDirectory;
        public string Version;
        public string Suffix;

        public UpkBuilder(Disk disk, BuildTarget target)
            : base(disk)
        {
            _target = target;
            _libBuilder = new LibraryBuilder(Disk, target);
        }

        public void BuildAll(IReadOnlyList<string> projects)
        {
            if (projects.Count == 0)
                throw new InvalidOperationException("No input files");

            foreach (var f in projects)
                Build(f);
        }

        public string Build(string project)
        {
            return Build(Project.Load(project));
        }

        public string Build(Project project)
        {
            var version = project.Version;

            if (!string.IsNullOrEmpty(Version))
                version = Version;
            if (!string.IsNullOrEmpty(Suffix))
                version += "-" + TruncateSpecialVersion(Suffix);

            var isUpToDate = false;
            var buildDir = BuildDirectory.UnixToNative() ?? GetBuildDirectory(project, out isUpToDate);
            var outputDir = OutputDirectory.UnixToNative() ?? Directory.GetCurrentDirectory();
            var upk = Path.Combine(outputDir, project.Name + "-" + version + ".upk");

            using (Log.StartAnimation("Creating " + upk.ToRelativePath().Quote(), ConsoleColor.Cyan))
            {
                try
                {
                    Build(project, buildDir, isUpToDate, upk, propertyName =>
                    {
                        switch (propertyName)
                        {
                            case "version":
                                return version;
                            default:
                                Log.VeryVerbose("NuGet: Unspecified property: " + propertyName);
                                return null;
                        }
                    });
                    return upk;
                }
                catch
                {
                    Disk.DeleteFile(upk);
                    throw;
                }
            }
        }

        void Build(Project project, string buildDir, bool isUpToDate, string upk, Func<string, string> propertyProvider)
        {
            if (!isUpToDate)
                new ProjectBuilder(
                        Log.GetQuieterLog(),
                        _target,
                        new BuildOptions
                        {
                            Configuration = Configuration,
                            OutputDirectory = buildDir
                        })
                    .Build(project);

            using (var f = Disk.CreateFile(upk))
                new PackageBuilder(
                        Path.Combine(buildDir, project.Name + ".nuspec"), 
                        buildDir,
                        propertyProvider, 
                        true)
                    .Save(f);
        }

        string GetBuildDirectory(Project project, out bool isUpToDate)
        {
            var original = project.GetOutputDirectory(Configuration, _target);
            var info = new BuildFile(original);

            if (!info.Exists)
            {
                try
                {
                    var upper = project.FullPath.ToUpperInvariant();
                    foreach (var source in _libBuilder.GetSourceDirectories(project.Config) ?? new HashSet<string>())
                    {
                        if (upper.StartsWith(source.ToUpperInvariant()))
                        {
                            var lib = new LibraryProject(project, source);
                            if (!lib.Exists || 
                                lib.Configuration != Configuration)
                                goto RETURN_ORIGINAL;

                            var time = lib.LastBuildTime;
                            if (File.GetLastWriteTime(project.FullPath) >= time)
                                goto RETURN_ORIGINAL;

                            var root = project.RootDirectory;
                            foreach (var file in project.AllFiles)
                                if (File.GetLastWriteTime(Path.Combine(root, file.NativePath)) >= time)
                                    goto RETURN_ORIGINAL;

                            // Found up-to-date build
                            isUpToDate = true;
                            return lib.VersionDirectory;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                    Log.Warning("GetBuildDirectory() failed: " + e.Message);
                }
            }
        RETURN_ORIGINAL:
            isUpToDate = false;
            return original;
        }

        static string TruncateSpecialVersion(string input)
        {
            // We only want letters, digits and '-' in this string.
            // And the string cannot start with a number.
            var sb = new StringBuilder();
            if (input.Length == 0 || char.IsDigit(input[0]))
                sb.Append('-');

            foreach (char c in input)
                sb.Append(char.IsLetterOrDigit(c) ? c : '-');

            // "special version" in NuGet aren't allowed more than 20 characters.
            return sb.Length > 20
                ? sb.ToString(sb.Length - 20, 20)
                : sb.ToString();
        }
    }
}
