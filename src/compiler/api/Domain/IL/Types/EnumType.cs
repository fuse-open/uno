namespace Uno.Compiler.API.Domain.IL.Types
{
    public class EnumType : DataType
    {
        public EnumType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        public override TypeType TypeType => TypeType.Enum;
    }
}
