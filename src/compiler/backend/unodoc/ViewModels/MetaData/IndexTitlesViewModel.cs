namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class IndexTitlesViewModel
    {
        public string IndexTitle { get; }
        public string FullyQualifiedIndexTitle { get; }

        public IndexTitlesViewModel(string indexTitle, string fullyQualifiedIndexTitle)
        {
            IndexTitle = indexTitle;
            FullyQualifiedIndexTitle = fullyQualifiedIndexTitle;
        }
    }
}