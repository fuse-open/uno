using System;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public class Parameter : SourceObject, IEntity
    {
        public readonly NewObject[] Attributes;
        public readonly string UnoName;

        public DataType Type;
        public ParameterModifier Modifier;
        public Expression OptionalDefault;

        public Parameter(Source src, NewObject[] attributes, ParameterModifier modifier, DataType dt, string name, Expression optionalDefault)
            : base(src)
        {
            Attributes = attributes;
            Modifier = modifier;
            Type = dt;
            UnoName = name;
            OptionalDefault = optionalDefault;
        }

        public bool IsReference => Modifier == ParameterModifier.Ref || Modifier == ParameterModifier.Out || Modifier == ParameterModifier.Const;

        protected string _name;
        public string Name => _name ?? UnoName;

        public void SetName(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        Source IEntity.Source => Source;
        IEntity IEntity.MasterDefinition => this;
        NewObject[] IEntity.Attributes => Attributes;
        EntityType IEntity.EntityType => EntityType.Other;
        MemberType IEntity.MemberType => MemberType.Other;
        NamescopeType IEntity.NamescopeType => NamescopeType.Other;
        DataType IEntity.DeclaringType { get { throw new InvalidOperationException(); } }
        DataType IEntity.ReturnType => Type;
        string IEntity.DocComment => null;
        string IEntity.FullName => Name;
        string IEntity.UnoName => UnoName;
        string IEntity.Name => Name;
        bool IEntity.HasRefCount => true;
        bool IEntity.IsStripped => false;
        bool IEntity.IsPublic => true;
        bool IEntity.IsProtected => false;
        bool IEntity.IsPrivate => false;
        bool IEntity.IsInternal => false;
        bool IEntity.IsPartial => false;
        bool IEntity.IsStatic => false;
        bool IEntity.IsGenerated => false;
        bool IEntity.IsAbstract => false;
        bool IEntity.IsVirtual => false;
        bool IEntity.IsVirtualBase => false;
        bool IEntity.IsVirtualOverride => false;
        bool IEntity.IsSealed => false;
        bool IEntity.IsExtern => false;
        bool IEntity.IsImplicitCast => false;
        bool IEntity.IsExplicitCast => false;
        bool IEntity.IsAccessibleFrom(Source src) => Source.Package.IsAccessibleFrom(src.Package);
        SourcePackage IEntity.Package => Source.Package;
    }
}