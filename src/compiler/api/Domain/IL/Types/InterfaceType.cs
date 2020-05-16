namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class InterfaceType : GenericType
    {
        public InterfaceType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        public override TypeType TypeType => TypeType.Interface;

        protected override GenericType CreateParameterizable()
        {
            return new InterfaceType(Source, Parent, DocComment, Modifiers, UnoName);
        }
    }
}
