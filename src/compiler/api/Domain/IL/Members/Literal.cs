namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Literal : Member
    {
        public object Value;

        public override MemberType MemberType => MemberType.Literal;

        public Literal(Source src, DataType owner, string name, string comment, Modifiers modifiers, DataType type, object value)
            : base(src, comment, modifiers, name, owner, type)
        {
            Value = value;
        }
    }
}