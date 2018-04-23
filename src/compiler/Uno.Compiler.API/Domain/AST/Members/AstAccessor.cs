using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstAccessor : SourceObject
    {
        public readonly Modifiers Modifiers;
        public readonly string OptionalCondition;
        public readonly AstScope OptionalBody;

        public AstAccessor(Source src, Modifiers modifiers, string cond, AstScope body)
            : base(src)
        {
            Modifiers = modifiers;
            OptionalCondition = cond;
            OptionalBody = body;
        }
    }
}