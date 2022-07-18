using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.IObservable`1")]
    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}
