using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstConstraint
    {
        public readonly AstIdentifier Parameter;
        public readonly AstConstraintType Type;
        public readonly IReadOnlyList<AstExpression> BaseTypes;
        public readonly Source OptionalConstructor;

        public AstConstraint(AstIdentifier parameter, AstConstraintType type, IReadOnlyList<AstExpression> baseTypes, Source optionalCtor = null)
        {
            Parameter = parameter;
            Type = type;
            BaseTypes = baseTypes;
            OptionalConstructor = optionalCtor;
        }
    }
}