using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class ImplementedInterfacesViewModel : List<ImplementedInterfaceViewModel>
    {
        public ImplementedInterfacesViewModel(IEnumerable<ImplementedInterfaceViewModel> ifaces)
        {
            AddRange(ifaces);
        }
    }
}