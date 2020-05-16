using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public abstract class AstNamedMember : AstClassMember
    {
        public readonly AstExpression ReturnType;
        public readonly AstIdentifier Name;

        protected AstNamedMember(string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond, AstExpression returnType, AstIdentifier name)
            : base(comment, attributes, modifiers, cond)
        {
            ReturnType = returnType;
            Name = name;
        }
    }
}