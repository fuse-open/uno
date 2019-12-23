using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    [extern(DOTNET) DotNetType]
    public class OSFrame
    {
        public event EventHandler Resized;

        internal void OnResized(EventArgs args)
        {
            if (Resized != null)
                Resized(this, args);
        }
    }
}
