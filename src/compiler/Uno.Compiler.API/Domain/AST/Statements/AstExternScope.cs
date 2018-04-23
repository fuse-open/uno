using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstExternScope : AstStatement
    {
        public readonly IReadOnlyList<AstAttribute> Attributes;
        public readonly IReadOnlyList<AstArgument> OptionalArguments;
        public readonly SourceValue Body;

        public override AstStatementType StatementType => AstStatementType.ExternScope;

        public AstExternScope(Source src, IReadOnlyList<AstAttribute> attributes, IReadOnlyList<AstArgument> optionalArgs, SourceValue body)
            : base(src)
        {
            Attributes = attributes;
            OptionalArguments = optionalArgs;
            Body = body;
        }
    }
}