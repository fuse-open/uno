using System;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public abstract class ResolvedGenericProperty : IProperty
    {
        readonly IProperty _p;
        readonly Func<IDataType> _actualType;

        public bool IsActualDataTypeAvailable => _actualType() != null;

        IDataType ActualType
        {
            get
            {
                var t = _actualType();
                if (t == null) throw new Exception("Generic parameter not resolved yet");
                return t;
            }
        }

        protected ResolvedGenericProperty(IProperty p, Func<IDataType> actualType)
        {
            _p = p;
            _actualType = actualType;
        }

        public string UXAuxNameTable => _p.UXAuxNameTable;

        public IdentifierScope UXIdentifierScope => _p.UXIdentifierScope;

        public bool IsUXVerbatim => _p.IsUXVerbatim;

        public int UXArgumentIndex => _p.UXArgumentIndex;

        public bool IsOfGenericArgumentType => false;

        public IDataType ListItemType => ActualType;

        public IDataType DeclaringType => _p.DeclaringType;

        public string Name => _p.Name;

        public IDataType DataType => ActualType;

        public bool Accepts(IDataType type)
        {
            return _p.Accepts(type);
        }

        public PropertyType PropertyType => _p.PropertyType;

        public AutoBindingType AutoBindingType => _p.AutoBindingType;


        public bool IsConstructorArgument => _p.IsConstructorArgument;

        public bool IsUXNameProperty => _p.IsUXNameProperty;

        public bool IsUXFileNameProperty => _p.IsUXFileNameProperty;

        public bool IsUXAutoNameTableProperty => _p.IsUXAutoNameTableProperty;

        public bool IsUXAutoClassNameProperty => _p.IsUXAutoClassNameProperty;
    }

    public sealed class ResolvedGenericConstructorArgument: ResolvedGenericProperty, IConstructorArgument
    {
        readonly IConstructorArgument _ca;

        public ResolvedGenericConstructorArgument(IConstructorArgument ca, Func<IDataType> actualType): base(ca, actualType)
        {
            _ca = ca;
        }

        public string DefaultValue
        {
            get
            {
                return _ca.DefaultValue;
            }
        }
    }


    public sealed class ResolvedGenericMutableProperty : ResolvedGenericProperty, IMutableProperty
    {
        readonly IMutableProperty _mp;

        public ResolvedGenericMutableProperty(IMutableProperty mp, Func<IDataType> actualType)
            : base(mp, actualType)
        {
            _mp = mp;
        }

        public string OriginSetterName => null;

        public string ValueChangedEvent => null;

        public bool CanGet => _mp.CanGet;

        public bool CanSet => _mp.CanSet;
    }
}
