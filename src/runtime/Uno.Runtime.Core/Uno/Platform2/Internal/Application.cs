// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform2/Internal/Application.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform2.Internal
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Application
    {
        public static void Start()
        {
            global::Uno.Platform.CoreApp.Start();
        }

        public static void EnterForeground()
        {
            global::Uno.Platform.CoreApp.EnterForeground();
        }

        public static void EnterInteractive()
        {
            global::Uno.Platform.CoreApp.EnterInteractive();
        }

        public static void ExitInteractive()
        {
            global::Uno.Platform.CoreApp.ExitInteractive();
        }

        public static void EnterBackground()
        {
            global::Uno.Platform.CoreApp.EnterBackground();
        }

        public static void Terminate()
        {
            global::Uno.Platform.CoreApp.Terminate();
        }

        public static void OnReceivedLowMemoryWarning()
        {
            global::Uno.Platform.CoreApp.OnReceivedLowMemoryWarning();
        }
    }
}
