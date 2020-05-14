using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class AttributeViewModel : DocumentReferenceViewModel
    {
        public List<string> Parameters { get; }

        public AttributeViewModel(DocumentIdViewModel id,
                                  DocumentUriViewModel uri,
                                  IndexTitlesViewModel titles,
                                  List<string> parameters)
            : base(id, uri, titles)
        {
            Parameters = parameters;
        }

        public bool ShouldSerializeParameters()
        {
            return Parameters != null && Parameters.Count > 0;
        }
    }
}

