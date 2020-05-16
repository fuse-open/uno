namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class LocationViewModel
    {
        public string NamespaceTitle { get; private set; }
        public string NamespaceUri { get; private set; }
        public string PackageName { get; private set; }
        public string PackageVersion { get; private set; }

        public LocationViewModel(string namespaceTitle, string namespaceUri, string packageName, string packageVersion)
        {
            NamespaceTitle = namespaceTitle;
            NamespaceUri = namespaceUri;
            PackageName = packageName;
            PackageVersion = packageVersion;
        }
    }
}