using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class RootViewModel : DocumentViewModel
    {
        public RootViewModel(TitlesViewModel titles)
                : base(new DocumentIdViewModel("__root__", null, "Root", new List<string>()),
                       new DocumentUriViewModel("__root__", "index", false),
                       titles,
                       null,
                       null,
                       null,
                       null) {}
    }
}
