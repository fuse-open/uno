// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/EventSources.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform.EventSources
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class MouseSource
    {
        public static bool _hasPointerEntered;

        public static void OnPointerPressed(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;

            if (MouseSource.PointerPressed != null)
                MouseSource.PointerPressed(origin, args);
        }

        public static void OnPointerReleased(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;

            if (MouseSource.PointerReleased != null)
                MouseSource.PointerReleased(origin, args);
        }

        public static void OnPointerMoved(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;

            if (!MouseSource._hasPointerEntered)
                MouseSource.OnPointerEntered(args);

            if (MouseSource.PointerMoved != null)
                MouseSource.PointerMoved(origin, args);
        }

        public static void OnPointerLeft(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;
            MouseSource._hasPointerEntered = false;

            if (MouseSource.PointerLeft != null)
                MouseSource.PointerLeft(origin, args);
        }

        public static void OnPointerEntered(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;
            MouseSource._hasPointerEntered = true;

            if (MouseSource.PointerEntered != null)
                MouseSource.PointerEntered(origin, args);
        }

        public static void OnPointerWheelChanged(global::Uno.Platform.PointerEventArgs args)
        {
            object origin = null;
            origin = global::Uno.Application.Current.Window;

            if (MouseSource.PointerWheelChanged != null)
                MouseSource.PointerWheelChanged(origin, args);
        }

        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerPressed;
        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerReleased;
        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerMoved;
        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerWheelChanged;
        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerLeft;
        public static event global::System.EventHandler<global::Uno.Platform.PointerEventArgs> PointerEntered;
    }
}
