using System.Collections.Generic;
using System.IO;
using Uno.Configuration.Format;
using Uno.Compiler;
using Uno.ProjectFormat;

namespace Uno.Build.Packages
{
    public class PackageFile
    {
        internal static string GetName(string dir)
        {
            return Path.Combine(dir, ".uno", "package");
        }

        public static bool Exists(string dir)
        {
            return File.Exists(GetName(dir));
        }

        public static PackageFile Load(string dir)
        {
            return new PackageFile(StuffObject.Load(GetName(dir)), dir);
        }

        public string BuildCondition;
        public string SourceDirectory;
        public bool IsTransitive, IsPlaceholder;
        public readonly List<PackageReference> References = new List<PackageReference>();
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
        public string Version => RootDirectory.GetPathComponent(-1);
        public string Name => RootDirectory.GetPathComponent(-2);

        PackageFile(string dir)
        {
            RootDirectory = dir;
        }

        PackageFile(StuffObject stuff, string dir)
            : this(dir)
        {
            stuff.TryGetValue(nameof(BuildCondition), out BuildCondition);
            stuff.TryGetValue(nameof(SourceDirectory), out SourceDirectory);
            stuff.TryGetValue(nameof(IsTransitive), out IsTransitive);
            References.AddRange(stuff.GetArray(nameof(References), PackageReference.FromString));
            InternalsVisibleTo.AddRange(stuff.GetArray(nameof(InternalsVisibleTo)));
            SourceFiles.AddRange(stuff.GetArray(nameof(SourceFiles), FileItem.FromString));
            ExtensionsFiles.AddRange(stuff.GetArray(nameof(ExtensionsFiles), FileItem.FromString));
            BundleFiles.AddRange(stuff.GetArray(nameof(BundleFiles), FileItem.FromString));
            StuffFiles.AddRange(stuff.GetArray(nameof(StuffFiles), FileItem.FromString));
            ForeignSourceFiles.AddRange(stuff.GetArray(nameof(ForeignSourceFiles), ForeignItem.FromString));
            ExtensionsBackends.AddRange(stuff.GetArray(nameof(ExtensionsBackends)));
            Namespaces.AddRange(stuff.GetArray(nameof(Namespaces)));
        }

        public PackageFile(string installDir, string name, string version)
            : this(Path.Combine(installDir, name, version))
        {
            IsPlaceholder = true;
        }

        public PackageFile(SourcePackage upk, string dir)
            : this(dir)
        {
            BuildCondition = upk.BuildCondition;
            IsTransitive = upk.IsTransitive;

            foreach (var reference in upk.References)
                References.Add(new PackageReference(upk.Source, reference.Name, reference.Version));

            InternalsVisibleTo.AddRange(upk.InternalsVisibleTo);
            SourceFiles.AddRange(upk.SourceFiles);
            ExtensionsFiles.AddRange(upk.ExtensionsFiles);
            BundleFiles.AddRange(upk.BundleFiles);
            StuffFiles.AddRange(upk.StuffFiles);
            UXFiles.AddRange(upk.UXFiles);
            ForeignSourceFiles.AddRange(upk.ForeignSourceFiles);

            References.Sort();
            InternalsVisibleTo.Sort();
        }

        public void Save()
        {
            new StuffObject {
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

        public SourcePackage CreateSourcePackage()
        {
            var upk = new SourcePackage(
                Name,
                Version,
                Filename,
                SourceDirectory ?? RootDirectory,
                CacheDirectory,
                SourcePackageFlags.Cached | (
                    IsTransitive 
                        ? SourcePackageFlags.Transitive 
                        : 0),
                BuildCondition);

            upk.SourceFiles.AddRange(SourceFiles);
            upk.ExtensionsFiles.AddRange(ExtensionsFiles);
            upk.ForeignSourceFiles.AddRange(ForeignSourceFiles);
            upk.BundleFiles.AddRange(BundleFiles);
            upk.StuffFiles.AddRange(StuffFiles);

            foreach (var p in InternalsVisibleTo)
                upk.InternalsVisibleTo.Add(p);
            foreach (var b in ExtensionsBackends)
                upk.CachedExtensionsBackends.Add(b);
            foreach (var b in Namespaces)
                upk.CachedNamespaces.Add(b);

            return upk;
        }

        public override string ToString()
        {
            return Name + " (" + Version + ")";
        }
    }
}
