using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [TargetSpecificImplementation]
    extern(APPLE) static class iOSHttpSharedCache
    {
        [TargetSpecificImplementation]
        public static extern void SetupSharedCache(
            bool isCacheEnabled = true, long sizeInBytes = 0);

        [TargetSpecificImplementation]
        public static extern void PurgeSharedCache();
    }
}
