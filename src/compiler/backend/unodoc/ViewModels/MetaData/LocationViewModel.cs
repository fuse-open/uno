namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class LocationViewModel
    {
        public string NamespaceTitle { get; private set; }
        public string NamespaceUri { get; private set; }
        public string LibraryName { get; private set; }
        public string LibraryVersion { get; private set; }

        public LocationViewModel(string namespaceTitle, string namespaceUri, string libraryName, string libraryVersion)
        {
            NamespaceTitle = namespaceTitle;
            NamespaceUri = namespaceUri;
            LibraryName = libraryName;
            LibraryVersion = libraryVersion;
        }
    }
}