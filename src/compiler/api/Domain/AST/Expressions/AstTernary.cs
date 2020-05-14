namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstTernary : AstExpression
    {
        public readonly AstExpression Condition, True, False;

        public override AstExpressionType ExpressionType => AstExpressionType.Ternary;

        public AstTernary(AstExpression cond, Source src, AstExpression onTrue, AstExpression onFalse)
            : base(src)
        {
            Condition = cond;
            True = onTrue;
            False = onFalse;
        }
    }
}