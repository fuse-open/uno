namespace Uno.Compiler.API.Domain.Extensions
{
    public struct BundleFile
    {
        public readonly string BundleName;
        public readonly string TargetName;
        public readonly string SourcePath;
        public readonly ContentType ContentType;

        public BundleFile(string bundleName, string sourcePath, ContentType type)
            : this(bundleName, bundleName, sourcePath, type)
        {
        }

        public BundleFile(string bundleName, string targetName, string sourcePath, ContentType type)
        {
            BundleName = bundleName;
            TargetName = targetName;
            SourcePath = sourcePath;
            ContentType = type;
        }

        public override string ToString()
        {
            return BundleName;
        }
    }
}