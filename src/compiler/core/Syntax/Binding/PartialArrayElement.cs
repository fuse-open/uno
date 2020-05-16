using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialArrayElement : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.ArrayElement;

        public readonly DataType ElementType;
        public readonly Expression Index;

        public PartialArrayElement(Source src, Expression array, DataType elmType, Expression index)
            : base(src, array)
        {
            ElementType = elmType;
            Index = index;
        }

        public override string ToString()
        {
            return Object + "[" + Index + "]";
        }
    }
}