using System;
using System.IO;
using NuGet;
using Uno.IO;

namespace Uno.Build.Packages.Feeds
{
    public class UpkFile : IPackage
    {
        public readonly string Filename;

        ZipPackage _zip;
        internal ZipPackage ZipPackage
        {
            get
            {
                try
                {
                    return _zip ?? (_zip = new ZipPackage(Filename));
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Failed to load " + Filename.ToRelativePath() + ": " + e.Message, e);
                }
            }
        }

        public string Name { get; }
        public string Version { get; }
        public string Source => Filename;
        public long ZipPackageSize => new FileInfo(Source).Length;

        public UpkFile(string filename, string name = null, string version = null)
        {
            Filename = filename;
            Name = name ?? ZipPackage.Id;
            Version = version ?? ZipPackage.Version.ToString();
        }

        public void Install(string directory)
        {
            using (var f = ZipPackage.GetStream())
                f.ExtractNupkg(directory);
        }

        public override string ToString()
        {
            return Filename.ToRelativePath();
        }
    }
}