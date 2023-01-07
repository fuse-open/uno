using System;
using System.Drawing;
using System.Runtime.Versioning;
using AppKit;
using CoreGraphics;
using Uno.Platform;
using Uno.Platform.Internal;

namespace Uno.AppLoader.MonoMac
{
    [SupportedOSPlatform("macOS10.14")]
    class MonoMacPlatformWindow : WindowBackend
    {
        UnoGLView _view;
        Int2 _clientSize;
        float _dpi;

        public MonoMacPlatformWindow(UnoGLView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _dpi = InternalGetDensity ();
            _clientSize = InternalGetClientSize();

            _view.Resize += (object s, EventArgs a) => 
            { 
                _clientSize = InternalGetClientSize(); 
                Bootstrapper.OnWindowSizeChanged(this);
            };

            _view.Window.DidChangeScreen += (object s, EventArgs a) => 
            { 
                _dpi = InternalGetDensity(); 
                //_view.SwapBuffers(); //Update(); 
                Bootstrapper.OnWindowSizeChanged(this); // it's a hack. but everything is a hack around here.
                _view.Size = new Size(13,37); // omg, this was the only way i made the window not flicker after being moved to a new screen.. no idea why it works, also, it doesnt seem to affect the actual view size :p
            };
        }

        Int2 InternalGetClientSize()
        {
            return new Int2 ((int)(_view.Bounds.Width * GetDensity ()), (int)(_view.Bounds.Height * GetDensity ()));
        }

        float InternalGetDensity()
        {
            if (_view.Window == null)
                return 1;

            return (float)_view.Window.Screen.BackingScaleFactor;
        }

        public override void Close()
        {
            _view.Window.Close();
        }

        public override string GetTitle()
        {
            return _view.Window.Title;
        }

        public override void SetTitle(string title)
        {
            _view.Window.Title = title;
        }

        public override Uno.Int2 GetClientSize()
        {
            return _clientSize;
        }

        public override void SetClientSize(Uno.Int2 size)
        {
            _view.Bounds = new CGRect(_view.Bounds.Location, new CGSize(size.X, size.Y));
        }

        public override bool GetFullscreen()
        {
            return NSApplication.SharedApplication.PresentationOptions.HasFlag(NSApplicationPresentationOptions.FullScreen);
        }

        public override void SetFullscreen(bool fullscreen)
        {
            if (fullscreen != GetFullscreen())
                _view.Window.ToggleFullScreen(_view);
        }

        public override bool GetKeyState(Key key)
        {
            switch (key)
            {
                case Key.ControlKey:
                    return (_view.ModifierFlags & NSEventModifierMask.ControlKeyMask) != 0;
                case Key.ShiftKey:
                    return (_view.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0;
                case Key.AltKey:
                    return (_view.ModifierFlags & NSEventModifierMask.AlternateKeyMask) != 0;
                case Key.OSKey:
                case Key.MetaKey:
                    return (_view.ModifierFlags & NSEventModifierMask.CommandKeyMask) != 0;
                default:
                    return false;
            }
        }

        public override void SetPointerCursor(PointerCursor p)
        {
        }

        public override PointerCursor GetPointerCursor()
        {
            return PointerCursor.Default;
        }

        public override bool GetMouseButtonState(MouseButton button)
        {
            return (_view.PressedMouseButtons & (1 << (int)button)) != 0;
        }

        public override void BeginTextInput(TextInputHint hint)
        {
            _view.EnableText = true;
        }

        public override void EndTextInput()
        {
            _view.EnableText = false;
        }

        public override bool IsTextInputActive()
        {
            return _view.EnableText;
        }

        public override bool HasOnscreenKeyboardSupport()
        {
            return false;
        }

        public override bool IsOnscreenKeyboardVisible()
        {
            return false;
        }

        public override float GetDensity()
        {
            return _dpi;
        }
    }
}
