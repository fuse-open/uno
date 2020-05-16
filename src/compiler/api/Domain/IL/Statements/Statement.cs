using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public abstract class Statement : SourceObject
    {
        public object Tag;

        public abstract StatementType StatementType
        {
            get;
        }

        protected Statement(Source src)
            : base(src)
        {
        }

        public virtual void Visit(Pass p, ExpressionUsage u = ExpressionUsage.Statement)
        {
        }

        public abstract Statement CopyStatement(CopyState state);
    }
}
