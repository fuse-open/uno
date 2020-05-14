namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class DocumentReferenceViewModel
    {
        public DocumentIdViewModel Id { get; protected set; }
        public DocumentUriViewModel Uri { get; protected set; }
        public IndexTitlesViewModel Titles { get; protected set; }

        public DocumentReferenceViewModel(DocumentIdViewModel id, DocumentUriViewModel uri, IndexTitlesViewModel titles)
        {
            Id = id;
            Uri = uri;
            Titles = titles;
        }
    }
}
