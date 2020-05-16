using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstGenericSignature
    {
        public readonly IReadOnlyList<AstIdentifier> Parameters;
        public readonly IReadOnlyList<AstConstraint> Constraints;

        public AstGenericSignature(IReadOnlyList<AstIdentifier> parameters, IReadOnlyList<AstConstraint> constraints)
        {
            Parameters = parameters;
            Constraints = constraints;
        }
    }
}