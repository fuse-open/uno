using Uno.Compiler.ExportTargetInterop;

// 'Uno.Compiler.ExportTargetInterop.Android.ExternBlockHost' cannot
//  be extended because it does not specify
// 'Uno.Compiler.ExportTargetInterop.TargetSpecificImplementationAttribute'

namespace Uno.Compiler.ExportTargetInterop.Foreign.Android
{
    [TargetSpecificImplementation]
    internal extern(Android) static class ExternBlockHost
    {
        [TargetSpecificImplementation]
        static extern void RegisterFunctions();
    }
}
