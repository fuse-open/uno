using Uno;

namespace Uno.UX
{
    public sealed class UXGlobalResourceAttribute: Attribute
    {
        public readonly string Alias;

        public UXGlobalResourceAttribute(string alias)
        {
            Alias = alias;
        }

        public UXGlobalResourceAttribute() {}
    }

    public sealed class UXGlobalModuleAttribute: Attribute
    {
        public UXGlobalModuleAttribute() {}
    }
}
