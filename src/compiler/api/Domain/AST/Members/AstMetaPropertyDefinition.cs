using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstMetaPropertyDefinition
    {
        public readonly IReadOnlyList<string> Tags;
        public readonly IReadOnlyList<AstReqStatement> Requirements;
        public readonly AstStatement Value;

        public AstMetaPropertyDefinition(AstStatement value, IReadOnlyList<string> tags = null, IReadOnlyList<AstReqStatement> reqs = null)
        {
            Value = value;
            Tags = tags ?? new string[0];
            Requirements = reqs ?? new AstReqStatement[0];
        }
    }
}