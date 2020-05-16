using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public abstract class PartialExpression : SourceObject
    {
        public abstract PartialExpressionType ExpressionType { get; }

        public static readonly PartialExpression Invalid = new PartialValue(Expression.Invalid);

        public virtual bool IsInvalid => false;

        protected PartialExpression(Source src)
            : base(src)
        {
        }
    }
}
