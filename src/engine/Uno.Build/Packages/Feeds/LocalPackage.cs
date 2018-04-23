using System;

namespace Uno.Build.Packages.Feeds
{
    public class LocalPackage : IPackage, IComparable<LocalPackage>
    {
        public string Name { get; }
        public string Version { get; }
        public string Source { get; }

        public LocalPackage(string name, string version, string source)
        {
            Name = name;
            Version = version;
            Source = source;
        }

        public int CompareTo(LocalPackage other)
        {
            var diff = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            return diff == 0
                ? -VersionRange.Compare(Version, other.Version)
                : diff;
        }

        public override string ToString()
        {
            return Name + " " + Version;
        }

        public void Install(string directory)
        {
            throw new InvalidOperationException(this.Quote() + " is already installed");
        }
    }
}