using System;

namespace Uno.ProjectFormat
{
    public class PackageReference : IComparable<PackageReference>
    {
        public Source Source;
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }

        public PackageReference(Source src, string name, string version = null)
        {
            Source = src;
            PackageName = name;
            PackageVersion = version;
        }

        public static PackageReference FromString(string str)
        {
            return FromString(Source.Unknown, str);
        }

        public static PackageReference FromString(Source src, string str)
        {
            var parts = str.Split(str.IndexOf(':') != -1 ? ':' : ',');
            var name = parts[0].Trim();
            var version = parts.Length > 1 && !string.IsNullOrEmpty(parts[1])
                ? parts[1].Trim()
                : null;

            if (parts.Length > 2)
                throw new ArgumentException("Invalid arguments provided for 'PACKAGE[:VERSION]'");

            return new PackageReference(src, name, version);
        }

        public override string ToString()
        {
            return PackageName + (
                    !string.IsNullOrEmpty(PackageVersion)
                        ? ":" + PackageVersion
                        : null
                );
        }

        public override bool Equals(object obj)
        {
            return obj is PackageReference && Equals((PackageReference) obj);
        }

        bool Equals(PackageReference other)
        {
            return string.Equals(PackageName, other.PackageName) &&
                   string.Equals(PackageVersion ?? "", other.PackageVersion ?? "");
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = PackageName.GetHashCode();
                if (PackageVersion != null)
                    hash = (hash * 397) ^ PackageVersion.GetHashCode();
                return hash;
            }
        }

        public int CompareTo(PackageReference other)
        {
            return string.Compare(PackageName, other.PackageName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
