namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ReturnsViewModel
    {
        public string Href { get; private set; }
        public bool IsVirtual { get; private set; }
        public string Title { get; private set; }
        public string FullyQualifiedTitle { get; private set; }

        public ReturnsViewModel(string href, bool isVirtual, string title, string fullyQualifiedTitle)
        {
            Href = href;
            IsVirtual = isVirtual;
            Title = title;
            FullyQualifiedTitle = fullyQualifiedTitle;
        }
    }
}
