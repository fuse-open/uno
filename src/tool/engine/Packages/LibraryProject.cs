using System;
using System.IO;
using System.Linq;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.Build.Packages
{
    public class LibraryProject
    {
        public readonly Project Project;
        public readonly string PackageDirectory;
        public readonly string CacheDirectory;
        public readonly string ConfigFile;
        public readonly string PackageFile;

        public bool Exists => File.Exists(PackageFile) && File.Exists(ConfigFile);
        public BuildConfiguration Configuration => (BuildConfiguration) Enum.Parse(typeof(BuildConfiguration), File.ReadAllText(ConfigFile).Trim());

        public LibraryProject(Project project, string sourceDir)
        {
            Project = project;
            PackageDirectory = Path.Combine(sourceDir, "build", project.Name);
            CacheDirectory = Path.Combine(PackageDirectory, ".uno");
            ConfigFile = Path.Combine(CacheDirectory, "config");
            PackageFile = Path.Combine(CacheDirectory, "package");
        }

        LibraryProject(LibraryProject lib)
        {
            Project = lib.Project;
            PackageDirectory = lib.PackageDirectory;
            CacheDirectory = Path.Combine(PackageDirectory, ".uno");
            ConfigFile = Path.Combine(CacheDirectory, "config");
            PackageFile = Path.Combine(CacheDirectory, "package");
        }

        public bool TryGetExistingBuild(out LibraryProject existing)
        {
            existing = null;
            if (!Directory.Exists(PackageDirectory))
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
                           Project.PackageReferences.Select(x => x.PackageName.ToUpperInvariant()).Concat(
                           Project.ProjectReferences.Select(x => x.ProjectName.ToUpperInvariant()).Concat(
                           Project.UnoCoreReference ? new[] { "UnoCore".ToUpperInvariant() } : new string[0])
                       ).ToArray());
            }
        }

        DateTime? _buildTime;
        public DateTime LastBuildTime => _buildTime ?? (_buildTime =
            File.Exists(PackageFile)
                ? File.GetLastWriteTime(PackageFile)
                : new DateTime(2000, 1, 1)
            ).Value;

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
