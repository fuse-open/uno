using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class UxNamespaceViewModel
    {
        public string Uri { get; private set; }
        public string Title { get; private set; }
        public List<UxNamespaceEntryViewModel> Entries { get; private set; }

        public UxNamespaceViewModel(string uri, string title, List<UxNamespaceEntryViewModel> entries)
        {
            Uri = uri;
            Title = title;
            Entries = entries;
        }
    }
}
