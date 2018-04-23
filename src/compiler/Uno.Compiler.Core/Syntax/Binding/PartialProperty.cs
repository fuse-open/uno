using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialProperty : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Property;

        public readonly Property Property;

        public PartialProperty(Source src, Property property, Expression instance)
            : base(src, instance)
        {
            Property = property;
        }

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}