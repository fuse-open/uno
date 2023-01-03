using System;

namespace Uno.ProjectFormat
{
    public class LibraryReference : IComparable<LibraryReference>
    {
        public Source Source;
        public string LibraryName { get; set; }
        public string LibraryVersion { get; set; }

        public LibraryReference(Source src, string name, string version = null)
        {
            Source = src;
            LibraryName = name;
            LibraryVersion = version;
        }

        public static LibraryReference FromString(string str)
        {
            return FromString(Source.Unknown, str);
        }

        public static LibraryReference FromString(Source src, string str)
        {
            var parts = str.Split(str.IndexOf(':') != -1 ? ':' : ',');
            var name = parts[0].Trim();
            var version = parts.Length > 1 && !string.IsNullOrEmpty(parts[1])
                ? parts[1].Trim()
                : null;

            if (parts.Length > 2)
                throw new ArgumentException("Invalid arguments provided for 'LIBRARY[:VERSION]'");

            return new LibraryReference(src, name, version);
        }

        public override string ToString()
        {
            return LibraryName + (
                    !string.IsNullOrEmpty(LibraryVersion)
                        ? ":" + LibraryVersion
                        : null
                );
        }

        public override bool Equals(object obj)
        {
            return obj is LibraryReference && Equals((LibraryReference) obj);
        }

        bool Equals(LibraryReference other)
        {
            return string.Equals(LibraryName, other.LibraryName) &&
                   string.Equals(LibraryVersion ?? "", other.LibraryVersion ?? "");
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = LibraryName.GetHashCode();
                if (LibraryVersion != null)
                    hash = (hash * 397) ^ LibraryVersion.GetHashCode();
                return hash;
            }
        }

        public int CompareTo(LibraryReference other)
        {
            return string.Compare(LibraryName, other.LibraryName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
