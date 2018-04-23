using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstFinalizer : AstClassMember
    {
        public readonly Source Source;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstScope OptionalBody;

        public override AstMemberType MemberType => AstMemberType.Finalizer;

        public AstFinalizer(Source src, string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, IReadOnlyList<AstParameter> paramList, AstScope body)
            : base(comment, attrs, modifiers, cond)
        {
            Source = src;
            Parameters = paramList;
            OptionalBody = body;
        }
    }
}