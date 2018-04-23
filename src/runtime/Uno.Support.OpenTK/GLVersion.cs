namespace Uno.Support.OpenTK
{
    public struct GLVersion
    {
        public readonly string VersionString;
        public readonly int Major;
        public readonly int Minor;

        internal GLVersion(int major, int minor, string versionString)
        {
            Major = major;
            Minor = minor;
            VersionString = versionString;
        }
    }
}