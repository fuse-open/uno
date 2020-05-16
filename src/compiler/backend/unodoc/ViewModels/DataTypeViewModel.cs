using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class DataTypeViewModel : DocumentViewModel, IReturnEnabledViewModel, IParameterEnabledViewModel
    {
        public DocumentReferenceViewModel Base { get; }
        public LocationViewModel Location { get; }
        public InheritanceViewModel Inheritance { get; }
        public ParametersViewModel Parameters { get; }
        public ReturnsViewModel Returns { get; }
        public UxClassPropertiesViewModel UxProperties { get; }
        public ValuesViewModel Values { get; }
        public ImplementedInterfacesViewModel ImplementedInterfaces { get; }
        public AttributesViewModel Attributes { get; }

        public DataTypeViewModel(DocumentIdViewModel id,
                                 DocumentUriViewModel uri,
                                 TitlesViewModel titlesViewModel,
                                 SyntaxViewModel syntaxViewModel,
                                 DocumentReferenceViewModel @base,
                                 LocationViewModel location,
                                 InheritanceViewModel inheritance,
                                 ParametersViewModel parameters,
                                 ReturnsViewModel returns,
                                 UxClassPropertiesViewModel uxProperties,
                                 ValuesViewModel values,
                                 CommentViewModel comment,
                                 DocumentReferenceViewModel declaredIn,
                                 ImplementedInterfacesViewModel implementedInterfaces,
                                 AttributesViewModel attributes,
                                 IEntity underlyingEntity)
                : base(id, uri, titlesViewModel, syntaxViewModel, comment, declaredIn, underlyingEntity)
        {
            Base = @base;
            Location = location;
            Inheritance = inheritance;
            Parameters = parameters;
            Returns = returns;
            UxProperties = uxProperties;
            Values = values;
            ImplementedInterfaces = implementedInterfaces;
            Attributes = attributes;
        }

        public bool ShouldSerializeBaseDataType()
        {
            return Base != null;
        }

        public bool ShouldSerializeParameters()
        {
            return Parameters != null && Parameters.Count > 0;
        }

        public bool ShouldSerializeReturns()
        {
            return Returns != null;
        }

        public bool ShouldSerializeUxProperties()
        {
            return UxProperties != null;
        }

        public bool ShouldSerializeValues()
        {
            return Values != null && Values.Count > 0;
        }

        public bool ShouldSerializeImplementedInterfaces()
        {
            return ImplementedInterfaces != null && ImplementedInterfaces.Count > 0;
        }

        public bool ShouldSerializeAttributes()
        {
            return Attributes != null && Attributes.Count > 0;
        }
    }
}
