using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstMethod : AstNamedMember
    {
        public readonly AstExpression OptionalInterfaceType;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstGenericSignature OptionalGenericSignature;
        public readonly AstScope OptionalBody;

        public override AstMemberType MemberType => AstMemberType.Method;

        public AstMethod(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression dataType, AstExpression optionalInterfaceType, AstIdentifier name, IReadOnlyList<AstParameter> argList, AstGenericSignature optionalGenericSig, AstScope body)
            : base(comment, attrs, modifiers, cond, dataType, name)
        {
            OptionalInterfaceType = optionalInterfaceType;
            Parameters = argList;
            OptionalGenericSignature = optionalGenericSig;
            OptionalBody = body;
        }
    }
}