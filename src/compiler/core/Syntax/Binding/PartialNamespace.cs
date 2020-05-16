using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialNamespace : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Namespace;

        public readonly Namespace Namespace;

        public PartialNamespace(Source src, Namespace ns)
            : base(src)
        {
            Namespace = ns;
        }

        public override string ToString()
        {
            return Namespace.ToString();
        }
    }
}