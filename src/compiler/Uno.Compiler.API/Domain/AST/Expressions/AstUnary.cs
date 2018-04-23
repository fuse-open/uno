namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstUnary : AstExpression
    {
        public readonly AstUnaryType Type;
        public readonly AstExpression Operand;

        public override AstExpressionType ExpressionType => (AstExpressionType)Type;

        public AstUnary(Source src, AstUnaryType type, AstExpression operand)
            : base(src)
        {
            Type = type;
            Operand = operand;
        }
    }
}