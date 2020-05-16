namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstCast : AstExpression
    {
        public readonly AstExpression TargetType;
        public readonly AstExpression Argument;

        public override AstExpressionType ExpressionType => AstExpressionType.Cast;

        public AstCast(AstExpression targetType, AstExpression arg)
            : base(arg.Source)
        {
            TargetType = targetType;
            Argument = arg;
        }
    }
}