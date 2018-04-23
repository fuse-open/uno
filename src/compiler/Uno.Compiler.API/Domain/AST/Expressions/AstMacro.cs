namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstMacro : AstExpression
    {
        public readonly string Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Macro;

        public AstMacro(Source src, string value)
            : base(src)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}