using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstOperator : AstClassMember
    {
        public readonly Source Source;
        public readonly AstExpression ReturnType;
        public readonly OperatorType Operator;
        public readonly IReadOnlyList<AstParameter> Parameters;
        public readonly AstScope OptionalBody;

        public override AstMemberType MemberType => AstMemberType.Operator;

        public AstOperator(Source src, string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression returnType, OperatorType op, IReadOnlyList<AstParameter> argList, AstScope body)
            : base(comment, attrs, modifiers, cond)
        {
            Source = src;
            ReturnType = returnType;
            Operator = op;
            Parameters = argList;
            OptionalBody = body;
        }
    }
}