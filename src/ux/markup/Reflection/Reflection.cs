using System.Collections.Generic;

namespace Uno.UX.Markup.Reflection
{
    public enum AutoBindingType
    {
        Primary,
        Components,
        Content,
        LineNumber,
        SourceFileName,
        None,
    }

    public enum PropertyType
    {
        Atomic,
        Reference,
        Delegate,
        List
    }

    public enum IdentifierScope
    {
        Data,
        Globals,
        Names
    }

    public interface IProperty
    {
        bool IsUXNameProperty { get; }
        bool IsUXFileNameProperty { get; }
        bool IsUXAutoNameTableProperty { get; }
        bool IsUXAutoClassNameProperty { get; }
        bool IsActualDataTypeAvailable { get; }
        bool IsOfGenericArgumentType { get; }
        bool IsUXVerbatim { get; }
        string UXAuxNameTable { get; }
        IdentifierScope UXIdentifierScope { get; }
        int UXArgumentIndex { get; }
        IDataType ListItemType { get; }
        IDataType DeclaringType { get; }
        string Name { get; }
        IDataType DataType { get; }
        bool Accepts(IDataType type);
        PropertyType PropertyType { get; }
        AutoBindingType AutoBindingType { get; }
        bool IsConstructorArgument { get; }
    }

    public interface IMutableProperty: IProperty
    {
        string OriginSetterName { get; }

        bool CanGet { get; }
        bool CanSet { get; }
    }

    public interface IConstructorArgument: IProperty
    {
        string DefaultValue { get; }
    }

    public interface IAttachedProperty: IMutableProperty
    {
        string SetMethodName { get; }
        string GetMethodName { get; }
        IDataType OwnerType { get; }
    }


    public interface IEvent
    {
        string Name { get; }
        string DelegateName { get; }
    }

    public interface IAttachedEvent: IEvent
    {
        IDataType DeclaringType { get; }
        string AddMethodName { get; }
        string RemoveMethodName { get; }
    }

    public interface IRegularEvent: IEvent
    {

    }


    public class AutoGenericInfo
    {
        public string Alias { get; private set; }
        public string ArgumentProperty { get; private set; }

        public AutoGenericInfo(string alias, string argumentProperty)
        {
            Alias = alias;
            ArgumentProperty = argumentProperty;
        }
    }

    public class ValueBindingInfo
    {
        public string Alias { get; private set; }
        public string TargetProperty { get; private set; }
        public string ArgumentProperty { get; private set; }

        public ValueBindingInfo(string alias, string target, string argument)
        {
            Alias = alias;
            TargetProperty = target;
            ArgumentProperty = argument;
        }
    }

    public interface IDataTypeProvider
    {
        IDataType TryGetTypeByName(string name);
        IDataType GetTypeByGenericAlias(string alias);
        IDataType GetTypeByValueBindingAlias(string alias);
        IEnumerable<IDataType> DataTypes { get; }
        IDataType GetAttachedPropertyTypeByName(string name);
    }

    public interface IGlobalResource
    {
        string FullPath { get; }
        IDataType DataType { get; }
        string GlobalSymbol { get; }
    }

    public interface IDataType
    {
        bool IsFreestanding { get; }
        bool IsGenericParametrization { get; }
        int GenericParameterCount { get; }
        bool IsInnerClass { get; }
        IDataType ActualIDataTypeImpl { get; }
        AutoGenericInfo AutoGenericInfo { get; }
        ValueBindingInfo ValueBindingInfo { get; }
        bool HasUXConstructor { get; }
        bool IsValueType { get; }
        bool IsString { get; }
        string QualifiedName { get; }
        string FullName { get; }
        string ImplicitPropertySetter { get; }
        string UXFunctionName { get; }
        string UXUnaryOperatorName { get; }
        IEnumerable<string> UXTestBootstrapperFor { get;  }
        IDataType UXTestBootstrapper { get; }
        IEnumerable<string> Metadata { get; }
        IEnumerable<IProperty> Properties { get; }
        IEnumerable<IEvent> Events { get; }
        IEnumerable<IGlobalResource> GlobalResources { get; }
        ContentMode ContentMode { get; }
        bool IsGlobalModule { get; }
        bool Implements(IDataType interfaceType);
        string GetMissingPropertyHint(string propName);
    }

    public class Literal
    {
        public readonly string Name;
        public readonly int Value;

        public Literal(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public interface IEnum: IDataType
    {
        IEnumerable<Literal> Literals { get; }
    }

}
