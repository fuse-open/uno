using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    abstract class DeclaredUXParameter 
    {
        readonly ParameterNode _decl;
        readonly ClassNode _owner;

        public DeclaredUXParameter(ParameterNode decl, ClassNode owner)
        {
            _decl = decl;
            _owner = owner;
        }

        public IdentifierScope UXIdentifierScope => IdentifierScope.Globals;

        public bool IsUXNameProperty => false;

        public bool IsUXFileNameProperty => false;

        public bool IsUXAutoNameTableProperty => false;

        public bool IsUXAutoClassNameProperty => false;

        public bool IsActualDataTypeAvailable => true;

        public bool IsOfGenericArgumentType => false;

        public IDataType ListItemType => _decl.ResultingType;

        public IDataType DeclaringType => _owner;

        public string Name => _decl.Name;

        public IDataType DataType => _decl.ResultingType;

        public bool Accepts(IDataType type)
        {
            return type.Implements(_decl.ResultingType);
        }

        public PropertyType PropertyType
        {
            get
            {
                if (DataType.IsValueType || DataType.IsString) return PropertyType.Atomic;
                else return PropertyType.Reference;
            }
        }

        public AutoBindingType AutoBindingType => AutoBindingType.None;

        public abstract bool IsConstructorArgument { get; }
		
        public string OriginSetterName => "Set" + Name;

        public string ValueChangedEvent => Name + "Changed";

        public bool CanGet => true;

        public bool CanSet => true;

        public int UXArgumentIndex => -1;

        public bool IsUXVerbatim => false;

        public string UXAuxNameTable => null;
    }

	sealed class DeclaredUXProperty: DeclaredUXParameter, IMutableProperty
	{
		public DeclaredUXProperty(PropertyNode decl, ClassNode owner): base(decl, owner) { }
		public override bool IsConstructorArgument => false;
	}

	sealed class DeclaredUXDependency : DeclaredUXParameter, IConstructorArgument
	{
		public DeclaredUXDependency(DependencyNode decl, ClassNode owner) : base(decl, owner) { }
		public override bool IsConstructorArgument => true;

        public string DefaultValue
        {
            get { return null;  /* TODO, introduce UX feature for this */ }
        }

	}
}
