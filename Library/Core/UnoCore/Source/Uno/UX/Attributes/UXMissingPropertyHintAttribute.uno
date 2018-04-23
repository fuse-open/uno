using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.UX
{
    public sealed class UXMissingPropertyHintAttribute: Attribute 
    {
    	public UXMissingPropertyHintAttribute(string propertyName, string hint) {}
    }
}
