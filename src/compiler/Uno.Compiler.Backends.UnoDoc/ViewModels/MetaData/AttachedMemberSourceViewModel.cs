namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class AttachedMemberSourceViewModel
    {
        public string Uri { get; private set; }
        public string Title { get; private set; }

        public AttachedMemberSourceViewModel(string uri, string title)
        {
            Uri = uri;
            Title = title;
        }
    }
}