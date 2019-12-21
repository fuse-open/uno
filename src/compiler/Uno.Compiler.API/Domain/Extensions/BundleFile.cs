namespace Uno.Compiler.API.Domain.Extensions
{
    public struct BundleFile
    {
        public readonly string BundleName;
        public readonly string TargetName;
        public readonly string SourcePath;

        public BundleFile(string bundleName, string sourcePath)
            : this(bundleName, bundleName, sourcePath)
        {
        }

        public BundleFile(string bundleName, string targetName, string sourcePath)
        {
            BundleName = bundleName;
            TargetName = targetName;
            SourcePath = sourcePath;
        }

        public override string ToString()
        {
            return BundleName;
        }
    }
}