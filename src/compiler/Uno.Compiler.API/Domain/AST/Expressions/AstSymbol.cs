namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstSymbol : AstExpression
    {
        public readonly AstSymbolType SymbolType;

        public override AstExpressionType ExpressionType => (AstExpressionType)SymbolType;

        public AstSymbol(Source src, AstSymbolType type)
            : base(src)
        {
            SymbolType = type;
        }
    }
}