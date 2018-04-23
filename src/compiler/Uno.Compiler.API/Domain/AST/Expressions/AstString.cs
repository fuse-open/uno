namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstString : AstExpression
    {
        public readonly string Value;

        public override AstExpressionType ExpressionType => AstExpressionType.String;

        public AstString(Source src, string value)
            : base(src)
        {
            Value = value;
        }
    }
}