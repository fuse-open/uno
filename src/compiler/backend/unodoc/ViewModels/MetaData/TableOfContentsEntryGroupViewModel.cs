using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class TableOfContentsEntryGroupViewModel
    {
        public DocumentReferenceViewModel DeclaredIn { get; private set; }
        public List<TableOfContentsEntryViewModel> Items { get; private set; }

        public TableOfContentsEntryGroupViewModel(DocumentReferenceViewModel declaredIn, List<TableOfContentsEntryViewModel> items)
        {
            DeclaredIn = declaredIn;
            Items = items;
        }
    }
}