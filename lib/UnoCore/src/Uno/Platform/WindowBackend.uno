using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    using Xli;

    public abstract class WindowBackend
    {
        internal static WindowBackend Instance;

        extern(CPLUSPLUS && !MOBILE)
        static WindowBackend()
        {
            Instance = new XliWindow();
        }

        extern(DOTNET)
        public static void SetInstance(WindowBackend instance)
        {
            Instance = instance;
        }

        public abstract void Close();
        public abstract string GetTitle();
        public abstract void SetTitle(string title);
        public abstract int2 GetClientSize();
        public abstract void SetClientSize(int2 size);
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

        public virtual void SetOnscreenKeyboardPosition(int2 position)
        {
        }

        public virtual int2 GetOnscreenKeyboardPosition()
        {
            return int2(0, 0);
        }

        public virtual int2 GetOnscreenKeyboardSize()
        {
            return int2(0, 0);
        }

        public virtual bool IsStatusBarVisible()
        {
            return false;
        }

        public virtual int2 GetStatusBarPosition()
        {
            return int2(0, 0);
        }

        public virtual int2 GetStatusBarSize()
        {
            return int2(0, 0);
        }
    }
}
