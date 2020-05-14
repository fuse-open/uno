namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class MethodGroupType : DataType
    {
        internal MethodGroupType()
            : base(Source.Unknown, null, null, 0, "<method_group>")
        {
        }

        public override TypeType TypeType => TypeType.MethodGroup;
    }
}