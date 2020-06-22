using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform2.Internal
{
    // The purpose of this class is to make Uno.Platform.CoreApp
    // entrypoints accessible from C# assemblies and the fusetool without making
    // the entrypoints otherwise part of the public API for Platform2.

    public extern(DOTNET) static class Application
    {
        public static void Start() { Uno.Platform.CoreApp.Start(); }

        public static void EnterForeground() { Uno.Platform.CoreApp.EnterForeground(); }
        public static void EnterInteractive() { Uno.Platform.CoreApp.EnterInteractive(); }
        public static void ExitInteractive() { Uno.Platform.CoreApp.ExitInteractive(); }
        public static void EnterBackground() { Uno.Platform.CoreApp.EnterBackground(); }

        public static void Terminate() { Uno.Platform.CoreApp.Terminate(); }

        public static void OnReceivedLowMemoryWarning() { Uno.Platform.CoreApp.OnReceivedLowMemoryWarning(); }
    }
}
