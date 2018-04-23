using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialVariable : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Variable;

        public readonly Variable Variable;

        public PartialVariable(Source src, Variable variable)
            : base(src)
        {
            Variable = variable;
        }

        public override string ToString()
        {
            return Variable.Name;
        }
    }
}