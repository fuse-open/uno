namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class NullType : DataType
    {
        internal NullType()
            : base(Source.Unknown, null, null, 0, "<null>")
        {
        }

        public override TypeType TypeType => TypeType.Null;
    }
}