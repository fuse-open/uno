using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstField : AstNamedMember
    {
        public readonly FieldModifiers FieldModifiers;
        public readonly AstExpression InitValue;

        public override AstMemberType MemberType => AstMemberType.Field;

        public AstField(string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond, FieldModifiers fieldModifiers, AstExpression dataType, AstIdentifier name, AstExpression initValue)
            : base(comment, attributes, modifiers, cond, dataType, name)
        {
            FieldModifiers = fieldModifiers;
            InitValue = initValue;
        }
    }
}
