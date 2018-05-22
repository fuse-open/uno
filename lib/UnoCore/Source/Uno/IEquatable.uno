using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.IEquatable`1")]
    public interface IEquatable<T>
    {
        bool Equals(T other);
    }
}
