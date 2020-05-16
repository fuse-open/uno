using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class NamespaceViewModel : DocumentViewModel
    {
        public NamespaceViewModel(DocumentIdViewModel id,
                                  DocumentUriViewModel uri,
                                  TitlesViewModel titles,
                                  SyntaxViewModel syntax,
                                  CommentViewModel comment)
                : base(id, uri, titles, syntax, comment, null, null) {}
    }
}