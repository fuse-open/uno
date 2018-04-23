// This file was generated based on Library/Core/UnoCore/Source/Uno/Runtime/Implementation/PlatformWindowImpl.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Runtime.Implementation
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class PlatformWindowImpl
    {
        static PlatformWindowImpl()
        {
        }

        public static PlatformWindowHandle GetInstance()
        {
            return global::Uno.ApplicationContext.AppHost.GetPlatformWindow();
        }

        public static void Close(PlatformWindowHandle handle)
        {
            (handle).Close();
        }

        public static string GetTitle(PlatformWindowHandle handle)
        {
            return (handle).GetTitle();
        }

        public static void SetTitle(PlatformWindowHandle handle, string title)
        {
            (handle).SetTitle(title);
        }

        public static global::Uno.Int2 GetClientSize(PlatformWindowHandle handle)
        {
            return (handle).GetClientSize();
        }

        public static void SetClientSize(PlatformWindowHandle handle, global::Uno.Int2 clientSize)
        {
            (handle).SetClientSize(clientSize);
        }

        public static bool GetFullscreen(PlatformWindowHandle handle)
        {
            return (handle).GetFullscreen();
        }

        public static void SetFullscreen(PlatformWindowHandle handle, bool fullscreen)
        {
            (handle).SetFullscreen(fullscreen);
        }

        public static global::Uno.Platform.PointerCursor GetPointerCursor(PlatformWindowHandle handle)
        {
            return (handle).GetPointerCursor();
        }

        public static void SetPointerCursor(PlatformWindowHandle handle, global::Uno.Platform.PointerCursor cursor)
        {
            (handle).SetPointerCursor(cursor);
        }

        public static bool GetMouseButtonState(PlatformWindowHandle handle, global::Uno.Platform.MouseButton button)
        {
            return (handle).GetMouseButtonState(button);
        }

        public static bool GetKeyState(PlatformWindowHandle handle, global::Uno.Platform.Key key)
        {
            return (handle).GetKeyState(key);
        }

        public static void BeginTextInput(PlatformWindowHandle handle, global::Uno.Platform.TextInputHint hint)
        {
            (handle).BeginTextInput(hint);
        }

        public static void EndTextInput(PlatformWindowHandle handle)
        {
            (handle).EndTextInput();
        }

        public static bool IsTextInputActive(PlatformWindowHandle handle)
        {
            return (handle).IsTextInputActive();
        }

        public static bool HasOnscreenKeyboardSupport(PlatformWindowHandle handle)
        {
            return (handle).HasOnscreenKeyboardSupport();
        }

        public static bool IsOnscreenKeyboardVisible(PlatformWindowHandle handle)
        {
            return (handle).IsOnscreenKeyboardVisible();
        }

        public static void SetOnscreenKeyboardPosition(PlatformWindowHandle handle, global::Uno.Int2 position)
        {
        }

        public static global::Uno.Int2 GetOnscreenKeyboardPosition(PlatformWindowHandle handle)
        {
            return new global::Uno.Int2(0, 0);
        }

        public static global::Uno.Int2 GetOnscreenKeyboardSize(PlatformWindowHandle handle)
        {
            return new global::Uno.Int2(0, 0);
        }

        public static bool IsStatusBarVisible(PlatformWindowHandle handle)
        {
            return false;
        }

        public static global::Uno.Int2 GetStatusBarPosition(PlatformWindowHandle handle)
        {
            return new global::Uno.Int2(0, 0);
        }

        public static global::Uno.Int2 GetStatusBarSize(PlatformWindowHandle handle)
        {
            return new global::Uno.Int2(0, 0);
        }

        public static float GetDensity(PlatformWindowHandle handle)
        {
            return (handle).GetDensity();
        }
    }
}
