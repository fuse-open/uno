// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public abstract class CoreApp
    {
        public static global::System.Collections.Generic.List<string> _delayedURIS;

        static CoreApp()
        {
            CoreApp._delayedURIS = new global::System.Collections.Generic.List<string>();
        }

        public CoreApp()
        {
            if (CoreApp.Current == null)
                CoreApp.Current = this;
        }

        public virtual void Load()
        {
        }

        public static void Start()
        {
            ApplicationState assert1;
            ApplicationState assert2;

            switch (CoreApp.State)
            {
                case ApplicationState.Background:
                {
                    CoreApp.Terminate();
                    break;
                }
                case ApplicationState.Uninitialized:
                case ApplicationState.Terminating:
                    break;
            }

            assert1 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert1 == ApplicationState.Uninitialized, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Uninitialized", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 74, new object[] { (object)assert1, (object)ApplicationState.Uninitialized});
            CoreApp.State = ApplicationState.Background;
            ApplicationStateTransitionHandler handler = CoreApp.Started;

            if (handler != null)
                handler(CoreApp.State);

            assert2 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert2 == ApplicationState.Background, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Background", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 84, new object[] { (object)assert2, (object)ApplicationState.Background});
        }

        public static void EnterForeground()
        {
            ApplicationState assert3;
            ApplicationState assert4;

            switch (CoreApp.State)
            {
                case ApplicationState.Terminating:
                {
                    global::Uno.Diagnostics.Debug.Log("EnterForeground() called on terminating application", global::Uno.Diagnostics.DebugMessageType.Debug, "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 92);
                    return;
                }
                case ApplicationState.Uninitialized:
                {
                    global::Uno.Diagnostics.Debug.Log("EnterForeground() called on uninitialized application", global::Uno.Diagnostics.DebugMessageType.Debug, "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 96);
                    return;
                }
                case ApplicationState.Background:
                    break;
                case ApplicationState.Foreground:
                case ApplicationState.Interactive:
                    return;
            }

            assert3 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert3 == ApplicationState.Background, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Background", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 108, new object[] { (object)assert3, (object)ApplicationState.Background});
            CoreApp.State = ApplicationState.Foreground;
            ApplicationStateTransitionHandler handler = CoreApp.EnteringForeground;

            if (handler != null)
                handler(CoreApp.State);

            assert4 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert4 == ApplicationState.Foreground, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Foreground", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 116, new object[] { (object)assert4, (object)ApplicationState.Foreground});
        }

        public static void EnterInteractive()
        {
            ApplicationState assert5;
            ApplicationState assert6;

            switch (CoreApp.State)
            {
                case ApplicationState.Terminating:
                {
                    global::Uno.Diagnostics.Debug.Log("EnterInteractive() called on terminating application", global::Uno.Diagnostics.DebugMessageType.Debug, "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 128);
                    return;
                }
                case ApplicationState.Uninitialized:
                {
                    global::Uno.Diagnostics.Debug.Log("EnterInteractive() called on uninitialized application", global::Uno.Diagnostics.DebugMessageType.Debug, "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 132);
                    return;
                }
                case ApplicationState.Background:
                {
                    CoreApp.EnterForeground();
                    break;
                }
                case ApplicationState.Foreground:
                    break;
                case ApplicationState.Interactive:
                    return;
            }

            assert5 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert5 == ApplicationState.Foreground, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Foreground", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 146, new object[] { (object)assert5, (object)ApplicationState.Foreground});
            CoreApp.State = ApplicationState.Interactive;
            ApplicationStateTransitionHandler handler = CoreApp.EnteringInteractive;

            if (handler != null)
                handler(CoreApp.State);

            assert6 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert6 == ApplicationState.Interactive, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Interactive", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 154, new object[] { (object)assert6, (object)ApplicationState.Interactive});
        }

        public static void ExitInteractive()
        {
            ApplicationState assert7;
            ApplicationState assert8;

            switch (CoreApp.State)
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

            assert7 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert7 == ApplicationState.Interactive, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Interactive", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 178, new object[] { (object)assert7, (object)ApplicationState.Interactive});
            CoreApp.State = ApplicationState.Foreground;
            ApplicationStateTransitionHandler handler = CoreApp.ExitedInteractive;

            if (handler != null)
                handler(CoreApp.State);

            assert8 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert8 == ApplicationState.Foreground, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Foreground", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 186, new object[] { (object)assert8, (object)ApplicationState.Foreground});
        }

        public static void EnterBackground()
        {
            ApplicationState assert9;
            ApplicationState assert10;

            switch (CoreApp.State)
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
                {
                    CoreApp.ExitInteractive();
                    break;
                }
            }

            assert9 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert9 == ApplicationState.Foreground, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Foreground", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 210, new object[] { (object)assert9, (object)ApplicationState.Foreground});
            CoreApp.State = ApplicationState.Background;
            ApplicationStateTransitionHandler handler = CoreApp.EnteringBackground;

            if (handler != null)
                handler(CoreApp.State);

            assert10 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert10 == ApplicationState.Background, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Background", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 218, new object[] { (object)assert10, (object)ApplicationState.Background});
        }

        public static void Terminate()
        {
            ApplicationState assert11;
            ApplicationState assert12;

            switch (CoreApp.State)
            {
                case ApplicationState.Terminating:
                case ApplicationState.Uninitialized:
                    return;
                case ApplicationState.Background:
                    break;
                case ApplicationState.Foreground:
                case ApplicationState.Interactive:
                {
                    CoreApp.EnterBackground();
                    break;
                }
            }

            assert11 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert11 == ApplicationState.Background, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Background", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 238, new object[] { (object)assert11, (object)ApplicationState.Background});
            CoreApp.State = ApplicationState.Terminating;
            ApplicationStateTransitionHandler handler = CoreApp.Terminating;

            if (handler != null)
                handler(CoreApp.State);

            assert12 = CoreApp.State;
            global::Uno.Diagnostics.Debug.Assert(assert12 == ApplicationState.Terminating, "Uno.Platform.CoreApp.State == Uno.Platform.ApplicationState.Terminating", "Library/Core/UnoCore/Source/Uno/Platform/CoreApp.uno", 246, new object[] { (object)assert12, (object)ApplicationState.Terminating});
            CoreApp.State = ApplicationState.Uninitialized;
        }

        public static void OnReceivedLowMemoryWarning()
        {
            global::System.EventHandler handler = CoreApp.ReceivedLowMemoryWarning;

            if (handler != null)
                handler(null, global::System.EventArgs.Empty);
        }

        public static void OnReceivedURI(string uri)
        {
            global::System.EventHandler<string> handler = CoreApp._receivedURI;

            if (handler != null)
            {
                CoreApp.DispatchDelayedUri();
                handler(null, uri);
            }
            else
                CoreApp._delayedURIS.Add(uri);
        }

        public static void DispatchDelayedUri()
        {
            global::System.EventHandler<string> handler = CoreApp._receivedURI;

            if (handler != null)
            {
                if (CoreApp._delayedURIS.Count > 0)
                {
                    global::System.Collections.Generic.List<string>.Enumerator enum13 = CoreApp._delayedURIS.GetEnumerator();

                    try
                    {
                        while (enum13.MoveNext())
                        {
                            string u = enum13.Current;
                            handler(null, u);
                        }
                    }
                    finally
                    {
                        enum13.Dispose();
                    }

                    CoreApp._delayedURIS.Clear();
                }
            }
        }

        public static CoreApp Current
        {
            get;
            set;
        }

        public static ApplicationState State
        {
            get;
            set;
        }

        public static event ApplicationStateTransitionHandler Started;
        public static event ApplicationStateTransitionHandler EnteringForeground;
        public static event ApplicationStateTransitionHandler EnteringInteractive;
        public static event ApplicationStateTransitionHandler ExitedInteractive;
        public static event ApplicationStateTransitionHandler EnteringBackground;
        public static event ApplicationStateTransitionHandler Terminating;
        public static event global::System.EventHandler ReceivedLowMemoryWarning;
        public static event global::System.EventHandler<string> _receivedURI;
        public static event global::System.EventHandler<string> ReceivedURI
        {
            add
            {
                CoreApp._receivedURI += value;
                CoreApp.DispatchDelayedUri();
            }
            remove { CoreApp._receivedURI -= value; }
        }
    }
}
