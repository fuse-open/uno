using Uno.Compiler.Backends.UnoDoc.Builders;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class BasicCommentViewModel : CommentViewModelBase
    {
        public string Brief { get; }
        public string Full { get; }
        public CommentAttributesViewModel Attributes { get; }

        public BasicCommentViewModel(SourceComment sourceComment)
        {
            Brief = string.IsNullOrWhiteSpace(sourceComment?.Brief) ? null : sourceComment.Brief;
            Full = string.IsNullOrWhiteSpace(sourceComment?.Full) ? null : sourceComment.Full;
            Attributes = ParseAttributes(sourceComment);
        }

        internal BasicCommentViewModel(string brief, string full, CommentAttributesViewModel attributes)
        {
            Brief = brief;
            Full = full;
            Attributes = attributes;
        }

        public bool ShouldSerialize()
        {
            return !string.IsNullOrWhiteSpace(Brief) ||
                   !string.IsNullOrWhiteSpace(Full) ||
                   ShouldSerializeAttributes();
        }

        public bool ShouldSerializeAttributes() => Attributes != null && Attributes.ShouldSerialize();
    }
}