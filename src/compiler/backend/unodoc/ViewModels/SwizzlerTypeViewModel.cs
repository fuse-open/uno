using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class SwizzlerTypeViewModel : DocumentViewModel
    {
        public SwizzlerTypeViewModel(DocumentIdViewModel id,
                                     DocumentUriViewModel uri,
                                     TitlesViewModel titles,
                                     CommentViewModel comment)
                : base(id, uri, titles, null, comment, null, null) {}
    }
}