using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstConstructor : AstClassMember
    {
        public readonly Source Source;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstConstructorCallType CallType;
        public readonly IReadOnlyList<AstArgument> CallArguments;
        public readonly AstScope OptionalBody;

        public override AstMemberType MemberType => AstMemberType.Constructor;

        public AstConstructor(Source src, string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond, IReadOnlyList<AstParameter> paramList, 
                AstConstructorCallType callType = 0, IReadOnlyList<AstArgument> callArgs = null, AstScope body = null)
            : base(comment, attributes, modifiers, cond)
        {
            Source = src;
            CallType = callType;
            CallArguments = callArgs;
            Parameters = paramList;
            OptionalBody = body;
        }
    }
}