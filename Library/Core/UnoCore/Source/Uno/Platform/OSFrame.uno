using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.Implementation;

namespace Uno.Platform
{
    [extern(DOTNET) DotNetType]
    public class OSFrame
    {
        internal PlatformWindowHandle _handle;

        internal OSFrame()
        {
            if defined(MOBILE)
                _handle = extern<PlatformWindowHandle> "(@{PlatformWindowHandle})NULL";
            else
                _handle = PlatformWindowImpl.GetInstance();
        }

        public event EventHandler Resized;

        internal void OnResized(EventArgs args)
        {
            if (Resized != null)
                Resized(this, args);
        }
    }
}
