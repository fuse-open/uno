using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialType : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Type;

        public readonly DataType Type;

        public PartialType(Source src, DataType dt)
            : base(src)
        {
            Type = dt;
        }

        public override bool IsInvalid => Type is InvalidType;

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}