using System;
using System.Drawing;
using AppKit;
using CoreGraphics;
using Uno.Platform;
using Uno.Platform.Internal;

namespace Uno.Support.MonoMac
{
    // Be sure what you do when changing the implementation. MonoMacGameView getters and setters is very expensive, 
    // since they communicate with the OS
    class MonoMacPlatformWindow : WindowBackend
    {
        UnoGLView _view;
        Uno.Int2 _clientSize;
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

        public override bool GetKeyState(Uno.Platform.Key key)
        {
            switch (key)
            {
                case Uno.Platform.Key.ControlKey:
                    return (_view.ModifierFlags & NSEventModifierMask.ControlKeyMask) != 0;
                case Uno.Platform.Key.ShiftKey:
                    return (_view.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0;
                case Uno.Platform.Key.AltKey:
                    return (_view.ModifierFlags & NSEventModifierMask.AlternateKeyMask) != 0;
                case Uno.Platform.Key.OSKey:
                case Uno.Platform.Key.MetaKey:
                    return (_view.ModifierFlags & NSEventModifierMask.CommandKeyMask) != 0;
                default:
                    return false;
            }
        }

        public override void SetPointerCursor(Uno.Platform.PointerCursor p)
        {
        }

        public override Uno.Platform.PointerCursor GetPointerCursor()
        {
            return Uno.Platform.PointerCursor.Default;
        }

        public override bool GetMouseButtonState(Uno.Platform.MouseButton button)
        {
            return (_view.PressedMouseButtons & (1 << (int)button)) != 0;
        }

        public override void BeginTextInput(Uno.Platform.TextInputHint hint)
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
