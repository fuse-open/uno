using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Property : ParametersMember
    {
        public Property OverriddenProperty { get; private set; }
        public Property ImplementedProperty { get; private set; }
        public Field ImplicitField { get; private set; }
        public Method GetMethod { get; private set; }
        public Method SetMethod { get; private set; }

        public Property(Source src, string comment, Modifiers modifiers, string name, DataType owner, DataType type, params Parameter[] parameters)
            : base(src, comment, modifiers, name, owner, type, parameters)
        {
        }

        public override MemberType MemberType => MemberType.Property;

        public string NameWithParameterList => Parameters.Length == 0 ?
            Name :
            (ImplementedProperty?.DeclaringType.ToString() ?? "this") +
            Parameters.BuildString("[", "]");

        public Field CreateImplicitField(Source src)
        {
            return ImplicitField = new Field(src, this, "_" + UnoName, DocComment,
                (IsStatic ? Modifiers.Static : 0) | Modifiers.Private | Modifiers.Generated, 0, ReturnType);
        }

        public Method CreateGetMethod(Source src, Modifiers modifiers, Scope optionalBody = null)
        {
            return GetMethod = new Method(src, this, DocComment, modifiers, 
                "get_" + UnoName, ReturnType, Parameters, optionalBody);
        }

        public Method CreateSetMethod(Source src, Modifiers modifiers, Scope optionalBody = null)
        {
            var p = new List<Parameter>();
            p.AddRange(Parameters);
            p.Add(new Parameter(src, AttributeList.Empty, 0, ReturnType, "value", null));

            return SetMethod = new Method(src, this, DocComment, modifiers,
                "set_" + UnoName, DataType.Void, p.ToArray(), optionalBody);
        }

        public void RemoveGetter()
        {
            GetMethod = null;
        }

        public void RemoveSetter()
        {
            SetMethod = null;
        }

        public override void SetMasterDefinition(Member master)
        {
            base.SetMasterDefinition(master);
            ImplicitField?.SetMasterDefinition(((Property) master)?.ImplicitField);
            GetMethod?.SetMasterDefinition(((Property) master)?.GetMethod);
            SetMethod?.SetMasterDefinition(((Property) master)?.SetMethod);
        }

        public void SetOverriddenProperty(Property overriddenProperty)
        {
            OverriddenProperty = overriddenProperty;
            GetMethod?.SetOverriddenMethod(overriddenProperty?.GetMethod);
            SetMethod?.SetOverriddenMethod(overriddenProperty?.SetMethod);
        }

        public void SetImplementedProperty(Property decl)
        {
            ImplementedProperty = decl;
            UnoName = decl.DeclaringType + "." + decl.UnoName;

            if (GetMethod != null && decl.GetMethod != null)
                GetMethod.SetImplementedMethod(decl.GetMethod);
            if (SetMethod != null && decl.SetMethod != null)
                SetMethod.SetImplementedMethod(decl.SetMethod);
        }

        public override string ToString()
        {
            return
                Parameters.Length == 0 ?
                    base.ToString() :
                    DeclaringType +
                    (ImplementedProperty != null ? "." + ImplementedProperty.DeclaringType : "") +
                    Parameters.BuildString("[", "]");
        }
    }
}