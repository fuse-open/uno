namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstLocal : AstExpression
    {
        public readonly AstIdentifier Name;

        public override AstExpressionType ExpressionType => AstExpressionType.Local;

        public AstLocal(AstIdentifier name)
            : base(name.Source)
        {
            Name = name;
        }
    }
}