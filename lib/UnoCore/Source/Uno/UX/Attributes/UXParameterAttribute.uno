using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.UX
{
    [extern(DOTNET) DotNetType]
    public sealed class UXParameterAttribute: Attribute
    {
        public readonly string Name;

        public UXParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
