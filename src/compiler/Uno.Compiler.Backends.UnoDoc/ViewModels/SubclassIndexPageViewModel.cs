using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class SubclassIndexPageViewModel
    {
        public DocumentReferenceViewModel Root { get; }
        public List<TableOfContentsEntryViewModel> Descendants { get; }

        public SubclassIndexPageViewModel(DocumentReferenceViewModel root, List<TableOfContentsEntryViewModel> descendants)
        {
            Root = root;
            Descendants = descendants;
        }
    }
}