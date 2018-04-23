namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialThis : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.This;

        public PartialThis(Source src)
            : base(src)
        {
        }

        public override string ToString()
        {
            return "this";
        }
    }
}