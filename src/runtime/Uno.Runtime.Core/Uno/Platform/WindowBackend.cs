// This file was generated based on lib/UnoCore/Source/Uno/Platform/WindowBackend.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public abstract class WindowBackend : global::Uno.Runtime.Implementation.PlatformWindowHandle
    {
        public static WindowBackend Instance;

        public WindowBackend()
        {
        }

        public static void SetInstance(WindowBackend instance)
        {
            WindowBackend.Instance = instance;
        }

        public abstract void Close();

        public abstract string GetTitle();

        public abstract void SetTitle(string title);

        public abstract global::Uno.Int2 GetClientSize();

        public abstract void SetClientSize(global::Uno.Int2 size);

        public abstract bool GetFullscreen();

        public abstract void SetFullscreen(bool fullscreen);

        public abstract void SetPointerCursor(PointerCursor p);

        public abstract PointerCursor GetPointerCursor();

        public abstract bool GetMouseButtonState(MouseButton button);

        public abstract bool GetKeyState(Key key);

        public abstract void BeginTextInput(TextInputHint hint);

        public abstract void EndTextInput();

        public abstract bool IsTextInputActive();

        public abstract bool HasOnscreenKeyboardSupport();

        public abstract bool IsOnscreenKeyboardVisible();

        public abstract float GetDensity();

        public virtual void SetOnscreenKeyboardPosition(global::Uno.Int2 position)
        {
        }

        public virtual global::Uno.Int2 GetOnscreenKeyboardPosition()
        {
            return new global::Uno.Int2(0, 0);
        }

        public virtual global::Uno.Int2 GetOnscreenKeyboardSize()
        {
            return new global::Uno.Int2(0, 0);
        }

        public virtual bool IsStatusBarVisible()
        {
            return false;
        }

        public virtual global::Uno.Int2 GetStatusBarPosition()
        {
            return new global::Uno.Int2(0, 0);
        }

        public virtual global::Uno.Int2 GetStatusBarSize()
        {
            return new global::Uno.Int2(0, 0);
        }
    }
}
