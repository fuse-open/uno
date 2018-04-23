// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/OSFrame.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class OSFrame
    {
        public global::Uno.Runtime.Implementation.PlatformWindowHandle _handle;

        public OSFrame()
        {
            this._handle = global::Uno.Runtime.Implementation.PlatformWindowImpl.GetInstance();
        }

        public void OnResized(global::System.EventArgs args)
        {
            if (this.Resized != null)
                this.Resized(this, args);
        }

        public event global::System.EventHandler Resized;
    }
}
