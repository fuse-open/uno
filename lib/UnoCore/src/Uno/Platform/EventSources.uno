using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform.EventSources
{
    public static class InterAppInvoke
    {
        public static event EventHandler<string> ReceivedURI;
        static string DelayedUri = "";
        internal static void OnReceivedURI(string Uri)
        {
            if (CoreApp.State == ApplicationState.Foreground || CoreApp.State == ApplicationState.Interactive)
            {
                EventHandler<string> handler = ReceivedURI;
                if (handler != null)
                    handler(null, Uri);
            }
            else
            {
                DelayedUri = Uri;
                CoreApp.EnteringForeground += DispatchDelayedUri;
            }
        }
        static void DispatchDelayedUri(ApplicationState state)
        {
            EventHandler<string> handler = ReceivedURI;
            if (handler != null)
                handler(null, DelayedUri);
            DelayedUri = "";
            CoreApp.EnteringForeground -= DispatchDelayedUri;
        }
    }


    //
    // Hardware Keys
    //
    public static class HardwareKeys
    {
        public static event EventHandler<Uno.Platform.KeyEventArgs> KeyDown;
        public static event EventHandler<Uno.Platform.KeyEventArgs> KeyUp;

        internal static bool OnKeyDown(Uno.Platform.Key key, Uno.Platform.EventModifiers modifiers)
        {
            var args = new Uno.Platform.KeyEventArgs(key, modifiers);
            EventHandler<Uno.Platform.KeyEventArgs> handler = KeyDown;
            if (handler != null)
                handler(null, args);

            return args.Handled;
        }

        internal static bool OnKeyUp(Uno.Platform.Key key, Uno.Platform.EventModifiers modifiers)
        {
            var args = new Uno.Platform.KeyEventArgs(key, modifiers);
            EventHandler<Uno.Platform.KeyEventArgs> handler = KeyUp;
            if (handler != null)
                handler(null, args);

            return args.Handled;
        }
    }

    public static class TextSource
    {
        public static event EventHandler<TextInputEventArgs> TextInput;

        public static extern(!MOBILE) void BeginTextInput(TextInputHint hint)
        {
            Application.Current.Window.Backend.BeginTextInput(hint);
        }

        public static extern(!MOBILE) void EndTextInput()
        {
            Application.Current.Window.Backend.EndTextInput();
        }

        public static extern(MOBILE) void BeginTextInput(TextInputHint hint) {}
        public static extern(MOBILE) void EndTextInput() {}

        public static bool IsTextInputActive
        {
            get
            {
                if defined(MOBILE)
                {
                    return false;
                }
                else
                {
                    return Application.Current.Window.Backend.IsTextInputActive();
                }
            }
        }

        internal static bool OnTextInput(string text, Uno.Platform.EventModifiers modifiers)
        {
            // global::Uno.Application app = global::Uno.Application.Current;
            // var origin = (app == null) ? null : app.Window;
            var args = new TextInputEventArgs(text, modifiers);
            EventHandler<TextInputEventArgs> handler = TextInput;
            if (handler != null)
                handler(null, args);

            return args.Handled;
        }
    }

    public static class MouseSource
    {
        public static event EventHandler<PointerEventArgs> PointerPressed;
        public static event EventHandler<PointerEventArgs> PointerReleased;
        public static event EventHandler<PointerEventArgs> PointerMoved;
        public static event EventHandler<PointerEventArgs> PointerWheelChanged;
        public static event EventHandler<PointerEventArgs> PointerLeft;
        public static event EventHandler<PointerEventArgs> PointerEntered;

        internal static void OnPointerPressed(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            if (PointerPressed != null)
                PointerPressed(origin, args);
        }

        internal static void OnPointerReleased(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            if (PointerReleased != null)
                PointerReleased(origin, args);
        }

        static bool _hasPointerEntered = false;
        internal static void OnPointerMoved(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            if(!_hasPointerEntered)
                OnPointerEntered(args);

            if (PointerMoved != null)
                PointerMoved(origin, args);
        }

        internal static void OnPointerLeft(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            _hasPointerEntered = false;
            if (PointerLeft != null)
                PointerLeft(origin, args);
        }

        internal static void OnPointerEntered(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            _hasPointerEntered = true;
            if (PointerEntered != null)
                PointerEntered(origin, args);
        }

        internal static void OnPointerWheelChanged(PointerEventArgs args)
        {
            object origin = null;
            if defined(!MOBILE)
                origin = Uno.Application.Current.Window;

            if (PointerWheelChanged != null)
                PointerWheelChanged(origin, args);
        }
    }
}
