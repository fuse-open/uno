using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public class AstScope : AstStatement
    {
        // Used by the parser to tag the Scope as unclosed (invalid scope)
        // Unclosed scopes can still be used by e.g. intellisense to detect local variables
        // in the valid part of the scope
        public bool IsClosed = true;

        public readonly IReadOnlyList<AstStatement> Statements;

        public override AstStatementType StatementType => AstStatementType.Scope;

        public AstScope(Source src, IReadOnlyList<AstStatement> statements, bool isClosed = true)
            : base(src)
        {
            Statements = statements;
            IsClosed = isClosed;
        }

        public AstScope(Source src, params AstStatement[] statements)
            : base(src)
        {
            Statements = statements;
        }
    }
}