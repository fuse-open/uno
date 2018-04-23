namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class UxClassPropertiesViewModel
    {
        public string UxNamespaceTitle { get; private set; }
        public string UxNamespaceUri { get; private set; }
        public string UxName { get; private set; }

        public UxClassPropertiesViewModel(string uxNamespaceTitle, string uxNamespaceUri, string uxName)
        {
            UxNamespaceTitle = uxNamespaceTitle;
            UxNamespaceUri = uxNamespaceUri;
            UxName = uxName;
        }
    }
}