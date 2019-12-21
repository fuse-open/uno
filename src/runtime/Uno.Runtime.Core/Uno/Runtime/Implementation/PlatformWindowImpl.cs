// This file was generated based on lib/UnoCore/Source/Uno/Runtime/Implementation/PlatformWindowImpl.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Runtime.Implementation
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class PlatformWindowImpl
    {
        public static PlatformWindowHandle GetInstance()
        {
            return global::Uno.Platform.WindowBackend.Instance;
        }

        public static void Close(PlatformWindowHandle handle)
        {
            ((global::Uno.Platform.WindowBackend)handle).Close();
        }

        public static string GetTitle(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetTitle();
        }

        public static void SetTitle(PlatformWindowHandle handle, string title)
        {
            ((global::Uno.Platform.WindowBackend)handle).SetTitle(title);
        }

        public static global::Uno.Int2 GetClientSize(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetClientSize();
        }

        public static void SetClientSize(PlatformWindowHandle handle, global::Uno.Int2 clientSize)
        {
            ((global::Uno.Platform.WindowBackend)handle).SetClientSize(clientSize);
        }

        public static bool GetFullscreen(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetFullscreen();
        }

        public static void SetFullscreen(PlatformWindowHandle handle, bool fullscreen)
        {
            ((global::Uno.Platform.WindowBackend)handle).SetFullscreen(fullscreen);
        }

        public static global::Uno.Platform.PointerCursor GetPointerCursor(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetPointerCursor();
        }

        public static void SetPointerCursor(PlatformWindowHandle handle, global::Uno.Platform.PointerCursor cursor)
        {
            ((global::Uno.Platform.WindowBackend)handle).SetPointerCursor(cursor);
        }

        public static bool GetMouseButtonState(PlatformWindowHandle handle, global::Uno.Platform.MouseButton button)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetMouseButtonState(button);
        }

        public static bool GetKeyState(PlatformWindowHandle handle, global::Uno.Platform.Key key)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetKeyState(key);
        }

        public static void BeginTextInput(PlatformWindowHandle handle, global::Uno.Platform.TextInputHint hint)
        {
            ((global::Uno.Platform.WindowBackend)handle).BeginTextInput(hint);
        }

        public static void EndTextInput(PlatformWindowHandle handle)
        {
            ((global::Uno.Platform.WindowBackend)handle).EndTextInput();
        }

        public static bool IsTextInputActive(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).IsTextInputActive();
        }

        public static bool HasOnscreenKeyboardSupport(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).HasOnscreenKeyboardSupport();
        }

        public static bool IsOnscreenKeyboardVisible(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).IsOnscreenKeyboardVisible();
        }

        public static void SetOnscreenKeyboardPosition(PlatformWindowHandle handle, global::Uno.Int2 position)
        {
            ((global::Uno.Platform.WindowBackend)handle).SetOnscreenKeyboardPosition(position);
        }

        public static global::Uno.Int2 GetOnscreenKeyboardPosition(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetOnscreenKeyboardPosition();
        }

        public static global::Uno.Int2 GetOnscreenKeyboardSize(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetOnscreenKeyboardSize();
        }

        public static bool IsStatusBarVisible(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).IsStatusBarVisible();
        }

        public static global::Uno.Int2 GetStatusBarPosition(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetStatusBarPosition();
        }

        public static global::Uno.Int2 GetStatusBarSize(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetStatusBarSize();
        }

        public static float GetDensity(PlatformWindowHandle handle)
        {
            return ((global::Uno.Platform.WindowBackend)handle).GetDensity();
        }
    }
}
