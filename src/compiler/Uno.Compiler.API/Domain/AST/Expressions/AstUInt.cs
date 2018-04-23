namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstUInt : AstExpression
    {
        public readonly uint Value;

        public override AstExpressionType ExpressionType => AstExpressionType.UInt;

        public AstUInt(Source src, uint value)
            : base(src)
        {
            Value = value;
        }
    }
}