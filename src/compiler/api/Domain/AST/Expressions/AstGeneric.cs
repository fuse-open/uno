namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstGeneric : AstExpression
    {
        public readonly AstExpression Base;
        public readonly int ArgumentCount;

        public override AstExpressionType ExpressionType => AstExpressionType.Generic;

        public AstGeneric(AstExpression name, int argCount)
            : base(name.Source)
        {
            Base = name;
            ArgumentCount = argCount;
        }
    }
}