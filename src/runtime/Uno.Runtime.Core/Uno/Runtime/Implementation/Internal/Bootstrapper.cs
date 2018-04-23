// This file was generated based on Library/Core/UnoCore/Source/Uno/Runtime/Implementation/Internal/Bootstrapper.uno.
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

        public static bool IsPrimaryFinger(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, int fingerId)
        {
            return (Bootstrapper._lastPrimaryFingerId == fingerId) && false;
        }

        public static global::Uno.Platform.EventModifiers GetEventModifiers(global::Uno.Runtime.Implementation.PlatformWindowHandle handle)
        {
            return ((((((global::Uno.Runtime.Implementation.PlatformWindowImpl.GetKeyState(handle, global::Uno.Platform.Key.ControlKey) ? global::Uno.Platform.EventModifiers.ControlKey : 0) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetKeyState(handle, global::Uno.Platform.Key.ShiftKey) ? global::Uno.Platform.EventModifiers.ShiftKey : 0)) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetKeyState(handle, global::Uno.Platform.Key.AltKey) ? global::Uno.Platform.EventModifiers.AltKey : 0)) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetKeyState(handle, global::Uno.Platform.Key.MetaKey) ? global::Uno.Platform.EventModifiers.MetaKey : 0)) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetMouseButtonState(handle, global::Uno.Platform.MouseButton.Left) ? global::Uno.Platform.EventModifiers.LeftButton : 0)) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetMouseButtonState(handle, global::Uno.Platform.MouseButton.Middle) ? global::Uno.Platform.EventModifiers.MiddleButton : 0)) | (global::Uno.Runtime.Implementation.PlatformWindowImpl.GetMouseButtonState(handle, global::Uno.Platform.MouseButton.Right) ? global::Uno.Platform.EventModifiers.RightButton : 0);
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

        public static bool OnKeyDown(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, global::Uno.Platform.Key key)
        {
            return global::Uno.Platform.EventSources.HardwareKeys.OnKeyDown(key, Bootstrapper.GetEventModifiers(handle));
        }

        public static bool OnKeyUp(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, global::Uno.Platform.Key key)
        {
            return global::Uno.Platform.EventSources.HardwareKeys.OnKeyUp(key, Bootstrapper.GetEventModifiers(handle));
        }

        public static bool OnTextInput(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, string text)
        {
            return global::Uno.Platform.EventSources.TextSource.OnTextInput(text, Bootstrapper.GetEventModifiers(handle));
        }

        public static bool OnMouseDown(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, int x, int y, global::Uno.Platform.MouseButton button)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(handle), button == global::Uno.Platform.MouseButton.Left, new global::Uno.Float2((float)x, (float)y), 0, button, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);
            return args.Handled;
        }

        public static bool OnMouseUp(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, int x, int y, global::Uno.Platform.MouseButton button)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(handle), button == global::Uno.Platform.MouseButton.Left, new global::Uno.Float2((float)x, (float)y), 0, button, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);
            return args.Handled;
        }

        public static bool OnMouseMove(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, int x, int y)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.EventModifiers modifiers = Bootstrapper.GetEventModifiers(handle);
            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, modifiers, modifiers.HasFlag(global::Uno.Platform.EventModifiers.LeftButton), new global::Uno.Float2((float)x, (float)y), 0, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);
            Bootstrapper._lastMouseX = x;
            Bootstrapper._lastMouseY = y;
            return args.Handled;
        }

        public static bool OnMouseOut(global::Uno.Runtime.Implementation.PlatformWindowHandle handle)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.EventModifiers modifiers = Bootstrapper.GetEventModifiers(handle);
            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, modifiers, modifiers.HasFlag(global::Uno.Platform.EventModifiers.LeftButton), new global::Uno.Float2(0.0f, 0.0f), 0, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerLeft(args);
            return args.Handled;
        }

        public static bool OnMouseWheel(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, float dHori, float dVert, int wheelDeltaMode)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Mouse, Bootstrapper.GetEventModifiers(handle), false, new global::Uno.Float2((float)Bootstrapper._lastMouseX, (float)Bootstrapper._lastMouseY), 0, 0, new global::Uno.Float2(dHori, dVert), (global::Uno.Platform.WheelDeltaMode)wheelDeltaMode);
            global::Uno.Platform.EventSources.MouseSource.OnPointerWheelChanged(args);
            return args.Handled;
        }

        public static bool OnTouchDown(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;
            Bootstrapper._fingerCount++;

            if (Bootstrapper._fingerCount == 1)
                Bootstrapper._lastPrimaryFingerId = id;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(handle), Bootstrapper.IsPrimaryFinger(handle, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerPressed(args);
            return args.Handled;
        }

        public static bool OnTouchMove(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (Bootstrapper._fingerCount == 1)
                Bootstrapper._lastPrimaryFingerId = id;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(handle), Bootstrapper.IsPrimaryFinger(handle, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerMoved(args);
            return args.Handled;
        }

        public static bool OnTouchUp(global::Uno.Runtime.Implementation.PlatformWindowHandle handle, float x, float y, int id)
        {
            global::Uno.Application app = global::Uno.Application.Current;
            Bootstrapper._fingerCount--;

            if (Bootstrapper._lastPrimaryFingerId == id)
                Bootstrapper._lastPrimaryFingerId = -1;

            if (app == null)
                return false;

            global::Uno.Platform.PointerEventArgs args = new global::Uno.Platform.PointerEventArgs(global::Uno.Platform.PointerType.Touch, Bootstrapper.GetEventModifiers(handle), Bootstrapper.IsPrimaryFinger(handle, id), new global::Uno.Float2(x, y), id, 0, new global::Uno.Float2(0.0f, 0.0f), global::Uno.Platform.WheelDeltaMode.DeltaPixel);
            global::Uno.Platform.EventSources.MouseSource.OnPointerReleased(args);
            return args.Handled;
        }

        public static void OnWindowSizeChanged(global::Uno.Runtime.Implementation.PlatformWindowHandle handle)
        {
            global::Uno.Application app = global::Uno.Application.Current;

            if (app != null)
            {
                app.GraphicsController.UpdateBackbuffer();
                app.Window.OnResized(global::System.EventArgs.Empty);
            }
        }

        public static bool OnAppTerminating(global::Uno.Runtime.Implementation.PlatformWindowHandle handle)
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
