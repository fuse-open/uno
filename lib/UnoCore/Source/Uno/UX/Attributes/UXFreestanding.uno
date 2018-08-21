using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.UX
{
    public sealed class UXFreestandingAttribute: Attribute {}

    public sealed class UXFunctionAttribute: Attribute 
    {
    	public UXFunctionAttribute(string name) {}
    }

    public sealed class UXArgAttribute: Attribute
    {
    	public UXArgAttribute(int index) {}
    }
}
