using System.Runtime.Versioning;
using AppKit;
using Uno.Platform;

namespace Uno.AppLoader.MonoMac
{
    [SupportedOSPlatform("macOS10.14")]
    public static class MonoMacEnums
    {
        public static NSAlertStyle GetAlertStyle(Diagnostics.LogLevel level)
        {
            switch (level)
            {
                case Diagnostics.LogLevel.Error:
                    return NSAlertStyle.Critical;
                case Diagnostics.LogLevel.Warning:
                    return NSAlertStyle.Warning;
                case Diagnostics.LogLevel.Information:
                    return NSAlertStyle.Informational;
                default:
                    return 0;
            }
        }

        public static bool TryGetUnoMouseButton(nint button, out MouseButton result)
        {
            switch (button)
            {
                case 0:
                    result = MouseButton.Left;
                    return true;
                case 1:
                    result = MouseButton.Right;
                    return true;
                case 2:
                    result = MouseButton.Middle;
                    return true;
                case 3:
                    result = MouseButton.X1;
                    return true;
                case 4:
                    result = MouseButton.X2;
                    return true;
                default:
                    result = 0;
                    return false;
            }
        }

        public static bool TryGetUnoKey(NSKey key, out Key result)
        {
            var functionKey = (NSFunctionKey)key;
            switch(functionKey)
            {
                case NSFunctionKey.Menu:
                    result = Key.AltKey;
                    return true;
                case NSFunctionKey.Break:
                    result = Key.Break;
                    return true;
                case NSFunctionKey.LeftArrow:
                    result = Key.Left;
                    return true;
                case NSFunctionKey.UpArrow:
                    result = Key.Up;
                    return true;
                case NSFunctionKey.RightArrow:
                    result = Key.Right;
                    return true;
                case NSFunctionKey.DownArrow:
                    result = Key.Down;
                    return true;
                case NSFunctionKey.Insert:
                    result = Key.Insert;
                    return true;
            }

            switch (key)
            {
                case NSKey.Delete:
                    result = Key.Backspace;
                    return true;
                case NSKey.Tab:
                    result = Key.Tab;
                    return true;
                case NSKey.Return:
                    result = Key.Enter;
                    return true;
                case NSKey.Shift:
                case NSKey.RightShift:
                    result = Key.ShiftKey;
                    return true;
                case NSKey.Control:
                case NSKey.RightControl:
                    result = Key.ControlKey;
                    return true;
                case NSKey.CapsLock:
                    result = Key.CapsLock;
                    return true;
                case NSKey.Escape:
                    result = Key.Escape;
                    return true;
                case NSKey.Space:
                    result = Key.Space;
                    return true;
                case NSKey.PageUp:
                    result = Key.PageUp;
                    return true;
                case NSKey.PageDown:
                    result = Key.PageDown;
                    return true;
                case NSKey.End:
                    result = Key.End;
                    return true;
                case NSKey.Home:
                    result = Key.Home;
                    return true;
                case NSKey.ForwardDelete:
                    result = Key.Delete;
                    return true;
                case NSKey.D0:
                    result = Key.D0;
                    return true;
                case NSKey.D1:
                    result = Key.D1;
                    return true;
                case NSKey.D2:
                    result = Key.D2;
                    return true;
                case NSKey.D3:
                    result = Key.D3;
                    return true;
                case NSKey.D4:
                    result = Key.D4;
                    return true;
                case NSKey.D5:
                    result = Key.D5;
                    return true;
                case NSKey.D6:
                    result = Key.D6;
                    return true;
                case NSKey.D7:
                    result = Key.D7;
                    return true;
                case NSKey.D8:
                    result = Key.D8;
                    return true;
                case NSKey.D9:
                    result = Key.D9;
                    return true;
                case NSKey.A:
                    result = Key.A;
                    return true;
                case NSKey.B:
                    result = Key.B;
                    return true;
                case NSKey.C:
                    result = Key.C;
                    return true;
                case NSKey.D:
                    result = Key.D;
                    return true;
                case NSKey.E:
                    result = Key.E;
                    return true;
                case NSKey.F:
                    result = Key.F;
                    return true;
                case NSKey.G:
                    result = Key.G;
                    return true;
                case NSKey.H:
                    result = Key.H;
                    return true;
                case NSKey.I:
                    result = Key.I;
                    return true;
                case NSKey.J:
                    result = Key.J;
                    return true;
                case NSKey.K:
                    result = Key.K;
                    return true;
                case NSKey.L:
                    result = Key.L;
                    return true;
                case NSKey.M:
                    result = Key.M;
                    return true;
                case NSKey.N:
                    result = Key.N;
                    return true;
                case NSKey.O:
                    result = Key.O;
                    return true;
                case NSKey.P:
                    result = Key.P;
                    return true;
                case NSKey.Q:
                    result = Key.Q;
                    return true;
                case NSKey.R:
                    result = Key.R;
                    return true;
                case NSKey.S:
                    result = Key.S;
                    return true;
                case NSKey.T:
                    result = Key.T;
                    return true;
                case NSKey.U:
                    result = Key.U;
                    return true;
                case NSKey.V:
                    result = Key.V;
                    return true;
                case NSKey.W:
                    result = Key.W;
                    return true;
                case NSKey.X:
                    result = Key.X;
                    return true;
                case NSKey.Y:
                    result = Key.Y;
                    return true;
                case NSKey.Z:
                    result = Key.Z;
                    return true;
                case NSKey.Keypad0:
                    result = Key.NumPad0;
                    return true;
                case NSKey.Keypad1:
                    result = Key.NumPad1;
                    return true;
                case NSKey.Keypad2:
                    result = Key.NumPad2;
                    return true;
                case NSKey.Keypad3:
                    result = Key.NumPad3;
                    return true;
                case NSKey.Keypad4:
                    result = Key.NumPad4;
                    return true;
                case NSKey.Keypad5:
                    result = Key.NumPad5;
                    return true;
                case NSKey.Keypad6:
                    result = Key.NumPad6;
                    return true;
                case NSKey.Keypad7:
                    result = Key.NumPad7;
                    return true;
                case NSKey.Keypad8:
                    result = Key.NumPad8;
                    return true;
                case NSKey.Keypad9:
                    result = Key.NumPad9;
                    return true;
                case NSKey.F1:
                    result = Key.F1;
                    return true;
                case NSKey.F2:
                    result = Key.F2;
                    return true;
                case NSKey.F3:
                    result = Key.F3;
                    return true;
                case NSKey.F4:
                    result = Key.F4;
                    return true;
                case NSKey.F5:
                    result = Key.F5;
                    return true;
                case NSKey.F6:
                    result = Key.F6;
                    return true;
                case NSKey.F7:
                    result = Key.F7;
                    return true;
                case NSKey.F8:
                    result = Key.F8;
                    return true;
                case NSKey.F9:
                    result = Key.F9;
                    return true;
                case NSKey.F10:
                    result = Key.F10;
                    return true;
                case NSKey.F11:
                    result = Key.F11;
                    return true;
                case NSKey.F12:
                    result = Key.F12;
                    return true;
                case NSKey.Command:
                    result = Key.OSKey;
                    return true;
                default:
                    result = 0;
                    return false;
            }
        }
    }
}

