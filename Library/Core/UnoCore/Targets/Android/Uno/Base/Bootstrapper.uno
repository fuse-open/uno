using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Android
{
    [TargetSpecificImplementation]
    [Require("Source.Include","BootstrapperImpl_Android.h")]
    extern(ANDROID)
    internal static class Bootstrapper
    {
        public static void _RegisterTypes()
        @{
            ::BootstrapperImpl();
        @}
    }
}
