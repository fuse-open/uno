namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstDefined : AstExpression
    {
        public readonly string Condition;

        public override AstExpressionType ExpressionType => AstExpressionType.Defined;

        public AstDefined(Source src, string cond)
            : base(src)
        {
            Condition = cond;
        }
    }
}