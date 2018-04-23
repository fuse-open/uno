namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class VoidType : DataType
    {
        public override TypeType TypeType => TypeType.Void;

        internal VoidType()
            : base(Source.Unknown, null, null, Modifiers.Public, "void")
        {
        }
    }
}