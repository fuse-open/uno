namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstBuiltinType : AstExpression
    {
        public readonly BuiltinType BuiltinType;

        public override AstExpressionType ExpressionType => AstExpressionType.BuiltinType;

        public AstBuiltinType(Source src, BuiltinType builtin)
            : base(src)
        {
            BuiltinType = builtin;
        }
    }
}