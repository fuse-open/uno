namespace Uno.Compiler.API.Domain.Extensions
{
    public struct BundleFile
    {
        public readonly SourceBundle Bundle;
        public readonly string BundleName;
        public readonly string TargetName;
        public readonly string SourcePath;

        public BundleFile(SourceBundle bundle, string bundleName, string sourcePath)
            : this(bundle, bundleName, bundleName, sourcePath)
        {
        }

        public BundleFile(SourceBundle bundle, string bundleName, string targetName, string sourcePath)
        {
            Bundle = bundle;
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