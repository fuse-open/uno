namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class TitlesViewModel : IndexTitlesViewModel
    {
        public string PageTitle { get; private set; }
        public string NavigationTitle { get; private set; }
        public string FullTitle { get; private set; }

        public TitlesViewModel(string pageTitle, string indexTitle, string fullyQualifiedIndexTitle, string navigationTitle, string fullTitle)
                : base(indexTitle, fullyQualifiedIndexTitle)
        {
            PageTitle = pageTitle;
            NavigationTitle = navigationTitle;
            FullTitle = fullTitle;
        }
    }
}