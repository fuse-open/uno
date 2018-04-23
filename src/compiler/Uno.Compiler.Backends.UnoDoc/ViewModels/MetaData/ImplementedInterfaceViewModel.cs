namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ImplementedInterfaceViewModel : DocumentReferenceViewModel
    {
        public BasicCommentViewModel Comment { get; }

        public ImplementedInterfaceViewModel(DocumentIdViewModel id,
                                             DocumentUriViewModel uri,
                                             IndexTitlesViewModel titles,
                                             BasicCommentViewModel comment)
            : base(id, uri, titles)
        {
            Comment = comment;
        }

        public bool ShouldSerializeComment()
        {
            return Comment != null && Comment.ShouldSerialize();
        }
    }
}