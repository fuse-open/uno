using Uno;

namespace Uno.UX
{
    public sealed class UXTestBootstrapperForAttribute: Attribute
    {
        public readonly string ClassName;
        public UXTestBootstrapperForAttribute(string className)
        {
            ClassName = className;
        }
    }
}
