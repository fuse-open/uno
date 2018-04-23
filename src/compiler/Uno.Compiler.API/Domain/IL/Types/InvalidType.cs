namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class InvalidType : DataType
    {
        internal InvalidType()
            : base(Source.Unknown, null, null, 0, "<invalid>")
        {
        }

        public override TypeType TypeType => TypeType.Invalid;
    }
}