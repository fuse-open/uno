namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstIdentifier : AstExpression
    {
        public readonly string Symbol;

        public override AstExpressionType ExpressionType => AstExpressionType.Identifier;

        public AstIdentifier(Source src, string symbol)
            : base(src)
        {
            Symbol = symbol;
        }

        public string GetParameterizedSymbol(int? typeParamCount)
        {
            return Symbol + (
                    typeParamCount != null
                        ? "<" + new string(',', typeParamCount.Value - 1) + ">"
                        : null
                );
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}