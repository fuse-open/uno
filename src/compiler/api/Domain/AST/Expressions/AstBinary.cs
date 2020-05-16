namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstBinary : AstExpression
    {
        public readonly AstBinaryType Type;
        public readonly AstExpression Left;
        public readonly AstExpression Right;

        public override AstExpressionType ExpressionType => (AstExpressionType) Type;
        public bool IsAssign => Type >= AstBinaryType.Assign && Type <= AstBinaryType.LogOrAssign;
        public AstBinaryType RemoveAssign => Type - AstBinaryType.AddAssign + AstBinaryType.Add;

        public AstBinary(AstBinaryType type, AstExpression left, Source src, AstExpression right)
            : base(src)
        {
            Type = type;
            Left = left;
            Right = right;
        }
    }
}