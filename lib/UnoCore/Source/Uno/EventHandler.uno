using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.EventHandler")]
    public delegate void EventHandler(object sender, EventArgs args);

    [extern(DOTNET) DotNetType("System.EventHandler`1")]
    public delegate void EventHandler<TEventArgs>(object sender, TEventArgs args);
}
