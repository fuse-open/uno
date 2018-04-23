using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Cast : Function
    {
        public readonly CastModifier Type;

        public override MemberType MemberType => MemberType.Cast;

        public Cast(Source src, DataType owner, CastModifier type, string comment, Modifiers modifiers, DataType targetType, Parameter[] parameters, Scope optionalBody = null)
            : base(src, owner, "op_" + type, targetType, parameters, optionalBody, comment, modifiers)
        {
            Type = type;
        }
    }
}