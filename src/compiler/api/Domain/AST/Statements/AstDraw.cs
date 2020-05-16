using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstDraw : AstStatement
    {
        public readonly AstNode Block;

        public override AstStatementType StatementType => AstStatementType.Draw;

        public AstDraw(Source src, IReadOnlyList<AstBlockMember> members)
            : base(src)
        {
            Block = new AstNode(0, new AstIdentifier(src, ".draw"), members);
        }
    }
}