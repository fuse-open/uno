using System.Runtime.InteropServices;
using System.Windows.Forms;
using Uno.Platform;

namespace Uno.Support.WinForms
{
    public static class WinFormsHelper
    {
        public static Cursor TryGetCursor(PointerCursor cursor)
        {
            switch (cursor)
            {
                default:
                case PointerCursor.None:
                    return null;

                case PointerCursor.Default:
                    return Cursors.Default;

                case PointerCursor.Crosshair:
                    return Cursors.Cross;
                
                case PointerCursor.Pointer:
                    return Cursors.Hand;
                
                case PointerCursor.Help:
                    return Cursors.Help;
                
                case PointerCursor.Move:
                    return Cursors.SizeAll;
                
                case PointerCursor.Wait:
                    return Cursors.WaitCursor;
                
                case PointerCursor.Progress:
                    return Cursors.AppStarting;
                
                case PointerCursor.ResizeNorth:
                    return Cursors.PanNorth;
                
                case PointerCursor.ResizeEast:
                    return Cursors.PanEast;
                
                case PointerCursor.ResizeSouth:
                    return Cursors.PanSouth;
                
                case PointerCursor.ResizeWest:
                    return Cursors.PanWest;
                
                case PointerCursor.ResizeNorthEast:
                    return Cursors.PanNE;
                
                case PointerCursor.ResizeNorthWest:
                    return Cursors.PanNW;
                
                case PointerCursor.ResizeSouthEast:
                    return Cursors.PanSE;
                
                case PointerCursor.ResizeSouthWest:
                    return Cursors.PanSW;
                
                case PointerCursor.IBeam:
                    return Cursors.IBeam;
            }
        }

        public static bool GetMouseButtonState(MouseButton button)
        {
            Keys vKey;
            return TryGetVKey(button, out vKey) && (GetAsyncKeyState(vKey) & 0x8000) != 0;
        }

        public static bool GetKeyState(Key key)
        {
            Keys vKey;
            return TryGetVKey(key, out vKey) && (GetAsyncKeyState(vKey) & 0x8000) != 0;
        }

        static bool TryGetVKey(MouseButton button, out Keys result)
        {
            switch (button)
            {
                case MouseButton.Left:
                    result = Keys.LButton;
                    return true;

                case MouseButton.Middle:
                    result = Keys.MButton;
                    return true;

                case MouseButton.Right:
                    result = Keys.RButton;
                    return true;

                case MouseButton.X1:
                    result = Keys.XButton1;
                    return true;

                case MouseButton.X2:
                    result = Keys.XButton2;
                    return true;

                default:
                    result = 0;
                    return false;
            }
        }

        public static bool TryGetUnoMouseButton(MouseButtons b, out MouseButton result)
        {
            switch (b)
            {
                case MouseButtons.Left:
                    result = MouseButton.Left;
                    return true;

                case MouseButtons.Middle:
                    result = MouseButton.Middle;
                    return true;

                case MouseButtons.Right:
                    result = MouseButton.Right;
                    return true;

                case MouseButtons.XButton1:
                    result = MouseButton.X1;
                    return true;

                case MouseButtons.XButton2:
                    result = MouseButton.X2;
                    return true;

                case MouseButtons.None:
                default:
                    result = 0;
                    return false;
            }
        }

        static bool TryGetVKey(Key key, out Keys result)
        {
            switch (key)
            {
                case Key.Backspace:
                    result = Keys.Back;
                    return true;

                case Key.Tab:
                    result = Keys.Tab;
                    return true;

                case Key.Enter:
                    result = Keys.Enter;
                    return true;

                case Key.ShiftKey:
                    result = Keys.ShiftKey;
                    return true;

                case Key.ControlKey:
                    result = Keys.ControlKey;
                    return true;

                case Key.AltKey:
                    result = Keys.Menu;
                    return true;

                case Key.Break:
                    result = Keys.Pause;
                    return true;

                case Key.CapsLock:
                    result = Keys.Capital;
                    return true;

                case Key.Escape:
                    result = Keys.Escape;
                    return true;

                case Key.Space:
                    result = Keys.Space;
                    return true;

                case Key.PageUp:
                    result = Keys.PageUp;
                    return true;

                case Key.PageDown:
                    result = Keys.PageDown;
                    return true;

                case Key.End:
                    result = Keys.End;
                    return true;

                case Key.Home:
                    result = Keys.Home;
                    return true;

                case Key.Left:
                    result = Keys.Left;
                    return true;

                case Key.Up:
                    result = Keys.Up;
                    return true;

                case Key.Right:
                    result = Keys.Right;
                    return true;

                case Key.Down:
                    result = Keys.Down;
                    return true;

                case Key.Insert:
                    result = Keys.Insert;
                    return true;

                case Key.Delete:
                    result = Keys.Delete;
                    return true;

                case Key.D0:
                    result = Keys.D0;
                    return true;

                case Key.D1:
                    result = Keys.D1;
                    return true;

                case Key.D2:
                    result = Keys.D2;
                    return true;

                case Key.D3:
                    result = Keys.D3;
                    return true;

                case Key.D4:
                    result = Keys.D4;
                    return true;

                case Key.D5:
                    result = Keys.D5;
                    return true;

                case Key.D6:
                    result = Keys.D6;
                    return true;

                case Key.D7:
                    result = Keys.D7;
                    return true;

                case Key.D8:
                    result = Keys.D8;
                    return true;

                case Key.D9:
                    result = Keys.D9;
                    return true;

                case Key.A:
                    result = Keys.A;
                    return true;

                case Key.B:
                    result = Keys.B;
                    return true;

                case Key.C:
                    result = Keys.C;
                    return true;

                case Key.D:
                    result = Keys.D;
                    return true;

                case Key.E:
                    result = Keys.E;
                    return true;

                case Key.F:
                    result = Keys.F;
                    return true;

                case Key.G:
                    result = Keys.G;
                    return true;

                case Key.H:
                    result = Keys.H;
                    return true;

                case Key.I:
                    result = Keys.I;
                    return true;

                case Key.J:
                    result = Keys.J;
                    return true;

                case Key.K:
                    result = Keys.K;
                    return true;

                case Key.L:
                    result = Keys.L;
                    return true;

                case Key.M:
                    result = Keys.M;
                    return true;

                case Key.N:
                    result = Keys.N;
                    return true;

                case Key.O:
                    result = Keys.O;
                    return true;

                case Key.P:
                    result = Keys.P;
                    return true;

                case Key.Q:
                    result = Keys.Q;
                    return true;

                case Key.R:
                    result = Keys.R;
                    return true;

                case Key.S:
                    result = Keys.S;
                    return true;

                case Key.T:
                    result = Keys.T;
                    return true;

                case Key.U:
                    result = Keys.U;
                    return true;

                case Key.V:
                    result = Keys.V;
                    return true;

                case Key.W:
                    result = Keys.W;
                    return true;

                case Key.X:
                    result = Keys.X;
                    return true;

                case Key.Y:
                    result = Keys.Y;
                    return true;

                case Key.Z:
                    result = Keys.Z;
                    return true;

                case Key.NumPad0:
                    result = Keys.NumPad0;
                    return true;

                case Key.NumPad1:
                    result = Keys.NumPad1;
                    return true;

                case Key.NumPad2:
                    result = Keys.NumPad2;
                    return true;

                case Key.NumPad3:
                    result = Keys.NumPad3;
                    return true;

                case Key.NumPad4:
                    result = Keys.NumPad4;
                    return true;

                case Key.NumPad5:
                    result = Keys.NumPad5;
                    return true;

                case Key.NumPad6:
                    result = Keys.NumPad6;
                    return true;

                case Key.NumPad7:
                    result = Keys.NumPad7;
                    return true;

                case Key.NumPad8:
                    result = Keys.NumPad8;
                    return true;

                case Key.NumPad9:
                    result = Keys.NumPad9;
                    return true;

                case Key.F1:
                    result = Keys.F1;
                    return true;

                case Key.F2:
                    result = Keys.F2;
                    return true;

                case Key.F3:
                    result = Keys.F3;
                    return true;

                case Key.F4:
                    result = Keys.F4;
                    return true;

                case Key.F5:
                    result = Keys.F5;
                    return true;

                case Key.F6:
                    result = Keys.F6;
                    return true;

                case Key.F7:
                    result = Keys.F7;
                    return true;

                case Key.F8:
                    result = Keys.F8;
                    return true;

                case Key.F9:
                    result = Keys.F9;
                    return true;

                case Key.F10:
                    result = Keys.F10;
                    return true;

                case Key.F11:
                    result = Keys.F11;
                    return true;

                case Key.F12:
                    result = Keys.F12;
                    return true;

                case Key.MenuButton:
                    result = 0;
                    return false;

                case Key.BackButton:
                    result = 0;
                    return false;

                case Key.OSKey:
                    result = Keys.LWin; // FIXME: What about RWin?
                    return true;

                case Key.MetaKey:
                    result = Keys.ControlKey;
                    return true;

                default:
                    result = 0;
                    return false;
            }
        }

        public static bool TryGetUnoKey(Keys k, out Key result)
        {
            switch (k)
            {
                // Modifier keys
                //case Keys.Shift:
                //case Keys.Control:
                //case Keys.Alt:

                case Keys.Back:
                    result = Key.Backspace;
                    return true;

                case Keys.Tab:
                    result = Key.Tab;
                    return true;

                case Keys.Enter:
                //case Keys.Return:
                    result = Key.Enter;
                    return true;

                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    result = Key.ShiftKey;
                    return true;
                
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    result = Key.ControlKey;
                    return true;
                
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    result = Key.AltKey;
                    return true;

                case Keys.Pause:
                    result = Key.Break;
                    return true;

                case Keys.CapsLock:
                    //case Keys.Capital:
                    result = Key.CapsLock;
                    return true;

                case Keys.Escape:
                    result = Key.Escape;
                    return true;

                case Keys.Space:
                    result = Key.Space;
                    return true;

                case Keys.PageUp:
                //case Keys.Prior:
                    result = Key.PageUp;
                    return true;

                case Keys.PageDown:
                //case Keys.Next:
                    result = Key.PageDown;
                    return true;

                case Keys.End:
                    result = Key.End;
                    return true;

                case Keys.Home:
                    result = Key.Home;
                    return true;

                case Keys.Left:
                    result = Key.Left;
                    return true;

                case Keys.Up:
                    result = Key.Up;
                    return true;

                case Keys.Right:
                    result = Key.Right;
                    return true;

                case Keys.Down:
                    result = Key.Down;
                    return true;

                case Keys.Insert:
                    result = Key.Insert;
                    return true;

                case Keys.Delete:
                    result = Key.Delete;
                    return true;

                case Keys.D0:
                    result = Key.D0;
                    return true;

                case Keys.D1:
                    result = Key.D1;
                    return true;

                case Keys.D2:
                    result = Key.D2;
                    return true;

                case Keys.D3:
                    result = Key.D3;
                    return true;

                case Keys.D4:
                    result = Key.D4;
                    return true;

                case Keys.D5:
                    result = Key.D5;
                    return true;

                case Keys.D6:
                    result = Key.D6;
                    return true;

                case Keys.D7:
                    result = Key.D7;
                    return true;

                case Keys.D8:
                    result = Key.D8;
                    return true;

                case Keys.D9:
                    result = Key.D9;
                    return true;

                case Keys.A:
                    result = Key.A;
                    return true;

                case Keys.B:
                    result = Key.B;
                    return true;

                case Keys.C:
                    result = Key.C;
                    return true;

                case Keys.D:
                    result = Key.D;
                    return true;

                case Keys.E:
                    result = Key.E;
                    return true;

                case Keys.F:
                    result = Key.F;
                    return true;

                case Keys.G:
                    result = Key.G;
                    return true;

                case Keys.H:
                    result = Key.H;
                    return true;

                case Keys.I:
                    result = Key.I;
                    return true;

                case Keys.J:
                    result = Key.J;
                    return true;

                case Keys.K:
                    result = Key.K;
                    return true;

                case Keys.L:
                    result = Key.L;
                    return true;

                case Keys.M:
                    result = Key.M;
                    return true;

                case Keys.N:
                    result = Key.N;
                    return true;

                case Keys.O:
                    result = Key.O;
                    return true;

                case Keys.P:
                    result = Key.P;
                    return true;

                case Keys.Q:
                    result = Key.Q;
                    return true;

                case Keys.R:
                    result = Key.R;
                    return true;

                case Keys.S:
                    result = Key.S;
                    return true;

                case Keys.T:
                    result = Key.T;
                    return true;

                case Keys.U:
                    result = Key.U;
                    return true;

                case Keys.V:
                    result = Key.V;
                    return true;

                case Keys.W:
                    result = Key.W;
                    return true;

                case Keys.X:
                    result = Key.X;
                    return true;

                case Keys.Y:
                    result = Key.Y;
                    return true;

                case Keys.Z:
                    result = Key.Z;
                    return true;

                case Keys.NumPad0:
                    result = Key.NumPad0;
                    return true;

                case Keys.NumPad1:
                    result = Key.NumPad1;
                    return true;

                case Keys.NumPad2:
                    result = Key.NumPad2;
                    return true;

                case Keys.NumPad3:
                    result = Key.NumPad3;
                    return true;

                case Keys.NumPad4:
                    result = Key.NumPad4;
                    return true;

                case Keys.NumPad5:
                    result = Key.NumPad5;
                    return true;

                case Keys.NumPad6:
                    result = Key.NumPad6;
                    return true;

                case Keys.NumPad7:
                    result = Key.NumPad7;
                    return true;

                case Keys.NumPad8:
                    result = Key.NumPad8;
                    return true;

                case Keys.NumPad9:
                    result = Key.NumPad9;
                    return true;

                case Keys.F1:
                    result = Key.F1;
                    return true;

                case Keys.F2:
                    result = Key.F2;
                    return true;

                case Keys.F3:
                    result = Key.F3;
                    return true;

                case Keys.F4:
                    result = Key.F4;
                    return true;

                case Keys.F5:
                    result = Key.F5;
                    return true;

                case Keys.F6:
                    result = Key.F6;
                    return true;

                case Keys.F7:
                    result = Key.F7;
                    return true;

                case Keys.F8:
                    result = Key.F8;
                    return true;

                case Keys.F9:
                    result = Key.F9;
                    return true;

                case Keys.F10:
                    result = Key.F10;
                    return true;

                case Keys.F11:
                    result = Key.F11;
                    return true;

                case Keys.F12:
                    result = Key.E;
                    return true;

                case Keys.F13:
                case Keys.F14:
                case Keys.F15:
                case Keys.F16:
                case Keys.F17:
                case Keys.F18:
                case Keys.F19:
                case Keys.F20:
                case Keys.F21:
                case Keys.F22:
                case Keys.F23:
                case Keys.F24:
                    result = 0;
                    return false;

                case Keys.LWin:
                case Keys.RWin:
                    result = Key.OSKey;
                    return true;

                case Keys.Decimal:
                case Keys.Divide:
                case Keys.Add:
                case Keys.Apps:
                case Keys.Attn:
                case Keys.BrowserBack:
                case Keys.BrowserFavorites:
                case Keys.BrowserForward:
                case Keys.BrowserHome:
                case Keys.BrowserRefresh:
                case Keys.BrowserSearch:
                case Keys.BrowserStop:
                case Keys.Cancel:
                case Keys.Clear:
                case Keys.Crsel:
                case Keys.EraseEof:
                case Keys.Execute:
                case Keys.Exsel:
                case Keys.FinalMode:
                case Keys.HangulMode:
                //case Keys.HanguelMode:
                case Keys.HanjaMode:
                case Keys.Help:
                case Keys.IMEAccept:
                //case Keys.IMEAceept:
                case Keys.IMEConvert:
                case Keys.IMEModeChange:
                case Keys.IMENonconvert:
                case Keys.JunjaMode:
                //case Keys.KanaMode:
                //case Keys.KanjiMode:
                case Keys.KeyCode:
                case Keys.LButton:
                case Keys.LaunchApplication1:
                case Keys.LaunchApplication2:
                case Keys.LaunchMail:
                case Keys.LineFeed:
                case Keys.MButton:
                case Keys.MediaNextTrack:
                case Keys.MediaPlayPause:
                case Keys.MediaPreviousTrack:
                case Keys.MediaStop:
                case Keys.Modifiers:
                case Keys.Multiply:
                case Keys.NoName:
                case Keys.None:
                case Keys.NumLock:
                case Keys.Oem1:
                case Keys.Oem102:
                case Keys.Oem2:
                case Keys.Oem3:
                case Keys.Oem4:
                case Keys.Oem5:
                case Keys.Oem6:
                case Keys.Oem7:
                case Keys.Oem8:
                //case Keys.OemBackslash:
                case Keys.OemClear:
                //case Keys.OemCloseBrackets:
                //case Keys.OemMinus:
                //case Keys.OemOpenBrackets:
                //case Keys.OemPeriod:
                //case Keys.OemPipe:
                //case Keys.OemQuestion:
                //case Keys.OemQuotes:
                //case Keys.OemSemicolon:
                case Keys.Oemcomma:
                case Keys.Oemplus:
                //case Keys.Oemtilde:
                case Keys.Pa1:
                case Keys.Packet:
                case Keys.Play:
                case Keys.Print:
                case Keys.PrintScreen:
                case Keys.ProcessKey:
                case Keys.RButton:
                case Keys.Scroll:
                case Keys.Select:
                case Keys.SelectMedia:
                case Keys.Separator:
                case Keys.Sleep:
                //case Keys.Snapshot:
                case Keys.Subtract:
                case Keys.VolumeDown:
                case Keys.VolumeMute:
                case Keys.VolumeUp:
                case Keys.XButton1:
                case Keys.XButton2:
                case Keys.Zoom:
                default:
                    result = 0;
                    return false;
            }
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
    }
}
