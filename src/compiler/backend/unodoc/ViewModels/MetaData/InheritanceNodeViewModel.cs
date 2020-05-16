using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class InheritanceNodeViewModel
    {
        public string Uri { get; private set; }
        public string Title { get; private set; }
        public List<InheritanceNodeViewModel> Children { get; } 
        public bool IsAncestor { get; private set; }
        public bool IsDescendant { get; private set; }
        public bool IsCurrent { get; private set; }

        public InheritanceNodeViewModel(string uri,
                                        string title,
                                        List<InheritanceNodeViewModel> children,
                                        bool isAncestor,
                                        bool isDescendant,
                                        bool isCurrent)
        {
            Uri = uri;
            Title = title;
            Children = children;
            IsAncestor = isAncestor;
            IsDescendant = isDescendant;
            IsCurrent = isCurrent;
        }
    }
}
