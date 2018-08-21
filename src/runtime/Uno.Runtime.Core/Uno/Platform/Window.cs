// This file was generated based on lib/UnoCore/Source/Uno/Platform/Window.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public sealed class Window : OSFrame
    {
        public Window()
        {
            Displays.MainDisplay.Tick += new global::System.EventHandler<TimerEventArgs>(this.OnTick);
            CoreApp.EnteringInteractive += new ApplicationStateTransitionHandler(this.OnGotFocus);
            CoreApp.ExitedInteractive += new ApplicationStateTransitionHandler(this.OnLostFocus);
            this._handle = global::Uno.Runtime.Implementation.PlatformWindowImpl.GetInstance();
        }

        public void Close()
        {
            global::Uno.Runtime.Implementation.PlatformWindowImpl.Close(this._handle);
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
            get { return global::Uno.Runtime.Implementation.PlatformWindowImpl.GetTitle(this._handle); }
            set { global::Uno.Runtime.Implementation.PlatformWindowImpl.SetTitle(this._handle, value); }
        }

        public PointerCursor PointerCursor
        {
            get { return global::Uno.Runtime.Implementation.PlatformWindowImpl.GetPointerCursor(this._handle); }
            set { global::Uno.Runtime.Implementation.PlatformWindowImpl.SetPointerCursor(this._handle, value); }
        }

        public global::Uno.Int2 ClientSize
        {
            get { return global::Uno.Runtime.Implementation.PlatformWindowImpl.GetClientSize(this._handle); }
            set { global::Uno.Runtime.Implementation.PlatformWindowImpl.SetClientSize(this._handle, value); }
        }

        public bool Fullscreen
        {
            get { return global::Uno.Runtime.Implementation.PlatformWindowImpl.GetFullscreen(this._handle); }
            set { global::Uno.Runtime.Implementation.PlatformWindowImpl.SetFullscreen(this._handle, value); }
        }

        [global::System.ObsoleteAttribute("Deprecated: Please use Uno.Platform.EventSources.TextSource.IsTextInputActive")]
        public bool IsTextInputActive
        {
            get { return global::Uno.Runtime.Implementation.PlatformWindowImpl.IsTextInputActive(this._handle); }
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
