using System;
using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class DataType : Namescope, IComparable<DataType>, IParametersEntity, IGenericEntity
    {
        DataType _master;
        DataType _prototype;
        Method _dummyMethod;

        public abstract TypeType TypeType { get; }
        public readonly BuiltinType BuiltinType;
        public Modifiers Modifiers;
        public RefArrayType RefArray;

        public string DocComment { get; }
        public NewObject[] Attributes { get; private set; }
        public DataType Base { get; private set; }
        public InterfaceType[] Interfaces { get; private set; }
        public Block Block { get; private set; }

        // Members
        public Constructor Initializer;
        public Finalizer Finalizer;
        public readonly List<Cast> Casts = new List<Cast>();
        public readonly List<Constructor> Constructors = new List<Constructor>();
        public readonly List<Event> Events = new List<Event>();
        public readonly List<Field> Fields = new List<Field>();
        public readonly List<Literal> Literals = new List<Literal>();
        public readonly List<Method> Methods = new List<Method>();
        public readonly List<Operator> Operators = new List<Operator>();
        public readonly List<Property> Properties = new List<Property>();
        public readonly List<DataType> Swizzlers = new List<DataType>();
        public readonly Dictionary<Method, Method> InterfaceMethods = new Dictionary<Method, Method>();
        public readonly List<DataType> NestedTypes = new List<DataType>();

        // Optimizing
        public readonly List<DataType> StrippedTypes = new List<DataType>();
        public readonly List<Member> StrippedMembers = new List<Member>();
        public readonly HashSet<string> SourceFiles = new HashSet<string>();
        public EntityStats Stats;
        public object Tag;

        // Compiler hooks
        public Action<DataType> AssigningBaseType;
        public Action<DataType> PopulatingMembers;
        public void AssignBaseType() { ActionQueue.Dequeue(ref AssigningBaseType, this); }
        public void PopulateMembers() { ActionQueue.Dequeue(ref PopulatingMembers, this); }

        // Generics
        protected GenericParameterType[] _flattenedParameters;
        DataType[] _flattenedArguments;

        protected DataType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, name)
        {
            DocComment = comment;
            Modifiers = modifiers;
            Interfaces = InterfaceTypes.Empty;
            Attributes = AttributeList.Empty;
            SourceFiles.Add(src.FullPath);

            if (!IsGenericParameter && parent is DataType &&
                (parent as DataType).IsFlattenedDefinition)
            {
                var parentParams = (parent as DataType).FlattenedParameters;
                _flattenedParameters = new GenericParameterType[parentParams.Length];

                for (int i = 0; i < _flattenedParameters.Length; i++)
                    _flattenedParameters[i] = parentParams[i];
            }

            if (IsIntrinsic)
                Intrinsics.TryGetValue(QualifiedName, out BuiltinType);
        }

        public bool IsMasterDefinition => MasterDefinition == this;
        public bool IsArray => this is ArrayType;

        public bool IsClosed
        {
            get
            {
                switch (TypeType)
                {
                    case TypeType.Enum:
                        return true;
                    case TypeType.GenericParameter:
                        return false;
                    case TypeType.RefArray:
                        return ElementType.IsClosed;
                    default:
                        if (IsFlattenedParameterization)
                            foreach (var e in FlattenedArguments)
                                if (!e.IsClosed)
                                    return false;

                        return !IsFlattenedDefinition;
                }
            }
        }

        /// <summary>
        /// True if calling a static method on the type has to force type initialization.
        /// </summary>
        public bool HasInitializer
        {
            get
            {
                switch (TypeType)
                {
                    case TypeType.Class:
                    case TypeType.Struct:
                        // Optimization: 'char' and 'string' are pre-initialized.
                        switch (BuiltinType)
                        {
                            case BuiltinType.Char:
                            case BuiltinType.String:
                                return false;
                        }

                        return Initializer != null;
                    default:
                        return false;
                }
            }
        }

        public DataType ElementType => ((ArrayType) this).ElementType;
        public DataType ReturnType => ((DelegateType) this).ReturnType;
        public Parameter[] Parameters => ((DelegateType) this).Parameters;
        public new DataType MasterDefinition => _master ?? this;
        public DataType Prototype => _prototype ?? this;
        public override string Name => _name ?? (MasterDefinition != this ? MasterDefinition.Name : UnoName);
        public sealed override NamescopeType NamescopeType => NamescopeType.DataType;

        public bool IsAccessibleFrom(Source src)
        {
            return Source.Package.IsAccessibleFrom(src.Package) && (
                    IsProtected || !IsInternal ||
                    Source.Package == src.Package ||
                    Source.Package.InternalsVisibleTo.Contains(src.Package.Name) ||
                    src.IsUnknown);
        }

        public void SetBase(DataType baseType)
        {
            Base = baseType;
        }

        public void SetInterfaces(params InterfaceType[] interfaceTypes)
        {
            Interfaces = interfaceTypes;
        }

        public void SetAttributes(params NewObject[] attributes)
        {
            Attributes = attributes;
        }

        public void SetBlock(Block block)
        {
            Block = block;
        }

        public void SetMasterDefinition(DataType master)
        {
            _master = master;
        }

        public void SetPrototype(DataType prototype)
        {
            _prototype = prototype.Prototype;
        }

        public virtual void MakeGenericDefinition(params GenericParameterType[] typeParams)
        {
            throw new InvalidOperationException();
        }

        public virtual bool IsGenericDefinition => false;
        public virtual bool IsGenericParameterization => false;

        public virtual GenericParameterType[] GenericParameters
        {
            get { throw new InvalidOperationException(); }
        }

        public virtual List<DataType> GenericParameterizations
        {
            get { throw new InvalidOperationException(); }
        }

        public virtual DataType CreateParameterization(params DataType[] args)
        {
            throw new InvalidOperationException();
        }

        public virtual DataType[] GenericArguments
        {
            get { throw new InvalidOperationException(); }
        }

        public virtual DataType GenericDefinition
        {
            get { throw new InvalidOperationException(); }
        }

        public bool IsFlattenedDefinition => _flattenedParameters != null;
        public bool IsFlattenedParameterization => IsGenericParameterization || !IsGenericDefinition && !IsGenericParameter && IsNestedType && ParentType.IsFlattenedParameterization;
        public GenericParameterType[] FlattenedParameters => _flattenedParameters;

        public DataType[] FlattenedArguments
        {
            get
            {
                if (_flattenedArguments == null)
                {
                    var args = new List<DataType>();
                    var dt = ParentType;

                    while (dt != null)
                    {
                        if (dt.IsGenericParameterization)
                            args.InsertRange(0, dt.GenericArguments);
                        else if (dt.IsGenericDefinition)
                            args.InsertRange(0, dt.GenericParameters);

                        dt = dt.ParentType;
                    }

                    if (IsGenericParameterization)
                        args.AddRange(GenericArguments);

                    _flattenedArguments = args.ToArray();
                }

                return _flattenedArguments;
            }
        }

        public int GenericIndex
        {
            get
            {
                var pt = ParentType;
                return pt != null && pt.IsFlattenedDefinition
                    ? Array.IndexOf(pt.FlattenedParameters, this)
                    : -1;
            }
        }

        public string GenericSuffix => GetGenericSuffix(this);
        public string AttributeString => "[" + ToString().Replace("Attribute", "") + "]";
        public string VerboseName => IsGenericType
                                        ? Parent + "." + Name
                                        : ToString();

        public override string FullName
        {
            get
            {
                if (this != Prototype)
                    return Prototype.FullName;

                string alias, name = QualifiedName;
                return TypeAliases.TryGetAliasFromType(name, out alias)
                    ? alias + GenericSuffix
                    : name + GenericSuffix;
            }
        }

        internal static string GetGenericSuffix(DataType dt)
        {
            if (dt != null)
            {
                if (dt.IsGenericDefinition)
                    return "`" + dt.GenericParameters.Length;

                if (dt.IsGenericParameterization)
                {
                    var sb = new StringBuilder("<");

                    for (int i = 0; i < dt.GenericArguments.Length; i++)
                    {
                        sb.AppendWhen(i > 0, ", ");
                        sb.Append(dt.GenericArguments[i]);
                    }

                    sb.Append(">");
                    return sb.ToString();
                }
            }

            return "";
        }

        public int CompareTo(DataType other)
        {
            if (other != null)
            {
                var diff = string.Compare(UnoName, other.UnoName, StringComparison.InvariantCulture);
                return diff != 0 ? diff :
                    IsGenericDefinition && other.IsGenericDefinition
                        ? GenericParameters.Length - other.GenericParameters.Length
                        : string.Compare(GenericSuffix, other.GenericSuffix, StringComparison.InvariantCulture);
            }

            return string.Compare(UnoName, null, StringComparison.InvariantCulture);
        }

        public void Visit(Pass p, bool recursive = true)
        {
            if (!p.Begin(this))
                return;

            var old = p.Type;
            p.Type = this;

            if (recursive)
                for (int i = 0; i < NestedTypes.Count; i++)
                    NestedTypes[i].Visit(p);

            p.Function = DummyMethod;

            Initializer?.Visit(p);
            Finalizer?.Visit(p);

            for (int i = 0; i < Methods.Count; i++)
                Methods[i].Visit(p);
            for (int i = 0; i < Constructors.Count; i++)
                Constructors[i].Visit(p);

            for (int i = 0; i < Properties.Count; i++)
            {
                var m = Properties[i];
                m.GetMethod?.Visit(p);
                m.SetMethod?.Visit(p);
            }

            for (int i = 0; i < Events.Count; i++)
            {
                var m = Events[i];
                m.AddMethod?.Visit(p);
                m.RemoveMethod?.Visit(p);
            }

            for (int i = 0; i < Operators.Count; i++)
                Operators[i].Visit(p);
            for (int i = 0; i < Casts.Count; i++)
                Casts[i].Visit(p);

            p.Function = null;

            Block?.Visit(p);

            p.Type = old;
            p.End(this);
        }

        Method DummyMethod => _dummyMethod ?? (
                                _dummyMethod = new Method(Source, this, null,
                                    Modifiers.Static | Modifiers.Generated,
                                    ".member_init", Void, ParameterList.Empty)
                            );

        public bool IsStripped
        {
            get
            {
                if (IsGenericParameterization)
                    return GenericDefinition.IsStripped;

                if (Parent != null)
                {
                    switch (Parent.NamescopeType)
                    {
                        case NamescopeType.DataType:
                            return ParentType.MasterDefinition.StrippedTypes.Contains(MasterDefinition) || ParentType.IsStripped;
                        case NamescopeType.Namespace:
                            return ParentNamespace.StrippedTypes.Contains(MasterDefinition);
                    }
                }

                return false;
            }
        }

        public bool IsReferenceType
        {
            get
            {
                switch (TypeType)
                {
                    case TypeType.Class:
                    case TypeType.RefArray:
                    case TypeType.Interface:
                    case TypeType.Delegate:
                        return true;
                    case TypeType.GenericParameter:
                        return ((GenericParameterType) this).ConstraintType == GenericConstraintType.Class;
                    default:
                        return false;
                }
            }
        }

        public bool IsValueType
        {
            get
            {
                switch (TypeType)
                {
                    case TypeType.Struct:
                    case TypeType.Enum:
                        return true;
                    case TypeType.GenericParameter:
                        return ((GenericParameterType) this).ConstraintType == GenericConstraintType.Struct;
                    default:
                        return false;
                }
            }
        }

        public bool IsUnsignedType
        {
            get
            {
                switch (BuiltinType)
                {
                    case BuiltinType.Byte:
                    case BuiltinType.Char:
                    case BuiltinType.UShort:
                    case BuiltinType.UInt:
                    case BuiltinType.ULong:
                        return true;
                    default:
                        return IsEnum && Base.IsUnsignedType;
                }
            }
        }

        public bool IsIntegralType
        {
            get
            {
                switch (BuiltinType)
                {
                    case BuiltinType.Int:
                    case BuiltinType.Long:
                    case BuiltinType.Short:
                    case BuiltinType.SByte:
                    case BuiltinType.UInt:
                    case BuiltinType.UShort:
                    case BuiltinType.ULong:
                    case BuiltinType.Byte:
                    case BuiltinType.Char:
                        return true;
                    default:
                        return IsEnum && Base.IsIntegralType;
                }
            }
        }

        public bool IsFloatingPointType
        {
            get
            {
                switch (BuiltinType)
                {
                    case BuiltinType.Float:
                    case BuiltinType.Double:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsGenericType
        {
            get
            {
                switch (TypeType)
                {
                    case TypeType.GenericParameter:
                        return true;

                    case TypeType.FixedArray:
                    case TypeType.RefArray:
                        return ElementType.IsGenericType;

                    default:
                        if (IsFlattenedParameterization)
                            foreach (var p in FlattenedArguments)
                                if (p.IsGenericType)
                                    return true;

                        return IsFlattenedDefinition;
                }
            }
        }

        IGenericEntity IGenericEntity.GenericDefinition => GenericDefinition;
        IEnumerable<IGenericEntity> IGenericEntity.GenericParameterizations => GenericParameterizations;

        public bool IsInvalid => TypeType == TypeType.Invalid;
        public bool IsVoid => TypeType == TypeType.Void;
        public bool IsNull => TypeType == TypeType.Null;
        public bool IsEnum => TypeType == TypeType.Enum;
        public bool IsClass => TypeType == TypeType.Class;
        public bool IsStruct => TypeType == TypeType.Struct;
        public bool IsInterface => TypeType == TypeType.Interface;
        public bool IsDelegate => TypeType == TypeType.Delegate;
        public bool IsRefArray => TypeType == TypeType.RefArray;
        public bool IsFixedArray => TypeType == TypeType.FixedArray;
        public bool IsGenericParameter => TypeType == TypeType.GenericParameter;
        public bool IsGenericMethodType => Stats.HasFlag(EntityStats.GenericMethodType);
        public bool IsParameterizedDefinition => Stats.HasFlag(EntityStats.ParameterizedDefinition);
        public bool CanLink => MasterDefinition.Stats.HasFlag(EntityStats.CanLink);
        public bool HasRefCount => Stats.HasFlag(EntityStats.RefCount);

        Source IEntity.Source => Source;
        IEntity IEntity.MasterDefinition => MasterDefinition;
        DataType IEntity.DeclaringType => ParentType;
        MemberType IEntity.MemberType => MemberType.Other;
        public SourcePackage Package => Source.Package;
        public bool IsPublic => Modifiers.HasFlag(Modifiers.Public);
        public bool IsProtected => Modifiers.HasFlag(Modifiers.Protected);
        public bool IsPrivate => Modifiers.HasFlag(Modifiers.Private);
        public bool IsInternal => Modifiers.HasFlag(Modifiers.Internal);
        public bool IsPartial => Modifiers.HasFlag(Modifiers.Partial);
        public bool IsStatic => Modifiers.HasFlag(Modifiers.Static);
        public bool IsIntrinsic => Modifiers.HasFlag(Modifiers.Intrinsic);
        public bool IsGenerated => Modifiers.HasFlag(Modifiers.Generated);
        public bool IsAbstract => Modifiers.HasFlag(Modifiers.Abstract);
        bool IEntity.IsVirtual => (Modifiers & (Modifiers.Virtual | Modifiers.Override | Modifiers.Abstract)) != 0;
        bool IEntity.IsVirtualBase => Modifiers.HasFlag(Modifiers.Virtual);
        bool IEntity.IsVirtualOverride => Modifiers.HasFlag(Modifiers.Override);
        public bool IsSealed => Modifiers.HasFlag(Modifiers.Sealed);
        public bool IsExtern => Modifiers.HasFlag(Modifiers.Extern);
        bool IEntity.IsImplicitCast => Modifiers.HasFlag(Modifiers.Implicit);
        bool IEntity.IsExplicitCast => Modifiers.HasFlag(Modifiers.Explicit);

        public static readonly DataType Null = new NullType();
        public static readonly DataType Void = new VoidType();
        public static readonly DataType Invalid = new InvalidType();
        public static readonly DataType MethodGroup = new MethodGroupType();

        static readonly Dictionary<string, BuiltinType> Intrinsics = new Dictionary<string, BuiltinType>()
        {
            { "Uno.Bool", BuiltinType.Bool },
            { "Uno.Byte", BuiltinType.Byte },
            { "Uno.Byte2", BuiltinType.Byte2 },
            { "Uno.Byte4", BuiltinType.Byte4 },
            { "Uno.Char", BuiltinType.Char },
            { "Uno.Double", BuiltinType.Double },
            { "Uno.Float", BuiltinType.Float },
            { "Uno.Float2", BuiltinType.Float2 },
            { "Uno.Float2x2", BuiltinType.Float2x2 },
            { "Uno.Float3", BuiltinType.Float3 },
            { "Uno.Float4", BuiltinType.Float4 },
            { "Uno.Float3x3", BuiltinType.Float3x3 },
            { "Uno.Float4x4", BuiltinType.Float4x4 },
            { "Uno.Graphics.Sampler2D", BuiltinType.Sampler2D },
            { "Uno.Graphics.SamplerCube", BuiltinType.SamplerCube },
            { "Uno.Graphics.Texture2D", BuiltinType.Texture2D },
            { "Uno.Graphics.TextureCube", BuiltinType.TextureCube },
            { "Uno.Graphics.VideoTexture", BuiltinType.VideoTexture },
            { "Uno.Graphics.VideoSampler", BuiltinType.VideoSampler },
            { "Uno.Int", BuiltinType.Int },
            { "Uno.Int2", BuiltinType.Int2 },
            { "Uno.Int3", BuiltinType.Int3 },
            { "Uno.Int4", BuiltinType.Int4 },
            { "Uno.IntPtr", BuiltinType.IntPtr },
            { "Uno.Long", BuiltinType.Long },
            { "Uno.Object", BuiltinType.Object },
            { "Uno.SByte", BuiltinType.SByte },
            { "Uno.SByte2", BuiltinType.SByte2 },
            { "Uno.SByte4", BuiltinType.SByte4 },
            { "Uno.Short", BuiltinType.Short },
            { "Uno.Short2", BuiltinType.Short2 },
            { "Uno.Short4", BuiltinType.Short4 },
            { "Uno.String", BuiltinType.String },
            { "Uno.UInt", BuiltinType.UInt },
            { "Uno.ULong", BuiltinType.ULong },
            { "Uno.UShort", BuiltinType.UShort },
            { "Uno.UShort2", BuiltinType.UShort2 },
            { "Uno.UShort4", BuiltinType.UShort4 },
        };
    }
}
