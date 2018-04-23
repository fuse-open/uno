using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Event : Member
    {
        public Event OverriddenEvent { get; private set; }
        public Event ImplementedEvent { get; private set; }
        public Method AddMethod { get; private set; }
        public Method RemoveMethod { get; private set; }
        public Field ImplicitField { get; private set; }

        public Event(Source src, string comment, Modifiers modifiers, DataType owner, DataType type, string name)
            : base(src, comment, modifiers, name, owner, type)
        {
        }

        public override MemberType MemberType => MemberType.Event;

        public Field CreateImplicitField(Source src)
        {
            return ImplicitField = new Field(src, this, UnoName, DocComment,
                (IsStatic ? Modifiers.Static : 0) | Modifiers.Private | Modifiers.Generated, 0, ReturnType);
        }

        public Method CreateAddMethod(Source src, Modifiers modifiers, Scope optionalBody = null)
        {
            return AddMethod = new Method(src, this, DocComment, modifiers, 
                "add_" + UnoName, DataType.Void, new[] { new Parameter(Source, AttributeList.Empty, 0, ReturnType, "value", null) }, optionalBody);
        }

        public Method CreateRemoveMethod(Source src, Modifiers modifiers, Scope optionalBody = null)
        {
            return RemoveMethod = new Method(src, this, DocComment, modifiers, 
                "remove_" + UnoName, DataType.Void, new[] { new Parameter(Source, AttributeList.Empty, 0, ReturnType, "value", null) }, optionalBody);
        }

        public void RemoveAddMethod()
        {
            AddMethod = null;
        }

        public void RemoveRemoveMethod()
        {
            RemoveMethod = null;
        }

        public override void SetMasterDefinition(Member master)
        {
            base.SetMasterDefinition(master);
            AddMethod?.SetMasterDefinition(((Event) master)?.AddMethod);
            RemoveMethod?.SetMasterDefinition(((Event) master)?.RemoveMethod);
            ImplicitField?.SetMasterDefinition(((Event) master)?.ImplicitField);
        }

        public void SetOverriddenEvent(Event overriddenEvent)
        {
            OverriddenEvent = overriddenEvent;
            AddMethod?.SetOverriddenMethod(overriddenEvent?.AddMethod);
            RemoveMethod?.SetOverriddenMethod(overriddenEvent?.RemoveMethod);
        }

        public void SetImplementedEvent(Event decl)
        {
            ImplementedEvent = decl;
            UnoName = decl.DeclaringType + "." + decl.UnoName;

            if (AddMethod != null && decl.AddMethod != null)
                AddMethod.SetImplementedMethod(decl.AddMethod);
            if (RemoveMethod != null && decl.RemoveMethod != null)
                RemoveMethod.SetImplementedMethod(decl.RemoveMethod);
        }
    }
}