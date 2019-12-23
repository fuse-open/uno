using Uno.Compiler.ExportTargetInterop;
using Uno.Platform;

namespace Uno.Runtime.Implementation
{
    [extern(DOTNET) DotNetType]
    public abstract class PlatformWindowHandle
    {
    }

    [extern(DOTNET) DotNetType]
    public extern(!MOBILE) static class PlatformWindowImpl
    {
        public static PlatformWindowHandle GetInstance()
        {
            return WindowBackend.Instance;
        }

        public static void Close(PlatformWindowHandle handle)
        {
            ((WindowBackend)handle).Close();
        }

        public static string GetTitle(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetTitle();
        }

        public static void SetTitle(PlatformWindowHandle handle, string title)
        {
            ((WindowBackend)handle).SetTitle(title);
        }

        public static int2 GetClientSize(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetClientSize();
        }

        public static void SetClientSize(PlatformWindowHandle handle, int2 clientSize)
        {
            ((WindowBackend)handle).SetClientSize(clientSize);
        }

        public static bool GetFullscreen(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetFullscreen();
        }

        public static void SetFullscreen(PlatformWindowHandle handle, bool fullscreen)
        {
            ((WindowBackend)handle).SetFullscreen(fullscreen);
        }

        public static Platform.PointerCursor GetPointerCursor(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetPointerCursor();
        }

        public static void SetPointerCursor(PlatformWindowHandle handle, Platform.PointerCursor cursor)
        {
            ((WindowBackend)handle).SetPointerCursor(cursor);
        }

        public static bool GetMouseButtonState(PlatformWindowHandle handle, Platform.MouseButton button)
        {
            return ((WindowBackend)handle).GetMouseButtonState(button);
        }

        public static bool GetKeyState(PlatformWindowHandle handle, Platform.Key key)
        {
            return ((WindowBackend)handle).GetKeyState(key);
        }

        public static void BeginTextInput(PlatformWindowHandle handle, Platform.TextInputHint hint)
        {
            ((WindowBackend)handle).BeginTextInput(hint);
        }

        public static void EndTextInput(PlatformWindowHandle handle)
        {
            ((WindowBackend)handle).EndTextInput();
        }

        public static bool IsTextInputActive(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).IsTextInputActive();
        }

        public static bool HasOnscreenKeyboardSupport(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).HasOnscreenKeyboardSupport();
        }

        public static bool IsOnscreenKeyboardVisible(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).IsOnscreenKeyboardVisible();
        }

        public static void SetOnscreenKeyboardPosition(PlatformWindowHandle handle, int2 position)
        {
            ((WindowBackend)handle).SetOnscreenKeyboardPosition(position);
        }

        public static int2 GetOnscreenKeyboardPosition(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetOnscreenKeyboardPosition();
        }

        public static int2 GetOnscreenKeyboardSize(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetOnscreenKeyboardSize();
        }

        public static bool IsStatusBarVisible(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).IsStatusBarVisible();
        }

        public static int2 GetStatusBarPosition(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetStatusBarPosition();
        }

        public static int2 GetStatusBarSize(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetStatusBarSize();
        }

        public static float GetDensity(PlatformWindowHandle handle)
        {
            return ((WindowBackend)handle).GetDensity();
        }
    }
}
