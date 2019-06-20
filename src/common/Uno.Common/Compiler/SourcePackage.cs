using System.Collections.Generic;

namespace Uno.Compiler
{
    public class SourcePackage
    {
        static SourcePackage _unknown;
        public static SourcePackage Unknown => _unknown ?? (_unknown = new SourcePackage("(unknown)"));

        // Cache key
        public static readonly object ILKey = new object();

        public Source Source { get; private set; }
        public string Name { get; }
        public string Version { get; }
        public string SourceDirectory { get; private set; }
        public string CacheDirectory { get; private set; }
        public string BuildCondition { get; private set; }
        public SourcePackageFlags Flags;
        public object Tag;

        public bool CanLink => Flags.HasFlag(SourcePackageFlags.CanLink);
        public bool IsCached => Flags.HasFlag(SourcePackageFlags.Cached);
        public bool IsStartup => Flags.HasFlag(SourcePackageFlags.Startup);
        public bool IsTransitive => Flags.HasFlag(SourcePackageFlags.Transitive);
        public bool IsVerified => Flags.HasFlag(SourcePackageFlags.Verified);
        public bool IsProject => Flags.HasFlag(SourcePackageFlags.Project);
        public bool IsUnknown => ReferenceEquals(this, _unknown);

        public readonly Dictionary<object, object> Cache = new Dictionary<object, object>();
        public readonly HashSet<string> CachedNamespaces = new HashSet<string>();
        public readonly HashSet<string> CachedExtensionsBackends = new HashSet<string>();
        public readonly HashSet<string> InternalsVisibleTo = new HashSet<string>();
        public readonly HashSet<SourcePackage> References = new HashSet<SourcePackage>();
        public readonly List<FileItem> ExtensionsFiles = new List<FileItem>();
        public readonly List<ForeignItem> ForeignSourceFiles = new List<ForeignItem>();
        public readonly List<FileItem> SourceFiles = new List<FileItem>();
        public readonly List<FileItem> StuffFiles = new List<FileItem>();
        public readonly List<FileItem> BundleFiles = new List<FileItem>();
        public readonly List<FileItem> AdditionalFiles = new List<FileItem>();
        public readonly List<FileItem> UXFiles = new List<FileItem>();

        public IEnumerable<FileItem> AllFiles
        {
            get
            {
                foreach (var f in ExtensionsFiles)
                    yield return f;
                foreach (var f in ForeignSourceFiles)
                    yield return new FileItem(f.UnixPath, f.Condition);
                foreach (var f in SourceFiles)
                    yield return f;
                foreach (var f in StuffFiles)
                    yield return f;
                foreach (var f in BundleFiles)
                    yield return f;
                foreach (var f in AdditionalFiles)
                    yield return f;
                foreach (var f in UXFiles)
                    yield return f;
            }
        }

        public bool HasTransitiveReferences
        {
            get
            {
                foreach (var e in References)
                    if (e.IsTransitive)
                        return true;
                return false;
            }
        }

        public SourcePackage(
            string name,
            string version = null,
            string path = null,
            string sourceDir = null,
            string cacheDir = null,
            SourcePackageFlags flags = 0,
            string buildCondition = null)
        {
            Name = name;
            Version = version;
            Source = new Source(new SourceFile(this, path ?? "(null)"));
            SourceDirectory = sourceDir ?? "(null)";
            CacheDirectory = cacheDir ?? "(null)";
            Flags = flags;
            BuildCondition = buildCondition;
        }

        // Used by Fuse Studio
        public void SetCacheDirectory(string dir)
        {
            CacheDirectory = dir;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj == this;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Version)
                    ? Name + "@" + Version
                    : Name;
        }

        public bool TryGetCache<T>(object key, out T result)
        {
            object value;
            if (Cache.TryGetValue(key, out value))
            {
                result = (T) value;
                return true;
            }

            result = default(T);
            return false;
        }

        public void FlattenTransitiveReferences()
        {
            if (!HasTransitiveReferences)
                return;

            var refs = new List<SourcePackage>(References);
            for (int i = 0; i < refs.Count; i++)
            {
                if (refs[i].IsTransitive)
                {
                    foreach (var r in refs[i].References)
                    {
                        if (!References.Contains(r))
                        {
                            References.Add(r);
                            refs.Add(r);
                        }
                    }
                }
            }
        }

        public bool IsAccessibleFrom(SourcePackage other)
        {
            return ReferenceEquals(other, this) ||
                   other.IsUnknown || IsUnknown ||
                   other.References.Contains(this);
        }
    }
}
