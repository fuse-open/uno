using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Finalizer : Function
    {
        public Finalizer(
            Source src,
            DataType owner,
            string comment, 
            Modifiers modifiers,
            Parameter[] parameters,
            Scope optionalBody = null)
            : base(src, owner, "Finalize", DataType.Void, parameters, optionalBody, comment, modifiers)
        {
        }

        public override MemberType MemberType => MemberType.Finalizer;
    }
}