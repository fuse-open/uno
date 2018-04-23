using System.Linq;
using Newtonsoft.Json;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class TableOfContentsEntryViewModel : DocumentReferenceViewModel
    {
        public BasicCommentViewModel Comment { get; }
        public ReturnsViewModel Returns { get; }
        public ParametersViewModel Parameters { get; }
        public MemberFlagsViewModel Flags { get; }

        [JsonIgnore]
        public DocumentReferenceViewModel DeclaredIn { get; }

        public TableOfContentsEntryViewModel(DocumentIdViewModel id,
                                             DocumentUriViewModel uri,
                                             IndexTitlesViewModel titles,
                                             BasicCommentViewModel comment,
                                             ReturnsViewModel returns,
                                             ParametersViewModel parameters,
                                             MemberFlagsViewModel flags,
                                             DocumentReferenceViewModel declaredIn)
                : base(id, uri, titles)
        {
            Comment = comment;
            Returns = returns;
            Parameters = parameters;
            Flags = flags;
            DeclaredIn = declaredIn;
        }

        public bool ShouldSerializeComment()
        {
            return Comment != null && Comment.ShouldSerialize();
        }

        public bool ShouldSerializeReturns()
        {
            return Returns != null;
        }

        public bool ShouldSerializeParameters()
        {
            return Parameters != null && Parameters.Any();
        }

        public bool ShouldSerializeFlags()
        {
            return Flags != null && Flags.ShouldSerialize();
        }

        public override string ToString()
        {
            return Titles?.IndexTitle ?? base.ToString();
        }
    }
}