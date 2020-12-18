using OpenGL;
using Uno.Collections;
using Uno.Graphics;
using Uno.Platform;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public enum ApplicationState
    {
        Uninitialized,
        Background,     // Not rendering
        Foreground,     // Rendering, not interactive
        Interactive,
        Terminating = -1
    }

    public delegate void ApplicationStateTransitionHandler(ApplicationState newState);

    [extern(CPlusPlus) Require("Source.Declaration","void uInitRtti();")]
    [extern(CPlusPlus) Require("Source.Declaration","void uStartApp();")]
    public abstract class CoreApp
    {
        public static CoreApp Current
        {
            get;
            internal set;
        }

        protected CoreApp()
        {
            if defined(Android)
                Android.Bootstrapper._RegisterTypes();

            if(Current == null)
                Current = this;
        }

        public virtual void Load()
        {
        }

        //
        // Lifecycle
        //
        public static ApplicationState State { get; private set; }

        public static event ApplicationStateTransitionHandler Started;
        public static event ApplicationStateTransitionHandler EnteringForeground;
        public static event ApplicationStateTransitionHandler EnteringInteractive;
        public static event ApplicationStateTransitionHandler ExitedInteractive;
        public static event ApplicationStateTransitionHandler EnteringBackground;
        public static event ApplicationStateTransitionHandler Terminating;

        internal static void Start()
        {
            switch (State)
            {
            case ApplicationState.Background:
                Terminate();
                break;

            case ApplicationState.Uninitialized:
            case ApplicationState.Terminating:
                break;
            }

            if defined(CPlusPlus) extern "uInitRtti()";

            if (State != ApplicationState.Uninitialized)
                throw new InvalidOperationException("CoreApp: Invalid state");

            if defined(CPlusPlus) extern "uStartApp()";

            State = ApplicationState.Background;

            ApplicationStateTransitionHandler handler = Started;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Background)
                throw new InvalidOperationException("CoreApp: Invalid state");
        }

        internal static void EnterForeground()
        {
            switch (State)
            {
                case ApplicationState.Terminating:
                    debug_log "EnterForeground() called on terminating application";
                    return;

                case ApplicationState.Uninitialized:
                    debug_log "EnterForeground() called on uninitialized application";
                    return;

                case ApplicationState.Background:
                    break;

                case ApplicationState.Foreground:
                case ApplicationState.Interactive:
                    // Interactive already a sub-state of Foreground
                    return;
            }

            if (State != ApplicationState.Background)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Foreground;

            ApplicationStateTransitionHandler handler = EnteringForeground;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Foreground)
                throw new InvalidOperationException("CoreApp: Invalid state");
        }

        [extern(android) Require("Source.Include", "Uno/Graphics/GLHelper.h")]
        internal static void EnterInteractive()
        {
            if defined(Android) {
                extern "GLHelper::SwapBackToBackgroundSurface()";
            }
            switch (State)
            {
                case ApplicationState.Terminating:
                    debug_log "EnterInteractive() called on terminating application";
                    return;

                case ApplicationState.Uninitialized:
                    debug_log "EnterInteractive() called on uninitialized application";
                    return;

                case ApplicationState.Background:
                    EnterForeground();
                    break;

                case ApplicationState.Foreground:
                    break;

                case ApplicationState.Interactive:
                    return;
            }

            if (State != ApplicationState.Foreground)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Interactive;

            ApplicationStateTransitionHandler handler = EnteringInteractive;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Interactive)
                throw new InvalidOperationException("CoreApp: Invalid state");
        }

        internal static void ExitInteractive()
        {
            if defined(Android) {
                extern "GLHelper::SwapBackToBackgroundSurface()";
            }
            switch (State)
            {
                case ApplicationState.Terminating:
                    return;

                case ApplicationState.Uninitialized:
                    return;

                case ApplicationState.Background:
                case ApplicationState.Foreground:
                    return;

                case ApplicationState.Interactive:
                    break;
            }

            if (State != ApplicationState.Interactive)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Foreground;

            ApplicationStateTransitionHandler handler = ExitedInteractive;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Foreground)
                throw new InvalidOperationException("CoreApp: Invalid state");
        }

        internal static void EnterBackground()
        {
            switch (State)
            {
                case ApplicationState.Terminating:
                    return;

                case ApplicationState.Uninitialized:
                    return;

                case ApplicationState.Background:
                    return;

                case ApplicationState.Foreground:
                    break;

                case ApplicationState.Interactive:
                    ExitInteractive();
                    break;
            }

            if (State != ApplicationState.Foreground)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Background;

            ApplicationStateTransitionHandler handler = EnteringBackground;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Background)
                throw new InvalidOperationException("CoreApp: Invalid state");
        }

        internal static void Terminate()
        {
            switch (State)
            {
                case ApplicationState.Terminating:
                case ApplicationState.Uninitialized:
                    return;

                case ApplicationState.Background:
                    break;

                case ApplicationState.Foreground:
                case ApplicationState.Interactive:
                    EnterBackground();
                    break;
            }

            if (State != ApplicationState.Background)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Terminating;

            ApplicationStateTransitionHandler handler = Terminating;
            if (handler != null)
                handler(State);

            if (State != ApplicationState.Terminating)
                throw new InvalidOperationException("CoreApp: Invalid state");

            State = ApplicationState.Uninitialized;
        }

        //
        // Memory conservation
        //

        public static event EventHandler ReceivedLowMemoryWarning;

        internal static void OnReceivedLowMemoryWarning()
        {
            EventHandler handler = ReceivedLowMemoryWarning;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        static List<string> _delayedURIS = new List<string>();

        public static event EventHandler<string> _receivedURI;
        public static event EventHandler<string> ReceivedURI
        {
            add
            {
                _receivedURI += value;
                DispatchDelayedUri();
            }

            remove
            {
                _receivedURI -= value;
            }
        }

        internal static void OnReceivedURI(string uri)
        {
            EventHandler<string> handler = _receivedURI;
            if (handler != null)
            {
                DispatchDelayedUri();
                handler(null, uri);
            }
            else
            {
                _delayedURIS.Add(uri);
            }
        }

        static void DispatchDelayedUri()
        {
            EventHandler<string> handler = _receivedURI;
            if (handler != null)
            {
                if (_delayedURIS.Count > 0)
                {
                    foreach(var u in _delayedURIS)
                    {
                        handler(null, u);
                    }
                    _delayedURIS.Clear();
                }
            }
        }
    }
}
