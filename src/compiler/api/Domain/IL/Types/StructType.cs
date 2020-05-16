namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class StructType : GenericType
    {
        public StructType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        public override TypeType TypeType => TypeType.Struct;

        protected override GenericType CreateParameterizable()
        {
            return new StructType(Source, Parent, DocComment, Modifiers, UnoName);
        }
    }
}
