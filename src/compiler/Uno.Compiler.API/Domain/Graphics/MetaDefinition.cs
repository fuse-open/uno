using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class MetaDefinition
    {
        public readonly IReadOnlyList<string> Tags;
        public readonly IReadOnlyList<ReqStatement> Requirements;
        public Statement Value;

        public MetaDefinition(Statement value, IReadOnlyList<string> tags, IReadOnlyList<ReqStatement> reqs)
        {
            Tags = tags;
            Requirements = reqs;
            Value = value;
        }

        public MetaDefinition(Statement value, IReadOnlyList<string> tags, params ReqStatement[] reqs)
        {
            Tags = tags;
            Requirements = reqs;
            Value = value;
        }
    }
}