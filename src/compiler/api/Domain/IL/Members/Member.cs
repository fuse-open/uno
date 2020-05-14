using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public abstract class Member : Entity, IEntity
    {
        protected Member _master;
        protected Member _prototype;

        public string DocComment { get; }
        public NewObject[] Attributes { get; private set; }
        public DataType DeclaringType { get; }
        public DataType ReturnType;
        public Modifiers Modifiers;
        public EntityStats Stats;
        public object Tag;

        protected Member(Source src, string comment, Modifiers modifiers, string name, DataType declaringType, DataType returnType)
            : base(src, name)
        {
            DocComment = comment;
            Attributes = AttributeList.Empty;
            DeclaringType = declaringType;
            ReturnType = returnType;
            Modifiers = modifiers;
        }

        public override EntityType EntityType => EntityType.Member;
        public abstract MemberType MemberType { get; }

        public string FullName => this != Prototype ? Prototype.FullName : DeclaringType + "." + Name + (this is Method ? ((Method) this).GenericSuffix : "");

        public override string Name => _name ?? (MasterDefinition != this ? MasterDefinition.Name : UnoName);

        public void SetAttributes(params NewObject[] attributes)
        {
            Attributes = attributes;
        }

        public bool IsMasterDefinition => MasterDefinition == this;

        public Member MasterDefinition => _master ?? this;

        public virtual void SetMasterDefinition(Member master)
        {
            _master = master;
        }

        public bool IsPrototype => Prototype == this;

        public Member Prototype => _prototype ?? this;

        public void SetPrototype(Member prototype)
        {
            _prototype = prototype.Prototype;
        }

        public override string ToString()
        {
            return this != Prototype
                ? Prototype.ToString()
                : DeclaringType + "." + UnoName;
        }

        public Member DeclaringMember
        {
            get
            {
                switch (MemberType)
                {
                    case MemberType.Field:
                        return ((Field) this).DeclaringMember;
                    case MemberType.Method:
                        return ((Method) this).DeclaringMember;
                    default:
                        return null;
                }
            }
        }

        public bool HasRefCount => Stats.HasFlag(EntityStats.RefCount);
        public bool IsStripped => DeclaringType.MasterDefinition.StrippedMembers.Contains(MasterDefinition) || DeclaringType.IsStripped;

        Source IEntity.Source => Source;
        IEntity IEntity.MasterDefinition => MasterDefinition;
        DataType IEntity.DeclaringType => DeclaringType;
        DataType IEntity.ReturnType => ReturnType;
        NamescopeType IEntity.NamescopeType => NamescopeType.Other;
        SourcePackage IEntity.Package => Source.Package;
        public bool IsPublic => MasterDefinition.Modifiers.HasFlag(Modifiers.Public);
        public bool IsProtected => MasterDefinition.Modifiers.HasFlag(Modifiers.Protected);
        public bool IsPrivate => MasterDefinition.Modifiers.HasFlag(Modifiers.Private);
        public bool IsInternal => MasterDefinition.Modifiers.HasFlag(Modifiers.Internal);
        public bool IsPartial => MasterDefinition.Modifiers.HasFlag(Modifiers.Partial);
        public bool IsStatic => MasterDefinition.Modifiers.HasFlag(Modifiers.Static);
        public bool IsIntrinsic => MasterDefinition.Modifiers.HasFlag(Modifiers.Intrinsic);
        public bool IsGenerated => MasterDefinition.Modifiers.HasFlag(Modifiers.Generated);
        public bool IsAbstract => MasterDefinition.Modifiers.HasFlag(Modifiers.Abstract);
        public bool IsVirtual => (MasterDefinition.Modifiers & (Modifiers.Virtual | Modifiers.Override | Modifiers.Abstract)) != 0;
        public bool IsVirtualBase => MasterDefinition.Modifiers.HasFlag(Modifiers.Virtual);
        public bool IsVirtualOverride => MasterDefinition.Modifiers.HasFlag(Modifiers.Override);
        public bool IsSealed => MasterDefinition.Modifiers.HasFlag(Modifiers.Sealed);
        public bool IsExtern => MasterDefinition.Modifiers.HasFlag(Modifiers.Extern);
        public bool IsImplicitCast => MasterDefinition.Modifiers.HasFlag(Modifiers.Implicit);
        public bool IsExplicitCast => MasterDefinition.Modifiers.HasFlag(Modifiers.Explicit);
        bool IEntity.IsAccessibleFrom(Source src) => DeclaringType.IsAccessibleFrom(src);
    }
}
