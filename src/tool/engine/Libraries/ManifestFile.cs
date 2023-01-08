using System.Collections.Generic;
using System.IO;
using Uno.Configuration.Format;
using Uno.Compiler;
using Uno.ProjectFormat;

namespace Uno.Build.Libraries
{
    public class ManifestFile
    {
        internal static string GetName(string dir)
        {
            return Path.Combine(dir, ".uno", "package");
        }

        public static bool Exists(string dir)
        {
            return File.Exists(GetName(dir));
        }

        public static ManifestFile Load(string dir)
        {
            return new ManifestFile(StuffObject.Load(GetName(dir)), dir);
        }

        public string BuildCondition;
        public string SourceDirectory;
        public bool IsTransitive, IsPlaceholder;
        public readonly List<LibraryReference> References = new List<LibraryReference>();
        public readonly List<string> InternalsVisibleTo = new List<string>();
        public readonly List<FileItem> SourceFiles = new List<FileItem>();
        public readonly List<FileItem> ExtensionsFiles = new List<FileItem>();
        public readonly List<FileItem> BundleFiles = new List<FileItem>();
        public readonly List<FileItem> StuffFiles = new List<FileItem>();
        public readonly List<FileItem> UXFiles = new List<FileItem>();
        public readonly List<ForeignItem> ForeignSourceFiles = new List<ForeignItem>();
        public readonly HashSet<string> ExtensionsBackends = new HashSet<string>();
        public readonly HashSet<string> Namespaces = new HashSet<string>();

        public readonly string RootDirectory;
        public string CacheDirectory => Path.Combine(RootDirectory, ".uno");
        public string Filename => GetName(RootDirectory);
        public string Version => _version ?? RootDirectory.GetPathComponent(-1);
        public string Name => _name ?? RootDirectory.GetPathComponent(-2);

        readonly string _name;
        readonly string _version;

        ManifestFile(string dir)
        {
            RootDirectory = dir;
        }

        ManifestFile(StuffObject stuff, string dir)
            : this(dir)
        {
            stuff.TryGetValue(nameof(Name), out _name);
            stuff.TryGetValue(nameof(Version), out _version);
            stuff.TryGetValue(nameof(BuildCondition), out BuildCondition);
            stuff.TryGetValue(nameof(SourceDirectory), out SourceDirectory);
            stuff.TryGetValue(nameof(IsTransitive), out IsTransitive);
            References.AddRange(stuff.GetArray(nameof(References), LibraryReference.FromString));
            InternalsVisibleTo.AddRange(stuff.GetArray(nameof(InternalsVisibleTo)));
            SourceFiles.AddRange(stuff.GetArray(nameof(SourceFiles), FileItem.FromString));
            ExtensionsFiles.AddRange(stuff.GetArray(nameof(ExtensionsFiles), FileItem.FromString));
            BundleFiles.AddRange(stuff.GetArray(nameof(BundleFiles), FileItem.FromString));
            StuffFiles.AddRange(stuff.GetArray(nameof(StuffFiles), FileItem.FromString));
            ForeignSourceFiles.AddRange(stuff.GetArray(nameof(ForeignSourceFiles), ForeignItem.FromString));
            ExtensionsBackends.AddRange(stuff.GetArray(nameof(ExtensionsBackends)));
            Namespaces.AddRange(stuff.GetArray(nameof(Namespaces)));
        }

        public ManifestFile(string installDir, string name, string version)
            : this(Path.Combine(installDir, name, version))
        {
            IsPlaceholder = true;
        }

        public ManifestFile(SourceBundle bundle, string dir)
            : this(dir)
        {
            _name = bundle.Name;
            _version = bundle.Version;
            BuildCondition = bundle.BuildCondition;
            IsTransitive = bundle.IsTransitive;

            foreach (var reference in bundle.References)
                References.Add(new LibraryReference(bundle.Source, reference.Name, reference.Version));

            InternalsVisibleTo.AddRange(bundle.InternalsVisibleTo);
            SourceFiles.AddRange(bundle.SourceFiles);
            ExtensionsFiles.AddRange(bundle.ExtensionsFiles);
            BundleFiles.AddRange(bundle.BundleFiles);
            StuffFiles.AddRange(bundle.StuffFiles);
            UXFiles.AddRange(bundle.UXFiles);
            ForeignSourceFiles.AddRange(bundle.ForeignSourceFiles);

            References.Sort();
            InternalsVisibleTo.Sort();
        }

        public void Save()
        {
            new StuffObject {
                {nameof(Name), Name},
                {nameof(Version), Version},
                {nameof(BuildCondition), BuildCondition},
                {nameof(SourceDirectory), SourceDirectory},
                {nameof(IsTransitive), IsTransitive},
                {nameof(References), References},
                {nameof(InternalsVisibleTo), InternalsVisibleTo},
                {nameof(SourceFiles), SourceFiles},
                {nameof(ExtensionsFiles), ExtensionsFiles},
                {nameof(BundleFiles), BundleFiles},
                {nameof(StuffFiles), StuffFiles},
                {nameof(ForeignSourceFiles), ForeignSourceFiles},
                {nameof(ExtensionsBackends), ExtensionsBackends},
                {nameof(Namespaces), Namespaces}
            }.Save(Filename, true);
        }

        public SourceBundle CreateBundle()
        {
            var bundle = new SourceBundle(
                Name,
                Version,
                Filename,
                SourceDirectory ?? RootDirectory,
                CacheDirectory,
                SourceBundleFlags.Cached | (
                    IsTransitive 
                        ? SourceBundleFlags.Transitive 
                        : 0),
                BuildCondition);

            bundle.SourceFiles.AddRange(SourceFiles);
            bundle.ExtensionsFiles.AddRange(ExtensionsFiles);
            bundle.ForeignSourceFiles.AddRange(ForeignSourceFiles);
            bundle.BundleFiles.AddRange(BundleFiles);
            bundle.StuffFiles.AddRange(StuffFiles);

            foreach (var p in InternalsVisibleTo)
                bundle.InternalsVisibleTo.Add(p);
            foreach (var b in ExtensionsBackends)
                bundle.CachedExtensionsBackends.Add(b);
            foreach (var b in Namespaces)
                bundle.CachedNamespaces.Add(b);

            return bundle;
        }

        public override string ToString()
        {
            return Name + " (" + Version + ")";
        }
    }
}
