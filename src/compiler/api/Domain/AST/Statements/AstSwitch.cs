using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstSwitch : AstStatement
    {
        public readonly AstExpression Condition;
        public readonly IReadOnlyList<AstSwitchCase> Cases;

        public override AstStatementType StatementType => AstStatementType.Switch;

        public AstSwitch(Source src, AstExpression cond, IReadOnlyList<AstSwitchCase> cases)
            : base(src)
        {
            Condition = cond;
            Cases = cases;
        }
    }
}