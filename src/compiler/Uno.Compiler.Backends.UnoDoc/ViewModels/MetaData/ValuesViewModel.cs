using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ValuesViewModel : List<ValueViewModel>
    {
        public ValuesViewModel(IEnumerable<ValueViewModel> values)
        {
            AddRange(values);
        }
    }
}