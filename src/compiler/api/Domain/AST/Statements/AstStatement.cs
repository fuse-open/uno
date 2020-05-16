using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public abstract class AstStatement : SourceObject
    {
        public abstract AstStatementType StatementType { get; }
        public bool IsInvalid => this is AstInvalid;

        protected AstStatement(Source src)
            : base(src)
        {
        }
    }
}
