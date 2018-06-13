using OpenGL;
using Uno.Platform;
using Uno.Diagnostics;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.Implementation.Internal
{
    [extern(DOTNET) DotNetType]
    public extern(!MOBILE) static class Bootstrapper
    {
        // Note: Ugly global state
        static int _lastMouseX;
        static int _lastMouseY;

        static int _lastPrimaryFingerId;
        static int _fingerCount;

        static Bootstrapper()
        {
            _lastPrimaryFingerId = -1;
        }

        static bool IsPrimaryFinger(PlatformWindowHandle handle, int fingerId)
        {
            return _lastPrimaryFingerId == fingerId && (defined(Android) || defined(iOS));
        }

        static EventModifiers GetEventModifiers(PlatformWindowHandle handle)
        {
            return
                (PlatformWindowImpl.GetKeyState(handle, Key.ControlKey) ? EventModifiers.ControlKey : 0) |
                (PlatformWindowImpl.GetKeyState(handle, Key.ShiftKey) ? EventModifiers.ShiftKey : 0) |
                (PlatformWindowImpl.GetKeyState(handle, Key.AltKey) ? EventModifiers.AltKey : 0) |
                (PlatformWindowImpl.GetKeyState(handle, Key.MetaKey) ? EventModifiers.MetaKey : 0) |
                (PlatformWindowImpl.GetMouseButtonState(handle, MouseButton.Left) ? EventModifiers.LeftButton : 0) |
                (PlatformWindowImpl.GetMouseButtonState(handle, MouseButton.Middle) ? EventModifiers.MiddleButton : 0) |
                (PlatformWindowImpl.GetMouseButtonState(handle, MouseButton.Right) ? EventModifiers.RightButton : 0);
        }

        public static void OnUpdate()
        {
            // {TODO} put real data in this arg
            var standInArgs = new TimerEventArgs(0, 0, Uno.Diagnostics.Clock.GetSeconds());
            Uno.Platform.Displays.TickAll(standInArgs);
            Application.Current.Update();
        }

        // The actual Application.DrawNextFrame is protected internal
        // but the value is needed by the preview engine bootstrapper
        // Adding this hook for now
        public static bool DrawNextFrame
        {
            get { return Application.Current.NeedsRedraw; }
        }

        public static void OnDraw()
        {
            var app = Application.Current;
            var gc = app.GraphicsController;

            gc.SetRenderTarget(gc.Backbuffer);
            gc.Clear(gc.ClearColor, gc.ClearDepth);

            app.Draw();
        }

        public static bool OnKeyDown(PlatformWindowHandle handle, Key key)
        {
            return Uno.Platform.EventSources.HardwareKeys.OnKeyDown(key, GetEventModifiers(handle));
        }

        public static bool OnKeyUp(PlatformWindowHandle handle, Key key)
        {
            return Uno.Platform.EventSources.HardwareKeys.OnKeyUp(key, GetEventModifiers(handle));
        }

        public static bool OnTextInput(PlatformWindowHandle handle, string text)
        {
            return Uno.Platform.EventSources.TextSource.OnTextInput(text, GetEventModifiers(handle));
        }

        public static bool OnMouseDown(PlatformWindowHandle handle, int x, int y, MouseButton button)
        {
            var app = Application.Current;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Mouse, GetEventModifiers(handle), button == MouseButton.Left, float2((float)x, (float)y), 0, button, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);

            return args.Handled;
        }

        public static bool OnMouseUp(PlatformWindowHandle handle, int x, int y, MouseButton button)
        {
            var app = Application.Current;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Mouse, GetEventModifiers(handle), button == MouseButton.Left, float2((float)x, (float)y), 0, button, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);

            return args.Handled;
        }

        public static bool OnMouseMove(PlatformWindowHandle handle, int x, int y)
        {
            var app = Application.Current;

            if (app == null)
                return false;

            var modifiers = GetEventModifiers(handle);

            var args = new PointerEventArgs(PointerType.Mouse, modifiers, modifiers.HasFlag(EventModifiers.LeftButton), float2((float)x, (float)y), 0, 0, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);

            _lastMouseX = x;
            _lastMouseY = y;

            return args.Handled;
        }

        public static bool OnMouseOut(PlatformWindowHandle handle)
        {
            var app = Application.Current;

            if (app == null)
                return false;

            var modifiers = GetEventModifiers(handle);

            var args = new PointerEventArgs(PointerType.Mouse, modifiers, modifiers.HasFlag(EventModifiers.LeftButton), float2((float)0, (float)0), 0, 0, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerLeft(args);

            return args.Handled;
        }

        public static bool OnMouseWheel(PlatformWindowHandle handle, float dHori, float dVert, int wheelDeltaMode)
        {
            var app = Application.Current;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Mouse, GetEventModifiers(handle), false, float2((float)_lastMouseX, (float)_lastMouseY), 0, 0, float2(dHori, dVert), (WheelDeltaMode)wheelDeltaMode);
            Uno.Platform.EventSources.MouseSource.OnPointerWheelChanged(args);

            return args.Handled;
        }

        public static bool OnTouchDown(PlatformWindowHandle handle, float x, float y, int id)
        {
            var app = Application.Current;

            _fingerCount++;

            if (_fingerCount == 1)
                _lastPrimaryFingerId = id;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Touch, GetEventModifiers(handle), IsPrimaryFinger(handle, id), float2(x, y), id, 0, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);

            return args.Handled;
        }

        public static bool OnTouchMove(PlatformWindowHandle handle, float x, float y, int id)
        {
            var app = Application.Current;

            if (_fingerCount == 1)
                _lastPrimaryFingerId = id;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Touch, GetEventModifiers(handle), IsPrimaryFinger(handle, id), float2(x, y), id, 0, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);

            return args.Handled;
        }

        public static bool OnTouchUp(PlatformWindowHandle handle, float x, float y, int id)
        {
            var app = Application.Current;

            _fingerCount--;

            if (_lastPrimaryFingerId == id)
                _lastPrimaryFingerId = -1;

            if (app == null)
                return false;

            var args = new PointerEventArgs(PointerType.Touch, GetEventModifiers(handle), IsPrimaryFinger(handle, id), float2(x, y), id, 0, float2(0, 0), WheelDeltaMode.DeltaPixel);
            Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);

            return args.Handled;
        }

        public static void OnWindowSizeChanged(PlatformWindowHandle handle)
        {
            var app = Application.Current;

            if (app != null)
            {
                app.GraphicsController.UpdateBackbuffer();
                app.Window.OnResized(EventArgs.Empty);
            }
        }

        public static bool OnAppTerminating(PlatformWindowHandle handle)
        {
            CoreApp.EnterBackground();
            CoreApp.Terminate();
            return false;
        }
    }
}
