using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class MemberViewModel : DocumentViewModel, IReturnEnabledViewModel, IParameterEnabledViewModel
    {
        public LocationViewModel Location { get; }
        public ParametersViewModel Parameters { get; }
        public ReturnsViewModel Returns { get; }
        public UxMemberPropertiesViewModel UxProperties { get; }
        public ValuesViewModel Values { get; }
        public MemberFlagsViewModel Flags { get; }
        public AttributesViewModel Attributes { get; }

        public MemberViewModel(DocumentIdViewModel id,
                               DocumentUriViewModel uri,
                               TitlesViewModel titles,
                               SyntaxViewModel syntax,
                               LocationViewModel location,
                               DocumentReferenceViewModel declaredIn,
                               ParametersViewModel parameters,
                               ReturnsViewModel returns,
                               UxMemberPropertiesViewModel uxProperties,
                               ValuesViewModel values,
                               MemberFlagsViewModel flags,
                               CommentViewModel comment,
                               AttributesViewModel attributes,
                               IEntity underlyingEntity)
                : base(id, uri, titles, syntax, comment, declaredIn, underlyingEntity)
        {
            Location = location;
            Parameters = parameters;
            Returns = returns;
            UxProperties = uxProperties;
            Flags = flags;
            Values = values;
            Attributes = attributes;
        }

        public bool ShouldSerializeFlags()
        {
            return Flags != null && Flags.ShouldSerialize();
        }

        public bool ShouldSerializeAttributes()
        {
            return Attributes != null && Attributes.Count > 0;
        }
    }
}
