using System;
using System.IO;
using System.Linq;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.Build.Libraries
{
    public class LibraryProject
    {
        public readonly Project Project;
        public readonly string RootDirectory;
        public readonly string CacheDirectory;
        public readonly string ConfigFile;
        public readonly string ManifestFile;

        public bool Exists => File.Exists(ManifestFile) && File.Exists(ConfigFile);
        public BuildConfiguration Configuration => (BuildConfiguration) Enum.Parse(typeof(BuildConfiguration), File.ReadAllText(ConfigFile).Trim());

        public LibraryProject(Project project, string sourceDir)
        {
            Project = project;
            RootDirectory = Path.Combine(sourceDir, "build", project.Name);
            CacheDirectory = Path.Combine(RootDirectory, ".uno");
            ConfigFile = Path.Combine(CacheDirectory, "config");
            ManifestFile = Path.Combine(CacheDirectory, "manifest");
        }

        LibraryProject(LibraryProject lib)
        {
            Project = lib.Project;
            RootDirectory = lib.RootDirectory;
            CacheDirectory = Path.Combine(RootDirectory, ".uno");
            ConfigFile = Path.Combine(CacheDirectory, "config");
            ManifestFile = Path.Combine(CacheDirectory, "manifest");
        }

        public bool TryGetExistingBuild(out LibraryProject existing)
        {
            existing = null;
            if (!Directory.Exists(RootDirectory))
                return false;

            existing = new LibraryProject(this);
            return true;
        }

        int? _hash;
        public override int GetHashCode()
        {
            return _hash ?? (_hash = Project.FullPath.GetHashCode()).Value;
        }

        string _upper;
        public string ToUpperInvariant()
        {
            return _upper ?? (_upper = Project.Name.ToUpperInvariant());
        }

        public string RelativePath => Project.FullPath.ToRelativePath();

        string[] _refs;
        public string[] References
        {
            get
            {
                return _refs ?? (_refs =
                           Project.PackageReferences.Select(x => x.LibraryName.ToUpperInvariant()).Concat(
                           Project.ProjectReferences.Select(x => x.ProjectName.ToUpperInvariant()).Concat(
                           Project.Name != "UnoCore" ? new[] { "UnoCore".ToUpperInvariant() } : new string[0])
                       ).ToArray());
            }
        }

        DateTime? _buildTime;
        public DateTime LastBuildTime => _buildTime ?? (_buildTime =
            File.Exists(ManifestFile)
                ? File.GetLastWriteTime(ManifestFile)
                : new DateTime(2000, 1, 1)
            ).Value;

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
