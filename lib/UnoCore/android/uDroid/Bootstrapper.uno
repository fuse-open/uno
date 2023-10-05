using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Android
{
    [TargetSpecificImplementation]
    [Require("source.include", "BootstrapperImpl_Android.h")]
    extern(ANDROID)
    internal static class Bootstrapper
    {
        public static void _RegisterTypes()
        @{
            ::BootstrapperImpl();
        @}
    }
}
