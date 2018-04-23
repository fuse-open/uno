using Uno.Compiler.Backends.UnoDoc.Builders;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class MinimalCommentViewModel : CommentViewModelBase
    {
        public string Brief { get; }
        public string Full { get; }
        public CommentAttributesViewModel Attributes { get; }

        public MinimalCommentViewModel(SourceComment sourceComment)
        {
            Brief = string.IsNullOrWhiteSpace(sourceComment?.Brief) ? null : sourceComment.Brief;
            Full = string.IsNullOrWhiteSpace(sourceComment?.Full) ? null : sourceComment.Full;
            Attributes = ParseAttributes(sourceComment);
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