using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Constructor : Function
    {
        public Constructor(
            Source src,
            DataType owner,
            string comment,
            Modifiers modifiers,
            Parameter[] parameters,
            Scope optionalBody = null)
            : base(src, owner, modifiers.HasFlag(Modifiers.Static) ? ".cctor" : ".ctor", DataType.Void, parameters, optionalBody, comment, modifiers)
        {
        }

        public override MemberType MemberType => MemberType.Constructor;
    }
}