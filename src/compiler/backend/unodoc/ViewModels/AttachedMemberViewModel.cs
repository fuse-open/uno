using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public class AttachedMemberViewModel : DocumentViewModel, IReturnEnabledViewModel, IParameterEnabledViewModel
    {
        public ReturnsViewModel Returns { get; private set; }
        public ValuesViewModel Values { get; private set; }
        public AttachedMemberSourceViewModel Source { get; private set; }
        public ParametersViewModel Parameters { get; private set; }

        public AttachedMemberViewModel(DocumentIdViewModel id,
                                       DocumentUriViewModel uri,
                                       TitlesViewModel titles,
                                       DocumentReferenceViewModel declaredIn,
                                       ParametersViewModel parameters,
                                       ReturnsViewModel returns,
                                       ValuesViewModel values,
                                       AttachedMemberSourceViewModel source,
                                       CommentViewModel comment,
                                       IEntity underlyingEntity)
                : base(id, uri, titles, null, comment, declaredIn, underlyingEntity)
        {
            Parameters = parameters;
            Returns = returns;
            Values = values;
            Source = source;
        }
    }
}
