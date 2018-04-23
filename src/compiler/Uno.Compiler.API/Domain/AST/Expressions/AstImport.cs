using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstImport : AstExpression
    {
        public readonly AstExpression Importer;
        public readonly IReadOnlyList<AstArgument> Arguments;

        public override AstExpressionType ExpressionType => AstExpressionType.Import;

        public AstImport(Source src, AstExpression importer, IReadOnlyList<AstArgument> args = null)
            : base(src)
        {
            Importer = importer;
            Arguments = args;
        }
    }
}