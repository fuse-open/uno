using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.IComparable`1")]
    public interface IComparable<T>
    {
        int CompareTo(T other);
    }
}
