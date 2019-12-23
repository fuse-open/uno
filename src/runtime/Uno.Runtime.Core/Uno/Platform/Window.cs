// This file was generated based on lib/UnoCore/Source/Uno/Platform/Window.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public sealed class Window : OSFrame
    {
        public WindowBackend Backend;

        public Window()
        {
            this.Backend = WindowBackend.Instance;
            Displays.MainDisplay.Tick += new global::System.EventHandler<TimerEventArgs>(this.OnTick);
            CoreApp.EnteringInteractive += new ApplicationStateTransitionHandler(this.OnGotFocus);
            CoreApp.ExitedInteractive += new ApplicationStateTransitionHandler(this.OnLostFocus);
        }

        public void Close()
        {
            this.Backend.Close();
        }

        public void OnTick(object sender, TimerEventArgs args)
        {
            global::System.EventHandler handler = this.Updating;

            if (handler != null)
                handler(sender, global::System.EventArgs.Empty);
        }

        public void OnGotFocus(ApplicationState newState)
        {
            global::System.EventHandler handler = this.GotFocus;

            if (handler != null)
                handler(null, global::System.EventArgs.Empty);
        }

        public void OnLostFocus(ApplicationState newState)
        {
            global::System.EventHandler handler = this.LostFocus;

            if (handler != null)
                handler(null, global::System.EventArgs.Empty);
        }

        public string Title
        {
            get { return this.Backend.GetTitle(); }
            set { this.Backend.SetTitle(value); }
        }

        public PointerCursor PointerCursor
        {
            get { return this.Backend.GetPointerCursor(); }
            set { this.Backend.SetPointerCursor(value); }
        }

        public global::Uno.Int2 ClientSize
        {
            get { return this.Backend.GetClientSize(); }
            set { this.Backend.SetClientSize(value); }
        }

        public bool Fullscreen
        {
            get { return this.Backend.GetFullscreen(); }
            set { this.Backend.SetFullscreen(value); }
        }

        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.EventSources.TextSource.IsTextInputActive")]
        public bool IsTextInputActive
        {
            get { return this.Backend.IsTextInputActive(); }
        }

        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.EventSources.TextSource.TextInput")]
        public event global::System.EventHandler<TextInputEventArgs> TextInput;
        [global::System.ObsoleteAttribute("Deprecated: Invalid on desktop")]
        public event global::System.EventHandler KeyboardResized;
        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.HardwareKeys.KeyDown")]
        public event global::System.EventHandler<KeyEventArgs> KeyPressed
        {
            add { EventSources.HardwareKeys.KeyDown += value; }
            remove { EventSources.HardwareKeys.KeyDown -= value; }
        }

        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.HardwareKeys.KeyUp")]
        public event global::System.EventHandler<KeyEventArgs> KeyReleased
        {
            add { EventSources.HardwareKeys.KeyUp += value; }
            remove { EventSources.HardwareKeys.KeyUp -= value; }
        }

        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.Displays.MainDisplay.Tick")]
        public event global::System.EventHandler Updating;
        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.CoreApp.EnteringInteractive")]
        public event global::System.EventHandler GotFocus;
        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.CoreApp.ExitedInteractive")]
        public event global::System.EventHandler LostFocus;
    }
}
