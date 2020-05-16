using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialValue : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Value;

        public readonly Expression Value;

        public PartialValue(Source src, Expression value)
            : base(src)
        {
            Value = value;
        }

        public PartialValue(Expression value)
            : base(value.Source)
        {
            Value = value;
        }

        public override bool IsInvalid => Value is InvalidExpression || Value.ReturnType is InvalidType;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}