using Uno.Platform;

namespace Uno.AppLoader.Dummy
{
    public class DummyWindow : WindowBackend
    {
        public override void Close()
        {
        }

        public override string GetTitle()
        {
            return "";
        }

        public override void SetTitle(string title)
        {
        }

        public override Int2 GetClientSize()
        {
            return new Int2(0, 0);
        }

        public override void SetClientSize(Int2 size)
        {
        }

        public override bool GetFullscreen()
        {
            return false;
        }

        public override void SetFullscreen(bool fullscreen)
        {
        }

        public override void SetPointerCursor(PointerCursor p)
        {
        }

        public override PointerCursor GetPointerCursor()
        {
            return PointerCursor.None;
        }

        public override bool GetMouseButtonState(MouseButton button)
        {
            return false;
        }

        public override bool GetKeyState(Key key)
        {
            return false;
        }

        public override void BeginTextInput(TextInputHint hint)
        {
        }

        public override void EndTextInput()
        {
        }

        public override bool IsTextInputActive()
        {
            return false;
        }

        public override bool HasOnscreenKeyboardSupport()
        {
            return false;
        }

        public override bool IsOnscreenKeyboardVisible()
        {
            return false;
        }

        public void SetDensity(float density)
        {
        }

        public override float GetDensity()
        {
            return 1;
        }
    }
}
