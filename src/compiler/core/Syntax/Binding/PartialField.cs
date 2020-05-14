using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialField : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Field;

        public readonly Field Field;

        public PartialField(Source src, Field field, Expression instance)
            : base(src, instance)
        {
            Field = field;
        }

        public override string ToString()
        {
            return Field.ToString();
        }
    }
}