namespace Uno.Compiler.API.Domain.Extensions
{
    public struct BundleFile
    {
        public readonly SourcePackage Package;
        public readonly string BundleName;
        public readonly string TargetName;
        public readonly string SourcePath;

        public BundleFile(SourcePackage package, string bundleName, string sourcePath)
            : this(package, bundleName, bundleName, sourcePath)
        {
        }

        public BundleFile(SourcePackage package, string bundleName, string targetName, string sourcePath)
        {
            Package = package;
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