using System;
using System.Runtime.InteropServices;

namespace Uno.Runtime.Implementation
{
    public abstract class PlatformWindowHandle
    {
        public abstract void Close();
        public abstract string GetTitle();
        public abstract void SetTitle(string title);
        public abstract Int2 GetClientSize();
        public abstract void SetClientSize(Int2 size);
        public abstract bool GetFullscreen();
        public abstract void SetFullscreen(bool fullscreen);
        public abstract void SetPointerCursor(Uno.Platform.PointerCursor p);
        public abstract Uno.Platform.PointerCursor GetPointerCursor();
        public abstract bool GetMouseButtonState(Platform.MouseButton button);
        public abstract bool GetKeyState(Platform.Key key);
        public abstract void BeginTextInput(Platform.TextInputHint hint);
        public abstract void EndTextInput();
        public abstract bool IsTextInputActive();
        public abstract bool HasOnscreenKeyboardSupport();
        public abstract bool IsOnscreenKeyboardVisible();
        public abstract float GetDensity();
    }
}
