using System;
using System.Collections.Generic;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    class DeferredGenericType : IDataType
    {
        public string ParameterProperty { get; }

        IDataType _resolvedGenericArgument;
        public void SetResolvedGenericArgument(IDataType args)
        {
            _resolvedGenericArgument = args;
        }

        public string GetMissingPropertyHint(string propname)
        {
            return ActualIDataTypeImpl.GetMissingPropertyHint(propname);
        }

        public IDataType UXTestBootstrapper => _resolvedGenericArgument.UXTestBootstrapper;
        public IEnumerable<string> UXTestBootstrapperFor => _resolvedGenericArgument.UXTestBootstrapperFor;

        public bool IsGlobalModule => false;

        public bool IsGenericParametrization => true;

        public int GenericParameterCount => ActualIDataTypeImpl.GenericParameterCount;

        public bool IsInnerClass => false;

        public string UXFunctionName
        {
            get
            {
                return _resolvedGenericArgument.UXFunctionName;
            }
        }

        public DeferredGenericType(IDataType genericType, string parameterProperty)
        {
            ActualIDataTypeImpl = genericType;
            ParameterProperty = parameterProperty;
        }

        public override int GetHashCode()
        {
            return ParameterProperty.GetHashCode();
        }

        public IEnumerable<string> Metadata => ActualIDataTypeImpl.Metadata;

        public override bool Equals(object obj)
        {
            var x = (DeferredGenericType)obj;
            if (x == null) return false;

            return ActualIDataTypeImpl == x.ActualIDataTypeImpl && ParameterProperty == x.ParameterProperty;
        }

        public AutoGenericInfo AutoGenericInfo => ActualIDataTypeImpl.AutoGenericInfo;

        public bool HasUXConstructor => ActualIDataTypeImpl.HasUXConstructor;

        public bool IsValueType => ActualIDataTypeImpl.IsValueType;

        public bool IsFreestanding => ActualIDataTypeImpl.IsFreestanding;

        public string QualifiedName
        {
            get
            {
                if (_resolvedGenericArgument == null)
                    throw new Exception("Cannot query generic type for QualifiedName at this point - parameter not resolved yet");

                return ActualIDataTypeImpl.QualifiedName + "<" + _resolvedGenericArgument.FullName + ">";
            }
        }

        public string FullName
        {
            get
            {
                if (_resolvedGenericArgument == null)
                    throw new Exception("Cannot query generic type for FullName at this point - parameter not resolved yet");

                return ActualIDataTypeImpl.FullName + "<" + _resolvedGenericArgument.FullName + ">";
            }
        }

        public IEnumerable<IProperty> Properties
        {
            get
            {
                foreach (var k in ActualIDataTypeImpl.Properties)
                {
                    if (k.IsOfGenericArgumentType)
                    {
                        if (k is IConstructorArgument)
                        {
                            yield return new ResolvedGenericConstructorArgument((IConstructorArgument)k, () => _resolvedGenericArgument);
                        }
                        else if (k is IMutableProperty)
                        {
                            yield return new ResolvedGenericMutableProperty((IMutableProperty)k, () => _resolvedGenericArgument);
                        }
                    }
                    else
                    {
                        yield return k;
                    }
                }
            }
        }

        public IEnumerable<IEvent> Events => ActualIDataTypeImpl.Events;

        public IEnumerable<IGlobalResource> GlobalResources => ActualIDataTypeImpl.GlobalResources;

        public ContentMode ContentMode => ActualIDataTypeImpl.ContentMode;

        public bool Implements(IDataType interfaceType)
        {
            return ActualIDataTypeImpl.Implements(interfaceType);
        }

        public bool IsString => false;

        public IDataType ActualIDataTypeImpl { get; }

        public ValueBindingInfo ValueBindingInfo => ActualIDataTypeImpl.ValueBindingInfo;

        public string ImplicitPropertySetter => ActualIDataTypeImpl.ImplicitPropertySetter;

        public string UXUnaryOperatorName => ActualIDataTypeImpl.UXUnaryOperatorName;
    }

}
