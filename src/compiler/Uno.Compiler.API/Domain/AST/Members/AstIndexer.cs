using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstIndexer : AstClassMember
    {
        public readonly Source Source;
        public readonly AstExpression ReturnType;
        public readonly AstExpression OptionalInterfaceType;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstAccessor Get;
        public readonly AstAccessor Set;

        public override AstMemberType MemberType => AstMemberType.Indexer;

        public AstIndexer(Source src, string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression returnType, AstExpression optionalInterfaceType, IReadOnlyList<AstParameter> parameterList, AstAccessor get, AstAccessor set)
            : base(comment, attrs, modifiers, cond)
        {
            Source = src;
            ReturnType = returnType;
            OptionalInterfaceType = optionalInterfaceType;
            Parameters = parameterList;
            Get = get;
            Set = set;
        }
    }
}