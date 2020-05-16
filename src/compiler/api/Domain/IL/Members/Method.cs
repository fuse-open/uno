using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Method : Function, IGenericEntity
    {
        public readonly List<DrawBlock> DrawBlocks = new List<DrawBlock>();
        public new readonly ClassType GenericType;
        public new readonly Member DeclaringMember;
        public Method OverriddenMethod { get; private set; }
        public Method ImplementedMethod { get; private set; }

        internal Method(Source src, Member owner, string comment,
            Modifiers modifiers, string name, DataType returnType, Parameter[] parameters, Scope optionalBody = null)
            : base(src, owner.DeclaringType, name, returnType, parameters, optionalBody, comment, modifiers)
        {
            DeclaringMember = owner;
        }

        public Method(Source src, DataType owner, string comment,
            Modifiers modifiers, string name, DataType returnType, Parameter[] parameters, Scope optionalBody = null)
            : base(src, owner, name, returnType, parameters, optionalBody, comment, modifiers)
        {
        }

        public Method(Source src, DataType owner, string comment,
            Modifiers modifiers, string name, ClassType genericType, DataType returnType, Parameter[] parameters, Scope optionalBody = null)
            : base(src, owner, name, returnType, parameters, optionalBody, comment, modifiers)
        {
            if (genericType != null)
            {
                genericType.Stats |= EntityStats.GenericMethodType;
                GenericType = genericType;
            }
        }

        public override MemberType MemberType => MemberType.Method;
        public Method GenericDefinition => GenericType.GenericDefinition.Methods[0];

        public IEnumerable<Method> GenericParameterizations
        {
            get
            {
                foreach (var p in GenericType.GenericParameterizations)
                    yield return p.Methods[0];
            }
        }

        public new bool IsGenericMethod => GenericType != null;
        public bool IsGenericDefinition => GenericType != null && GenericType.IsGenericDefinition;
        public GenericParameterType[] GenericParameters => GenericType.GenericParameters;
        public bool IsGenericParameterization => GenericType != null && GenericType.IsGenericParameterization;
        public DataType[] GenericArguments => GenericType.GenericArguments;

        public Method VirtualBase
        {
            get
            {
                var m = this;
                while (m.OverriddenMethod != null)
                    m = m.OverriddenMethod;
                return m;
            }
        }

        public void SetOverriddenMethod(Method overriddenMethod)
        {
            OverriddenMethod = overriddenMethod;
        }

        public void SetImplementedMethod(Method decl)
        {
            ImplementedMethod = decl;
            UnoName = decl.DeclaringType + "." + decl.UnoName;
        }

        public override void SetMasterDefinition(Member master)
        {
            var m = (Method)master;
            base.SetMasterDefinition(m);
            GenericType?.SetMasterDefinition(m?.GenericType.MasterDefinition);
        }

        IGenericEntity IGenericEntity.GenericDefinition => GenericDefinition;
        IEnumerable<IGenericEntity> IGenericEntity.GenericParameterizations => GenericParameterizations;
    }
}