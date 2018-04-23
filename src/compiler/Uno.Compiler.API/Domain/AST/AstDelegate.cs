using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstDelegate : AstBlockBase
    {
        public readonly AstExpression ReturnType;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstGenericSignature OptionalGenericSignature;

        public override AstMemberType MemberType => AstMemberType.Delegate;

        public AstDelegate(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, 
            AstExpression retType, AstIdentifier name, IReadOnlyList<AstParameter> parameters, AstGenericSignature optionalGenericSig)
            : base(comment, attrs, modifiers, cond, name, null)
        {
            ReturnType = retType;
            Parameters = parameters;
            OptionalGenericSignature = optionalGenericSig;
        }
    }
}
