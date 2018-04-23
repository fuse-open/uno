using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstNew : AstExpression
    {
        public readonly AstExpression OptionalType;
        public readonly AstExpression OptionalArraySize;
        public readonly IReadOnlyList<AstExpression> OptionalCollectionInitializer;
        public readonly IReadOnlyList<AstArgument> OptionalArguments;

        public override AstExpressionType ExpressionType => AstExpressionType.New;

        public AstNew(Source src, AstExpression type, IReadOnlyList<AstArgument> args)
            : base(src)
        {
            OptionalType = type;
            OptionalArguments = args;
        }
        
        public AstNew(Source src, AstExpression type, params AstArgument[] args)
            : base(src)
        {
            OptionalType = type;
            OptionalArguments = args;
        }

        public AstNew(Source src, AstExpression type, AstExpression arraySize, IReadOnlyList<AstArgument> args, IReadOnlyList<AstExpression> collectionInitializer)
            : base(src)
        {
            OptionalType = type;
            OptionalArraySize = arraySize;
            OptionalArguments = args;
            OptionalCollectionInitializer = collectionInitializer;
        }
    }
}