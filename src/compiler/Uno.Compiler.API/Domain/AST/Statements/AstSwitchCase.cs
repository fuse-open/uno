using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public struct AstSwitchCase
    {
        public readonly IReadOnlyList<AstExpression> Values;
        public readonly AstScope Scope;

        public AstSwitchCase(IReadOnlyList<AstExpression> values, AstScope scope)
        {
            Values = values;
            Scope = scope;
        }
    }
}