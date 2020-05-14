using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstFixedArrayDeclaration : AstStatement
    {
        public readonly AstFixedArray Type;
        public readonly AstIdentifier Name;
        public readonly AstExpression OptionalValue;

        public override AstStatementType StatementType => AstStatementType.FixedArrayDeclaration;

        public AstFixedArrayDeclaration(AstFixedArray type, AstIdentifier name, AstExpression value)
            : base(name.Source)
        {
            Type = type;
            Name = name;
            OptionalValue = value;
        }
    }
}