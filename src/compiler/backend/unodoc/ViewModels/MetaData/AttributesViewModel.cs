using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class AttributesViewModel : List<AttributeViewModel>
    {
        public AttributesViewModel(IEnumerable<AttributeViewModel> attributes)
        {
            AddRange(attributes);
        }
    }
}

