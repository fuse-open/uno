namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Field : Member
    {
        public FieldModifiers FieldModifiers;
        public new readonly Member DeclaringMember;

        internal Field(Source src, Member owner, string name, string comment, Modifiers modifiers, FieldModifiers fieldModifiers, DataType type)
            : base(src, comment, modifiers, name, owner.DeclaringType, type)
        {
            DeclaringMember = owner;
            FieldModifiers = fieldModifiers;
        }

        public Field(Source src, DataType owner, string name, string comment, Modifiers modifiers, FieldModifiers fieldModifiers, DataType type)
            : base(src, comment, modifiers, name, owner, type)
        {
            FieldModifiers = fieldModifiers;
        }

        public override MemberType MemberType => MemberType.Field;

        public new Field MasterDefinition => (Field) _master ?? this;

        public int Index => DeclaringType.Fields.IndexOf(this);
    }
}