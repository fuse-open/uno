using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstExtern : AstExpression
    {
        public readonly IReadOnlyList<AstAttribute> Attributes;
        public readonly AstExpression OptionalType;
        public readonly IReadOnlyList<AstArgument> OptionalArguments;
        public readonly SourceValue Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Extern;

        public AstExtern(Source src, IReadOnlyList<AstAttribute> attributes, AstExpression optionalType, IReadOnlyList<AstArgument> optionalArgs, SourceValue value)
            : base(src)
        {
            Attributes = attributes;
            OptionalType = optionalType;
            OptionalArguments = optionalArgs;
            Value = value;
        }
    }
}