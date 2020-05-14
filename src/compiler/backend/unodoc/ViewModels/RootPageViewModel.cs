using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class RootPageViewModel : PageViewModel
    {
        public List<UxNamespaceViewModel> UxNamespaces { get; private set; }
        public bool HasUxNamespaces { get; private set; }

        public RootPageViewModel(DocumentViewModel entity, TableOfContentsViewModel tableOfContents, List<UxNamespaceViewModel> uxNamespaces)
                : base(entity, tableOfContents)
        {
            UxNamespaces = uxNamespaces;
            HasUxNamespaces = uxNamespaces != null && uxNamespaces.Count > 0;
        }
    }
}
