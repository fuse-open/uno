using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ParametersViewModel : List<ParameterViewModel>
    {
        public ParametersViewModel(IEnumerable<ParameterViewModel> parameters)
        {
            AddRange(parameters);
        }
    }
}
