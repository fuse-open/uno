namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class UxNamespaceEntryViewModel
    {
        public string Uri { get; private set; }
        public string Title { get; private set; }

        public UxNamespaceEntryViewModel(string uri, string title)
        {
            Uri = uri;
            Title = title;
        }
    }
}
