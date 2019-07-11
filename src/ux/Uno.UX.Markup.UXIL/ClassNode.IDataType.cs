using System;
using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public partial class ClassNode: IDataType
    {
        public bool IsGenericParametrization => false;

        public IEnumerable<string> Metadata => BaseType.Metadata;

        public IDataType ActualIDataTypeImpl => BaseType.ActualIDataTypeImpl;

        public IDataType UXTestBootstrapper => BaseType.UXTestBootstrapper;
        public IEnumerable<string> UXTestBootstrapperFor => BaseType.UXTestBootstrapperFor;

        public bool IsFreestanding => BaseType.IsFreestanding;

        public AutoGenericInfo AutoGenericInfo => BaseType.AutoGenericInfo;

        public string UXFunctionName => BaseType.UXFunctionName;

        public bool IsGlobalModule => false;

        public int GenericParameterCount => BaseType.GenericParameterCount;

        public ValueBindingInfo ValueBindingInfo => BaseType.ValueBindingInfo;

        public bool HasUXConstructor => false;

        public bool IsValueType => false;

        public bool IsString => false;

        public string QualifiedName
        {
            get
            {
                if (IsInnerClass) return ParentScope.GeneratedClassName.FullName + "." + GeneratedClassName.FullName;
                return GeneratedClassName.FullName;
            }
        }

        public string GetMissingPropertyHint(string propname)
        {
            return BaseType.GetMissingPropertyHint(propname);
        }

        public string FullName => GeneratedClassName.FullName;

        public override string ToString() => GeneratedClassName.FullName;

        public IEnumerable<IProperty> PrecompiledProperties => BaseType is ClassNode ? (BaseType as ClassNode).PrecompiledProperties : BaseType.Properties;

        IEnumerable<IProperty> IDataType.Properties =>
            DeclaredUXIProperties
            .Union(BaseType.Properties);

        List<IProperty> _declaredProps;

        IEnumerable<IProperty> DeclaredUXIProperties
        {
            get
            {
                if (_declaredProps == null)
                {
                    _declaredProps = new List<IProperty>();
                    foreach (var c in DeclaredUXProperties)
                    {
                        _declaredProps.Add(new DeclaredUXProperty(c, this));
                    }
					foreach (var c in DeclaredDependencies)
					{
						_declaredProps.Add(new DeclaredUXDependency(c, this));
					}
                }
                return _declaredProps;
            }
        }

        IEnumerable<IEvent> IDataType.Events => BaseType.Events;

        IEnumerable<IGlobalResource> IDataType.GlobalResources => new IGlobalResource[0];

        public ContentMode ContentMode => BaseType.ContentMode;

        public bool Implements(IDataType interfaceType)
        {
            return BaseType.Implements(interfaceType);
        }

        public string ImplicitPropertySetter => BaseType.ImplicitPropertySetter;

        public string UXUnaryOperatorName => null;
    }
}
