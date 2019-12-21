// This file was generated based on lib/UnoCore/Source/Uno/Platform/OSFrame.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class OSFrame
    {
        public OSFrame()
        {
        }

        public void OnResized(global::System.EventArgs args)
        {
            if (this.Resized != null)
                this.Resized(this, args);
        }

        public event global::System.EventHandler Resized;
    }
}
