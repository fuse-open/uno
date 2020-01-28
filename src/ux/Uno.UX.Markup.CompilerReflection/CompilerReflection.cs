using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.UX.Markup.Reflection
{
    static class DataTypeExtensions
    {
        // Optimization as reading 'FullName' is slow
        public static string GetCachedFullName(this DataType dt)
        {
            return dt.Tag as string ?? (string)(dt.Tag = dt.FullName);
        }
    }

    abstract class PropertyReflectorBase
    {
        protected CompilerDataTypeProvider _c;

        protected static AutoBindingType GetAutoBindingType(IEnumerable<NewObject> attributes)
        {
            if (attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXPrimaryAttribute")) return Reflection.AutoBindingType.Primary;
            else if (attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXComponentsAttribute")) return Reflection.AutoBindingType.Components;
            else if (attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXContentAttribute")) return Reflection.AutoBindingType.Content;
            else if (attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXLineNumberAttribute")) return Reflection.AutoBindingType.LineNumber;
            else if (attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXSourceFileNameAttribute")) return Reflection.AutoBindingType.SourceFileName;
            else return Reflection.AutoBindingType.None;
        }

        internal PropertyReflectorBase(CompilerDataTypeProvider c)
        {
            _c = c;
        }

        
        public bool IsOfGenericArgumentType => UnoType.IsGenericParameter;

        public bool IsActualDataTypeAvailable => !IsOfGenericArgumentType;

        Dictionary<DataType, DataType> _listElementTypeCache = new Dictionary<DataType, DataType>();

        DataType GetListElementType(DataType dt)
        {
            DataType res;
            if (!_listElementTypeCache.TryGetValue(dt, out res))
            {
                res = dt.Interfaces.Union(new[] { dt }).First(x => x.GetCachedFullName().StartsWith("Uno.Collections.IList")).GenericArguments[0];
                _listElementTypeCache.Add(dt, res);
            }
            return res;
        }

        public IDataType ListItemType
        {
            get
            {
                if (PropertyType == Reflection.PropertyType.List)
                {
                    var listElmType = GetListElementType(UnoType);
                    return _c[listElmType];
                }
                else
                {
                    return _c[UnoType];
                }
            }
        }

        public abstract Uno.Compiler.API.Domain.IL.DataType UnoType
        {
            get;
        }

        public bool Accepts(Reflection.IDataType dt)
        {
            if (PropertyType == Reflection.PropertyType.List)
            {
                var listElmType = GetListElementType(UnoType);
                return ((DataTypeReflector)dt.ActualIDataTypeImpl).UnoDataType.IsCompatibleWith(listElmType);
            }
            else if (PropertyType == Reflection.PropertyType.Reference || PropertyType == Reflection.PropertyType.Atomic)
            {
                return ((DataTypeReflector)dt.ActualIDataTypeImpl).UnoDataType.IsCompatibleWith(UnoType);
            }
            else
            {
                throw new Exception();
            }
        }

        public Reflection.IDataType DataType => _c[UnoType];

        PropertyType? _propType;
        public PropertyType PropertyType
        {
            get
            {
                if (!_propType.HasValue) _propType = ComputePropType();
                return _propType.Value;
            }
        }
        PropertyType ComputePropType()
        {
            if (_c.IsListType(UnoType)) return Reflection.PropertyType.List;
            else if (UnoType is DelegateType) return Reflection.PropertyType.Delegate;
            else if (UnoType.IsReferenceTypeExceptStringAndValue()) return Reflection.PropertyType.Reference;
            else return Reflection.PropertyType.Atomic;
        }
    }

    class AttachedPropertyReflector: PropertyReflectorBase, Uno.UX.Markup.Reflection.IAttachedProperty
    {
        readonly AttachedProperty _apf;

        public AttachedPropertyReflector(AttachedProperty apf, CompilerDataTypeProvider c): base(c)
        {
            _apf = apf;
        }

        public bool IsUXVerbatim => _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXVerbatimAttribute");

        public string UXAuxNameTable
        {
            get { return _apf.SetMethod.Attributes.Where(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAuxNameTableAttribute").Select(x => x.Arguments.First().ConstantString).FirstOrDefault(); }
        }

        public bool IsUXNameProperty
        {
            get { return _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameAttribute"); }
        }

        public int UXArgumentIndex
        {
            get
            {
                var a = _apf.SetMethod.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXArgAttribute");
                if (a != null) return (int)a.Arguments[0].ConstantValue;
                return -1;
            }
        }

        public IdentifierScope UXIdentifierScope
        {
            get
            {
                return
                    _apf.SetMethod == null ? IdentifierScope.Globals : 
                    _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameScopeAttribute") ? IdentifierScope.Names :
                    _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXDataScopeAttribute") ? IdentifierScope.Data :
                    IdentifierScope.Globals;
            }
        }

        public bool IsUXFileNameProperty
        {
            get { return _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXFileNameAttribute"); }
        }

        public bool IsUXAutoNameTableProperty
        {
            get { return _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoNameTableAttribute"); }
        }

        public bool IsUXAutoClassNameProperty
        {
            get { return _apf.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoClassNameAttribute"); }
        }

        public override DataType UnoType => _apf.DataType;

        public string Name => _apf.Name;

        public Reflection.AutoBindingType AutoBindingType => Reflection.AutoBindingType.None;

        public IDataType DeclaringType
        {
            get
            {
                var declType = _apf.GetMethod != null ? _apf.GetMethod.DeclaringType : _apf.SetMethod?.DeclaringType;
                return _c[declType];
            }
        }

        public IDataType OwnerType
        {
            get
            {
                var ownerType = _apf.GetMethod != null ? _apf.GetMethod.Parameters[0].Type : _apf.SetMethod?.Parameters[0].Type;
                return _c[ownerType];
            }
        }

        public string ValueChangedEvent => null;

        public string OriginSetterName => null;


        public bool CanGet => _apf.GetMethod != null && _apf.GetMethod.IsPublic;

        public bool CanSet => _apf.SetMethod != null && _apf.SetMethod.IsPublic;

        public string StyleSetMethodName => _apf.StyleSetMethod?.Name;

        public string SetMethodName => _apf.SetMethod.Name;

        public string GetMethodName => _apf.GetMethod.Name;

        public bool IsConstructorArgument => false;
    }

    class PropertyReflector: PropertyReflectorBase, Uno.UX.Markup.Reflection.IMutableProperty
    {
        readonly Uno.Compiler.API.Domain.IL.Members.Property _property;

        public override Compiler.API.Domain.IL.DataType UnoType => _property.ReturnType;

        public bool IsUXVerbatim => _property.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXVerbatimAttribute");


        public PropertyReflector(Uno.Compiler.API.Domain.IL.Members.Property prop, CompilerDataTypeProvider c): base(c)
        {
            _property = prop;
        }

        public string UXAuxNameTable => null;

        public IdentifierScope UXIdentifierScope
        {
            get
            {
                return
                    _property.SetMethod == null ? IdentifierScope.Globals :
                    _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameScopeAttribute") ? IdentifierScope.Names :
                    _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXDataScopeAttribute") ? IdentifierScope.Data :
                    IdentifierScope.Globals;
            }
        }

        public bool IsUXNameProperty
        {
            get { return CanSet && _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameAttribute"); }
        }

        public int UXArgumentIndex
        {
            get
            {
                if (_property.SetMethod == null) return -1;

                var a = _property.SetMethod.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXArgAttribute");
                if (a != null) return (int)a.Arguments[0].ConstantValue;
                return -1;
            }
        }

        public bool IsUXFileNameProperty
        {
            get { return CanSet && _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXFileNameAttribute"); }
        }

        public bool IsUXAutoNameTableProperty
        {
            get { return CanSet && _property.SetMethod != null && _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoNameTableAttribute"); }
        }

        public bool IsUXAutoClassNameProperty
        {
            get { return CanSet && _property.SetMethod != null && _property.SetMethod.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoClassNameAttribute"); }
        }

        public string Name => _property.Name;

        AutoBindingType? _autoBindingTypeCache;
        public Reflection.AutoBindingType AutoBindingType => _autoBindingTypeCache.HasValue ? _autoBindingTypeCache.Value : (_autoBindingTypeCache = GetAutoBindingType(_property.Attributes)).Value;

        public IDataType DeclaringType => _c[_property.DeclaringType];

        public bool CanGet => _property.GetMethod != null && _property.GetMethod.IsPublic;

        public bool CanSet => _property.SetMethod != null && _property.SetMethod.IsPublic;

        public string OriginSetterName
        {
            get
            {
                var attrib = _property.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXOriginSetterAttribute");

                if (attrib != null)
                {
                    var constant = attrib.Arguments[0] as Constant;
                    if (constant != null)
                        return (string)constant.Value;
                }

                return null;
            }
        }

        public bool IsConstructorArgument
        {
            get { return false; }
        }
    }

    class ConstructorArgumentReflector: PropertyReflectorBase, Uno.UX.Markup.Reflection.IProperty, IConstructorArgument
    {
        readonly Constructor _ctor;
        readonly Parameter _p;

        public ConstructorArgumentReflector(Constructor ctor, Parameter p, CompilerDataTypeProvider c): base(c)
        {
            _ctor = ctor;
            _p = p;

            Name = p.Name;

            var attr = p.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXParameterAttribute");
            if (attr != null) Name = attr.Arguments[0].ConstantValue as string;
        }

        public bool IsUXVerbatim => _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXVerbatimAttribute");

        public IdentifierScope UXIdentifierScope
        {
            get
            {
                return
                    _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameScopeAttribute") ? IdentifierScope.Names :
                    _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXDataScopeAttribute") ? IdentifierScope.Data :
                    IdentifierScope.Globals;
            }
        }

        public string UXAuxNameTable => null;

        public int UXArgumentIndex
        {
            get
            {
                var a = _p.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXArgAttribute");
                if (a != null) return (int)a.Arguments[0].ConstantValue;
                return -1;
            }
        }

        public bool IsUXNameProperty
        {
            get { return _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXNameAttribute"); }
        }

        public bool IsUXFileNameProperty
        {
            get { return _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXFileNameAttribute"); }
        }

        public bool IsUXAutoNameTableProperty
        {
            get { return _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoNameTableAttribute"); }
        }

        public bool IsUXAutoClassNameProperty
        {
            get { return _p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoClassNameAttribute"); }
        }

        public IDataType DeclaringType => _c.TryGetTypeByName(_ctor.DeclaringType.GetCachedFullName());

        public string Name { get; }

        public AutoBindingType AutoBindingType => GetAutoBindingType(_p.Attributes);

        public override DataType UnoType => _p.Type;


        public bool IsConstructorArgument => true;

        public string DefaultValue
        {
            get
            {
                var a = _p.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXDefaultValueAttribute");
                if (a != null) return a.Arguments[0].ConstantString;
                return null;
            }
        }
    }

    abstract class EventReflector: Uno.UX.Markup.Reflection.IEvent
    {
        public abstract string Name { get; }

        public abstract string DelegateName { get; }
    }

    sealed class RegularEventReflector: EventReflector, Uno.UX.Markup.Reflection.IRegularEvent
    {
        readonly Compiler.API.Domain.IL.Members.Event _e;

        public RegularEventReflector(Compiler.API.Domain.IL.Members.Event e)
        {
            _e = e;
        }

        public override string Name => _e.Name;

        public override string DelegateName => _e.ReturnType.ToString();
    }

    sealed class AttachedEventReflector: EventReflector, Uno.UX.Markup.Reflection.IAttachedEvent
    {
        readonly AttachedEvent _ae;
        readonly CompilerDataTypeProvider _c;

        public AttachedEventReflector(AttachedEvent ae, CompilerDataTypeProvider c)
        {
            _ae = ae;
            _c = c;
        }

        public string AddMethodName => _ae.AddMethod.Name;

        public string RemoveMethodName => _ae.RemoveMethod.Name;

        public IDataType DeclaringType => _c.TryGetTypeByName(_ae.AddMethod.DeclaringType.GetCachedFullName());

        public override string Name => _ae.Name;

        public override string DelegateName => _ae.DataType.ToString();
    }


    sealed class GlobalResourceReflector: Uno.UX.Markup.Reflection.IGlobalResource
    {
        readonly Uno.Compiler.API.Domain.IL.Members.Member _m;
        readonly CompilerDataTypeProvider _c;

        public GlobalResourceReflector(Uno.Compiler.API.Domain.IL.Members.Member m, CompilerDataTypeProvider c)
        {
            _m = m;
            _c = c;
        }

        public string FullPath => _m.DeclaringType.GetCachedFullName() + "." + _m.Name;

        public IDataType DataType => _c.TryGetTypeByName(_m.ReturnType.GetCachedFullName());

        public string GlobalResourceName
        {
            get
            {
                var gs = _m.Attributes.FirstOrDefault(x => x.ReturnType.GetCachedFullName() == "Uno.UX.UXGlobalResourceAttribute");
                if (gs == null) return null;

                if (gs.Arguments.Length == 0) return _m.Name;

                var symbol = gs.Arguments[0] as Uno.Compiler.API.Domain.IL.Expressions.Constant;
                if (symbol == null) return _m.Name;

                var sv = symbol.Value as string;
                if (sv != null) return sv;

                throw new Exception(_m.DeclaringType + "." + _m.Name + ": GlobalResource argument must be a constant string");
            }
        }

        public string GlobalSymbol
        {
            get
            {
                return GlobalResourceName ?? FullPath;
            }
        }
    }

    class DataTypeReflector: Uno.UX.Markup.Reflection.IDataType
    {
        public override string ToString()
        {
            return UnoDataType.QualifiedName + " (reflector)";
        }

        public bool IsGlobalModule => UnoDataType.Attributes
            .Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXGlobalModuleAttribute");

        public bool IsFreestanding => UnoDataType.Attributes
            .Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXFreestandingAttribute");

        public string UXFunctionName => UnoDataType.Attributes
            .Where(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXFunctionAttribute")
            .Select(x => x.Arguments[0].ConstantString)
            .FirstOrDefault();

        public string UXUnaryOperatorName => UnoDataType.Attributes
            .Where(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXUnaryOperatorAttribute")
            .Select(x => x.Arguments[0].ConstantString)
            .FirstOrDefault();

        public IEnumerable<string> UXTestBootstrapperFor
        {
            get
            {
                return UnoDataType.Attributes
                       .Where(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXTestBootstrapperForAttribute")
                       .Select(x => x.Arguments[0].ConstantString);
            }
        }

        public IDataType UXTestBootstrapper
        {
            get
            {
                var t = UnoDataType;
                while (t != null)
                {
                    var bootstrapper = _c.TestBootstrappers.FirstOrDefault(x => x.UXTestBootstrapperFor.Contains(t.GetCachedFullName()));
                    if (bootstrapper != null) return bootstrapper;
                    t = t.Base;
                }
                return null;
            }
        }

        readonly CompilerDataTypeProvider _c;

        public bool IsInnerClass => false;

        internal DataType UnoDataType { get; }

        public bool IsGenericParametrization => UnoDataType.IsGenericParameterization;

        public int GenericParameterCount => UnoDataType.GenericParameters != null ? UnoDataType.GenericParameters.Length : 0;

        readonly Constructor _ctor;

        public bool HasUXConstructor => _ctor != null;

        public AutoGenericInfo AutoGenericInfo { get; }

        internal DataTypeReflector(Uno.Compiler.API.Domain.IL.DataType dt, CompilerDataTypeProvider c, AutoGenericInfo agi)
        {
            _c = c;
            UnoDataType = dt;

            AutoGenericInfo = agi;

            foreach (var ctor in UnoDataType.Constructors)
            {
                if (ctor.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXConstructorAttribute"))
                {
                    _ctor = ctor;
                    break;
                }
            }
        }

        public string GetMissingPropertyHint(string propname)
        {
            var t = UnoDataType;
            while (t != null)
            {
                var r = t.Attributes
                .Where(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXMissingPropertyHintAttribute")
                .Select(x => x.Arguments)
                .Where(x => x[0].ConstantString == propname)
                .Select(x => x[1].ConstantString)
                .FirstOrDefault();
                if (r != null) return r;
                t = t.Base;
            }
            return null;
        }

        public IEnumerable<string> Metadata
        {
            get
            {
                var t = UnoDataType;
                while (t != null)
                {
                    var md = t.Fields.Where(p => p.Attributes.Any(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXMetadataAttribute")).Select(x => x.FullName);
                    if (md.Any()) return md;
                    t = t.Base;
                }
                return null;
            }
        }

        string _qualifiedNameCache;
        public string QualifiedName => _qualifiedNameCache ?? (_qualifiedNameCache = UnoDataType.QualifiedName);

        string _fullNameCache;
        public string FullName => _fullNameCache ?? (_fullNameCache = UnoDataType.GetCachedFullName());

        bool IsSutiableProperty(Compiler.API.Domain.IL.Members.Property prop)
        {
            if (!prop.IsPublic && !prop.IsProtected) return false;
            if (prop.GetMethod == null || !(prop.GetMethod.IsPublic || prop.GetMethod.IsProtected)) return false;
            if (_c.IsListType(prop.ReturnType)) return true;

            return true;
        }

        IEnumerable<Reflection.IProperty> GenerateProps()
        {
            var t = UnoDataType;

            if (_ctor != null)
            {
                foreach (var p in _ctor.Parameters)
                {
                    yield return new ConstructorArgumentReflector(_ctor, p, _c);
                }
            }

            while (t != null)
            {
                foreach (var p in t.Properties.Where(IsSutiableProperty))
                    yield return new PropertyReflector(p, _c);

                t = t.Base;
            }

            foreach (var p in _c.AttachedProperties.Where(x => UnoDataType.IsCompatibleWith(x.OwnerType)))
                yield return new AttachedPropertyReflector(p, _c);
        }

        Reflection.IProperty[] _props;
        public IEnumerable<Reflection.IProperty> Properties => _props ?? (_props = GenerateProps().ToArray());

        IEnumerable<GlobalResourceReflector> GenerateGlobalResources()
        {
            foreach (var f in UnoDataType.Fields.Where(x => x.IsStatic && x.IsPublic))
                yield return new GlobalResourceReflector(f, _c);

            foreach (var f in UnoDataType.Properties.Where(x => x.IsStatic && x.IsPublic))
                yield return new GlobalResourceReflector(f, _c);
        }

        Reflection.IGlobalResource[] _globalResources;

        public IEnumerable<Reflection.IGlobalResource> GlobalResources => _globalResources ?? (_globalResources = GenerateGlobalResources().Where(x => x.GlobalResourceName != null) .ToArray());

        IEnumerable<Reflection.IEvent> GenerateEvents()
        {
            var t = UnoDataType;

            while (t != null)
            {
                foreach (var e in t.Events)
                    yield return new RegularEventReflector(e);



                t = t.Base;
            }

            foreach (var e in _c.AttachedEvents.Where(x => UnoDataType.IsCompatibleWith(x.OwnerType)))
                yield return new AttachedEventReflector(e, _c);
        }

        Reflection.IEvent[] _events;
        public IEnumerable<Reflection.IEvent> Events => _events ?? (_events = GenerateEvents().ToArray());

        public ContentMode ContentMode
        {
            get
            {
                var t = UnoDataType;

                while (t != null)
                {
                    var attr = t.Attributes.FirstOrDefault(x =>
                        (x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXContentModeAttribute") &&
                        (x.Arguments[0].ActualValue is Constant));

                    if (attr != null)
                    {
                        var mode = (string)attr.Arguments[0].ActualValue.ConstantValue;

                        if (mode == "Template") return ContentMode.Template;
                        else if (mode == "TemplateIfClass") return ContentMode.TemplateIfClass;
                        else if (mode == "Default") return ContentMode.Default;
                        else throw new Exception("Unsupported UXContentMode: \"" + mode + "\". Allowed options are \"Template\" or \"Default\"");
                    }
                    t = t.Base;
                }

                return ContentMode.Default;
            }
        }

        public bool Implements(IDataType baseType)
        {
            var t = _c.TryGetUnoTypeByName(baseType.FullName);
            return t != null && UnoDataType.IsCompatibleWith(t);
        }

        public bool IsValueType => UnoDataType.IsValueType;

        public bool IsString => UnoDataType.QualifiedName == "Uno.String";

        public IDataType ActualIDataTypeImpl => this;


        public ValueBindingInfo ValueBindingInfo
        {
            get
            {
                var aliasAttr = UnoDataType.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXValueBindingAliasAttribute");
                if (aliasAttr == null) return null;

                var targetProp = UnoDataType.EnumerateMembersRecursive().FirstOrDefault(x => x.Attributes.Any(y => y.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXValueBindingTargetAttribute"));
                var argumentProp = UnoDataType.EnumerateMembersRecursive().FirstOrDefault(x => x.Attributes.Any(y => y.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXValueBindingArgumentAttribute"));

                return new ValueBindingInfo((string)aliasAttr.Arguments[0].ConstantValue, targetProp?.Name, argumentProp?.Name);
            }
        }


        public string ImplicitPropertySetter
        {
            get
            {
                var p = UnoDataType;

                while (p != null)
                {
                    var implSet = p.Attributes.FirstOrDefault(x => x.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXImplicitPropertySetterAttribute");

                    if (implSet == null)
                    {
                        p = p.Base;
                        continue;
                    }

                    if (implSet == null) return null;

                    if (!(implSet.Arguments[0] is Constant) || !(implSet.Arguments[0].ConstantValue is string))
                        throw new Exception("UXImplicitPropertySetterAttribute: argument must be a constant string");

                    return implSet.Arguments[0].ConstantValue as string;
                }

                return null;
            }
        }
    }

    sealed class EnumReflector: DataTypeReflector, Uno.UX.Markup.Reflection.IEnum
    {
        public EnumReflector(Uno.Compiler.API.Domain.IL.Types.EnumType dt, CompilerDataTypeProvider c): base(dt, c, null)
        {

        }

        public IEnumerable<Reflection.Literal> Literals
        {
            get { return UnoDataType.Literals.Select(x => new Reflection.Literal(x.Name, (int)(dynamic)x.Value)); }
        }

        public IEnumerable<string> MetaData => null;

        
    }

    public class CompilerDataTypeProvider : IDataTypeProvider
    {
        readonly AttachedMemberRegistry _apr;
        readonly Dictionary<DataType, IDataType> _unoTypeToType = new Dictionary<DataType, IDataType>();
        readonly Dictionary<string, IDataType> _nameToType = new Dictionary<string, IDataType>();
        readonly Dictionary<string, DataType> _nameToUnoType = new Dictionary<string, DataType>();

        readonly Dictionary<string, IDataType> _genericAliasToType = new Dictionary<string, IDataType>();
        readonly Dictionary<string, IDataType> _valueBindingAliasToType = new Dictionary<string, IDataType>();

        internal IEnumerable<AttachedProperty> AttachedProperties => _apr.AttachedProperties;

        internal IEnumerable<AttachedEvent> AttachedEvents => _apr.AttachedEvents;

        internal IDataType this[DataType type] => _unoTypeToType[type];

        public IEnumerable<IDataType> DataTypes => _unoTypeToType.Values;

        readonly Dictionary<DataType, bool> _isListType = new Dictionary<DataType, bool>();
        public bool IsListType(DataType dt)
        {
            bool res;
            if (!_isListType.TryGetValue(dt, out res))
            {
                res = dt.GetCachedFullName().StartsWith("Uno.Collections.IList");
                _isListType.Add(dt, res);
            }
            return res;
        }

        IDataType[] _testBootstrappers;
        public IEnumerable<IDataType> TestBootstrappers
        {
            get
            {
                if (_testBootstrappers == null)
                    _testBootstrappers = DataTypes.Where(t => t.UXTestBootstrapperFor.Count() > 0).ToArray();

                return _testBootstrappers;
            }
        }

        public IDataType GetAttachedPropertyTypeByName(string name)
        {
            if (name.Contains('.'))
            {
                var p = _apr.AttachedProperties.FirstOrDefault(x => x.Name == name);
                if (p != null) return _unoTypeToType[p.DataType];
            }
            else
            {
                var p = _apr.AttachedProperties.FirstOrDefault(x => x.Name.Contains('.') ? x.Name.Split('.')[1] == name : x.Name == name);
                if (p != null) return _unoTypeToType[p.DataType];
            }
            return null;
        }

        public IDataType TryGetTypeByName(string name)
        {
            IDataType dt;
            _nameToType.TryGetValue(name, out dt);
            return dt;
        }

        internal DataType TryGetUnoTypeByName(string name)
        {
            DataType dt;
            _nameToUnoType.TryGetValue(name, out dt);
            return dt;
        }

        public IDataType GetTypeByGenericAlias(string alias)
        {
            IDataType dt;
            _genericAliasToType.TryGetValue(alias, out dt);
            return dt;
        }
        public IDataType GetTypeByValueBindingAlias(string alias)
        {
            if (alias == null)
                alias = "Data";

            IDataType dt;
            _valueBindingAliasToType.TryGetValue(alias, out dt);
            return dt;
        }


        public CompilerDataTypeProvider(Compiler.API.ICompiler compiler)
        {
            _apr = new AttachedMemberRegistry();
            _apr.Populate(compiler.Utilities.FindAllTypes().SelectMany(x => x.Methods).ToArray());

            var dataTypes = compiler.Utilities.FindAllTypes();

            _apr.Populate(dataTypes.SelectMany(x => x.Methods).ToArray());

            foreach (var unoType in dataTypes)
            {
                var uxType = CreateType(unoType);
                _unoTypeToType.Add(unoType, uxType);

                var name = uxType.FullName;
                if (!_nameToType.ContainsKey(name))
                    _nameToType.Add(name, uxType);

                if (!_nameToUnoType.ContainsKey(name))
                    _nameToUnoType.Add(name, unoType);

                if (unoType.MasterDefinition == unoType)
                {
                    if (uxType.AutoGenericInfo != null)
                    {
                        if (_genericAliasToType.ContainsKey(uxType.AutoGenericInfo.Alias))
                        {
                            throw new Exception("Multiple definitions of generic alias " + uxType.AutoGenericInfo.Alias + " found: " +
                                uxType.FullName + " and " + _genericAliasToType[uxType.AutoGenericInfo.Alias].FullName);
                        }
                        _genericAliasToType.Add(uxType.AutoGenericInfo.Alias, uxType);
                    }
                        

                    if (uxType.ValueBindingInfo != null)
                        _valueBindingAliasToType.Add(uxType.ValueBindingInfo.Alias, uxType);
                }
            }


        }

        IDataType CreateType(DataType x)
        {
            var autoGeneric = x.Attributes.FirstOrDefault(a => a.Constructor.DeclaringType.GetCachedFullName() == "Uno.UX.UXAutoGenericAttribute");

            AutoGenericInfo agi = null;

            if (autoGeneric != null)
            {
                var aliasConst = autoGeneric.Arguments[0] as Constant;
                var alias = aliasConst?.Value as string;

                var propertyConst = autoGeneric.Arguments[1] as Constant;
                var property = propertyConst?.Value as string;

                agi = new AutoGenericInfo(alias, property);
            }

            var et = x as EnumType;
            if (et != null) return new EnumReflector(et, this);

            return new DataTypeReflector(x, this, agi);
        }


    }
}
