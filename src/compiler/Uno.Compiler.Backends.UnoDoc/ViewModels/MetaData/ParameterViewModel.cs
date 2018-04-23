namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ParameterViewModel
    {
        public string Name { get; }
        public string Href { get; }
        public bool IsVirtual { get; }
        public string Title { get; }
        public string FullyQualifiedTitle { get; }

        public ParameterViewModel(string name, string href, bool isVirtual, string title, string fullyQualifiedTitle)
        {
            Name = name;
            Href = href;
            IsVirtual = isVirtual;
            Title = title;
            FullyQualifiedTitle = fullyQualifiedTitle;
        }
    }
}
