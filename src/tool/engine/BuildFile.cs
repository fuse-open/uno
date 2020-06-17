using System;
using System.IO;
using Uno.Configuration.Format;

namespace Uno.Build
{
    public class BuildFile
    {
        public readonly string RootDirectory;
        public readonly string FullName;
        public string UnoVersion;
        public string RunCommand;
        public string BuildCommand;
        public string Product;

        public bool Exists => File.Exists(FullName);
        public DateTime Timestamp => File.GetLastWriteTime(FullName);
        public bool IsProductUpToDate => !string.IsNullOrEmpty(Product) && 
                                         File.Exists(Path.Combine(RootDirectory, Product)) && 
                                         File.GetLastWriteTime(Path.Combine(RootDirectory, Product)) >= Timestamp &&
                                         UnoVersion == Diagnostics.UnoVersion.InformationalVersion;

        public BuildFile(string dir)
        {
            RootDirectory = dir;
            FullName = Path.Combine(dir, ".unobuild");
        }

        public int Load()
        {
            var stuff = StuffObject.Load(FullName);
            stuff.TryGetValue("$", out int hash);
            stuff.TryGetValue(nameof(UnoVersion), out UnoVersion);
            stuff.TryGetValue(nameof(BuildCommand), out BuildCommand);
            stuff.TryGetValue(nameof(RunCommand), out RunCommand);
            stuff.TryGetValue(nameof(Product), out Product);
            return hash;
        }

        public void Save(int hash)
        {
            Directory.CreateDirectory(RootDirectory);
            new StuffObject {
                {"$", hash },
                {nameof(UnoVersion), UnoVersion},
                {nameof(BuildCommand), BuildCommand},
                {nameof(RunCommand), RunCommand},
                {nameof(Product), Product}
            }.Save(FullName, true);
        }

        public void Delete()
        {
            if (Exists)
                File.Delete(FullName);
        }

        public void TouchProduct()
        {
            if (!string.IsNullOrEmpty(Product) && File.Exists(Path.Combine(RootDirectory, Product)))
                File.SetLastWriteTimeUtc(Path.Combine(RootDirectory, Product), DateTime.UtcNow);
        }
    }
}