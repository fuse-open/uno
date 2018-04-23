using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class PageViewModel
    {
        public DocumentViewModel Entity { get; private set; }
        public TableOfContentsViewModel TableOfContents { get; private set; }
        
        public PageViewModel(DocumentViewModel entity, TableOfContentsViewModel tableOfContents)
        {
            Entity = entity;
            TableOfContents = tableOfContents;            
        }
    }
}