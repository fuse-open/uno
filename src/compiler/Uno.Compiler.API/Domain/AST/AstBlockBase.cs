using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public abstract class AstBlockBase : AstClassMember
    {
        public readonly AstIdentifier Name;
        public readonly IReadOnlyList<AstBlockMember> Members;

        protected AstBlockBase(string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond, AstIdentifier name, IReadOnlyList<AstBlockMember> members)
            : base(comment, attributes, modifiers, cond)
        {
            Name = name ?? new AstInvalid(Source.Unknown);
            Members = members;
        }

        public override string ToString()
        {
            return Name.Symbol;
        }
    }
}
