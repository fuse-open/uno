using System.Windows.Forms;
using Uno.Platform;

namespace Uno.Support.WinForms
{
    public class WinFormsPlatformWindow : WindowBackend
    {
        readonly UnoGLControl _control;

        PointerCursor _cursor = PointerCursor.Default;
        bool _isTextInputActive = false;
        float _density = 1.0f;
        
        public WinFormsPlatformWindow(UnoGLControl control)
        {
            _control = control;
        }

        public override void Close()
        {
            _control.Window.Close();
        }

        public override string GetTitle()
        {
            return _control.Window.Title;
        }

        public override void SetTitle(string title)
        {
            _control.Window.Title = title;
        }

        public override Int2 GetClientSize()
        {
            return new Int2(_control.ClientSize.Width, _control.ClientSize.Height);
        }

        public override void SetClientSize(Int2 size)
        {
            _control.Window.SetClientSize(size.X, size.Y);
        }

        public override bool GetFullscreen()
        {
            return _control.Window.IsFullscreen;
        }

        public override void SetFullscreen(bool fullscreen)
        {
            _control.Window.IsFullscreen = fullscreen;
        }

        public override void SetPointerCursor(PointerCursor p)
        {
            _cursor = p;

            if (p == PointerCursor.None)
                Cursor.Hide();
            else
                _control.Cursor = WinFormsHelper.TryGetCursor(p);
        }

        public override PointerCursor GetPointerCursor()
        {
            return _cursor;
        }

        public override bool GetMouseButtonState(MouseButton button)
        {
            return WinFormsHelper.GetMouseButtonState(button);
        }

        public override bool GetKeyState(Key key)
        {
            return WinFormsHelper.GetKeyState(key);
        }

        public override void BeginTextInput(TextInputHint hint)
        {
            _isTextInputActive = true;
        }

        public override void EndTextInput()
        {
            _isTextInputActive = false;
        }

        public override bool IsTextInputActive()
        {
            return _isTextInputActive;
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
            _density = density;
        }

        public override float GetDensity()
        {
            return _density;
        }
    }
}
