using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public abstract class PartialMember : PartialExpression
    {
        public readonly Expression Object;

        protected PartialMember(Source src, Expression obj)
            : base(src)
        {
            Object = obj;
        }
    }
}