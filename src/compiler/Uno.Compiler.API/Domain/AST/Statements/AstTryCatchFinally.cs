using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstTryCatchFinally : AstStatement
    {
        public readonly AstScope TryScope;
        public readonly AstScope OptionalFinallyScope;
        public readonly IReadOnlyList<AstCatch> CatchBlocks;

        public override AstStatementType StatementType => AstStatementType.TryCatchFinally;

        public AstTryCatchFinally(Source src, AstScope tryScope, AstScope optionalFinallyScope, IReadOnlyList<AstCatch> catchBlocks) : base(src)
        {
            TryScope = tryScope;
            OptionalFinallyScope = optionalFinallyScope;
            CatchBlocks = catchBlocks;
        }
    }
}