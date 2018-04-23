using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstMetaProperty : AstBlockMember
    {
        public readonly MetaVisibility Visibility;
        public readonly AstExpression OptionalType;
        public readonly AstIdentifier Name;
        public readonly IReadOnlyList<AstMetaPropertyDefinition> Definitions;

        public override AstMemberType MemberType => AstMemberType.MetaProperty;

        public AstMetaProperty(MetaVisibility visibility, AstExpression optionalType, AstIdentifier name, IReadOnlyList<AstMetaPropertyDefinition> definitions)
        {
            Visibility = visibility;
            OptionalType = optionalType;
            Name = name;
            Definitions = definitions;
        }

        public AstMetaProperty(MetaVisibility visibility, AstExpression optionalType, AstIdentifier name, params AstMetaPropertyDefinition[] definitions)
        {
            Visibility = visibility;
            OptionalType = optionalType;
            Name = name;
            Definitions = definitions;
        }

        public AstMetaProperty(MetaVisibility visibility, AstExpression optionalType, AstIdentifier name, AstExpression value)
            : this(visibility, optionalType, name, new AstMetaPropertyDefinition(value))
        {
        }
    }
}