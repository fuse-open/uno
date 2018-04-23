namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ValueViewModel
    {
        public string Uri { get; }
        public string Title { get; }
        public BasicCommentViewModel Comment { get; }

        public ValueViewModel(string uri, string title, BasicCommentViewModel comment)
        {
            Uri = uri;
            Title = title;
            Comment = comment;
        }

        public bool ShouldSerializeComment()
        {
            return Comment != null && Comment.ShouldSerialize();
        }
    }
}