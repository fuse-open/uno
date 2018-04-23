using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialIndexer : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Indexer;

        public readonly Property Indexer;
        public readonly Expression[] Arguments;

        public PartialIndexer(Source src, Expression instance, Property indexer, params Expression[] args)
            : base(src, instance)
        {
            Indexer = indexer;
            Arguments = args;
        }

        public override string ToString()
        {
            return Indexer.ToString();
        }
    }
}