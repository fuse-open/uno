using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstConverter : AstClassMember
    {
        public readonly AstExpression TargetType;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstScope OptionalBody;

        public override AstMemberType MemberType => AstMemberType.Converter;

        public AstConverter(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression targetTypeExpression, IReadOnlyList<AstParameter> argList, AstScope body)
            : base(comment, attrs, modifiers, cond)
        {
            TargetType = targetTypeExpression;
            Parameters = argList;
            OptionalBody = body;
        }
    }
}