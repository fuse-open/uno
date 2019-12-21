// This file was generated based on lib/UnoCore/Source/Uno/Runtime/Implementation/Internal/Bootstrapper.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Runtime.Implementation.Internal
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Bootstrapper
    {
        public static int _lastMouseX;
        public static int _lastMouseY;
        public static int _lastPrimaryFingerId;
        public static int _fingerCount;

        static Bootstrapper()
        {
            Bootstrapper._lastPrimaryFingerId = -1;
        }

        public static bool IsPrimaryFinger(global::Uno.Platform.WindowBackend window, int fingerId)
        {
            return (Bootstrapper._lastPrimaryFingerId == fingerId) && false;
        }

        public static global::Uno.Platform.EventModifiers GetEventModifiers(global::Uno.Platform.WindowBackend window)
        {
            return ((((((window.GetKeyState(global::Uno.Platform.Key.ControlKey) ? global::Uno.Platform.EventModifiers.ControlKey : 0) | (window.GetKeyState(global::Uno.Platform.Key.ShiftKey) ? global::Uno.Platform.EventModifiers.ShiftKey : 0)) | (window.GetKeyState(global::Uno.Platform.Key.AltKey) ? global::Uno.Platform.EventModifiers.AltKey : 0)) | (window.GetKeyState(global::Uno.Platform.Key.MetaKey) ? global::Uno.Platform.EventModifiers.MetaKey : 0)) | (window.GetMouseButtonState(global::Uno.Platform.MouseButton.Left) ? global::Uno.Platform.EventModifiers.LeftButton : 0)) | (window.GetMouseButtonState(global::Uno.Platform.MouseButton.Middle) ? global::Uno.Platform.EventModifiers.MiddleButton : 0)) | (window.GetMouseButtonState(global::Uno.Platform.MouseButton.Right) ? global::Uno.Platform.EventModifiers.RightButton : 0);
        }

        public static void OnUpdate()
        {
            global::Uno.Platform.TimerEventArgs standInArgs = new global::Uno.Platform.TimerEventArgs(0.0, 0.0, global::Uno.Diagnostics.Clock.GetSeconds());
            global::Uno.Platform.Displays.TickAll(standInArgs);
            global::Uno.Application.Current.Update();
        }

        public static void OnDraw()
        {
            global::Uno.Application app = global::Uno.Application.Current;
            global::Uno.Graphics.GraphicsController gc = app.GraphicsController;
            gc.SetRenderTarget(gc.Backbuffer);
            gc.Clear(gc.ClearColor, gc.ClearDepth);
            app.Draw();
        }

        public static bool OnKeyDown(global::Uno.Platform.WindowBackend window, global::Uno.Platform.Key key)
        {
            return global::Uno.Platform.EventSources.HardwareKeys.OnKeyDown(key, Bootstrapper.GetEventModifiers(window));
        }

        public static bool OnKeyUp(global::Uno.Platform.WindowBackend window, global::Uno.Platform.Key key)
        {
            return global::Uno.Platform.EventSources.HardwareKeys.OnKeyUp(key, Bootstrapper.GetEventModifiers(window));
        }

        public static bool OnTextInput(global::Uno.Platform.WindowBackend window, string text)
        {
            return global::Uno.Platform.EventSources.TextSource.OnTextInput(text, Bootstrapper.GetEventModifiers(window));
        }

        public static bool OnMouseDown(global::Uno.Platform.WindowBackend window, int x, int y, global::Uno.Platform.MouseButton button)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(window), button == global::Uno.Platform.MouseButton.Left, new global::Uno.Float2((float)x, (float)y), 0, button, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);
            return args.Handled;
        }

        public static bool OnMouseUp(global::Uno.Platform.WindowBackend window, int x, int y, global::Uno.Platform.MouseButton button)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(window), button == global::Uno.Platform.MouseButton.Left, new global::Uno.Float2((float)x, (float)y), 0, button, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);
            return args.Handled;
        }

        public static bool OnMouseMove(global::Uno.Platform.WindowBackend window, int x, int y)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.EventModifiers modifiers = Bootstrapper.GetEventModifiers(window);
            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, modifiers, modifiers.HasFlag(global::Uno.Platform.EventModifiers.LeftButton), new global::Uno.Float2((float)x, (float)y), 0, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);
            Bootstrapper._lastMouseX = x;
            Bootstrapper._lastMouseY = y;
            return args.Handled;
        }

        public static bool OnMouseOut(global::Uno.Platform.WindowBackend window)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.EventModifiers modifiers = Bootstrapper.GetEventModifiers(window);
            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, modifiers, modifiers.HasFlag(global::Uno.Platform.EventModifiers.LeftButton), new global::Uno.Float2(0.0f, 0.0f), 0, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerLeft(args);
            return args.Handled;
        }

        public static bool OnMouseWheel(global::Uno.Platform.WindowBackend window, float dHori, float dVert, int wheelDeltaMode)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(window), false, new global::Uno.Float2((float)Bootstrapper._lastMouseX, (float)Bootstrapper._lastMouseY), 0, 0, new global::Uno.Float2(dHori, dVert), (global::Uno.Platform.WheelDeltaMode)wheelDeltaMode);
            global::Uno.Platform.EventSources.MouseSource.OnPointerWheelChanged(args);
            return args.Handled;
        }

        public static bool OnTouchDown(global::Uno.Platform.WindowBackend window, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;
            Bootstrapper._fingerCount++;

            if (Bootstrapper._fingerCount == 1)
                Bootstrapper._lastPrimaryFingerId = id;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(window), Bootstrapper.IsPrimaryFinger(window, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);
            return args.Handled;
        }

        public static bool OnTouchMove(global::Uno.Platform.WindowBackend window, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (Bootstrapper._fingerCount == 1)
                Bootstrapper._lastPrimaryFingerId = id;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(window), Bootstrapper.IsPrimaryFinger(window, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);
            return args.Handled;
        }

        public static bool OnTouchUp(global::Uno.Platform.WindowBackend window, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;
            Bootstrapper._fingerCount--;

            if (Bootstrapper._lastPrimaryFingerId == id)
                Bootstrapper._lastPrimaryFingerId = -1;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(window), Bootstrapper.IsPrimaryFinger(window, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);
            return args.Handled;
        }

        public static void OnWindowSizeChanged(global::Uno.Platform.WindowBackend window)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app != null)
            {
                app.GraphicsController.UpdateBackbuffer();
                app.Window.OnResized(global::System.EventArgs.Empty);
            }
        }

        public static bool OnAppTerminating(global::Uno.Platform.WindowBackend window)
        {
            global::Uno.Platform.CoreApp.EnterBackground();
            global::Uno.Platform.CoreApp.Terminate();
            return false;
        }

        public static bool DrawNextFrame
        {
            get { return global::Uno.Application.Current.NeedsRedraw; }
        }
    }
}
