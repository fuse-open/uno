using Uno.Compiler.Backends.UnoDoc.Builders;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class CommentViewModel : CommentViewModelBase
    {
        public string Brief { get; }
        public string Full { get; }
        public string Remarks { get; }
        public string Examples { get; }
        public string Ux { get; }
        public CommentAttributesViewModel Attributes { get; }

        public CommentViewModel(SourceComment sourceComment)
        {
            Brief = string.IsNullOrWhiteSpace(sourceComment?.Brief) ? null : sourceComment.Brief;
            Full = string.IsNullOrWhiteSpace(sourceComment?.Full) ? null : sourceComment.Full;
            Remarks = string.IsNullOrWhiteSpace(sourceComment?.Remarks) ? null : sourceComment.Remarks;
            Examples = string.IsNullOrWhiteSpace(sourceComment?.Examples) ? null : sourceComment.Examples;
            Ux = string.IsNullOrWhiteSpace(sourceComment?.Ux) ? null : sourceComment.Ux;
            Attributes = ParseAttributes(sourceComment);
        }

        public bool ShouldSerialize()
        {
            return !string.IsNullOrWhiteSpace(Brief) ||
                   !string.IsNullOrWhiteSpace(Full) ||
                   !string.IsNullOrWhiteSpace(Remarks) ||
                   !string.IsNullOrWhiteSpace(Examples) ||
                   !string.IsNullOrWhiteSpace(Ux) ||
                   ShouldSerializeAttributes();
        }

        public bool ShouldSerializeAttributes() => Attributes != null && Attributes.ShouldSerialize();

        public BasicCommentViewModel ToBasicComment()
        {
            var attrs = Attributes;
            attrs.SeeAlso.Clear(); // Don't include "See Also" in these comments as it can cause circular references
            return new BasicCommentViewModel(Brief, Full, attrs);
        }
    }
}
