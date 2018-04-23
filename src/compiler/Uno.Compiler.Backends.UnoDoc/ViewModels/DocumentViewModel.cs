using Newtonsoft.Json;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels
{
    public abstract class DocumentViewModel
    {
        public DocumentIdViewModel Id { get; }
        public DocumentUriViewModel Uri { get; }
        public TitlesViewModel Titles { get; }
        public SyntaxViewModel Syntax { get; }
        public CommentViewModel Comment { get; }

        [JsonIgnore]
        public DocumentReferenceViewModel Reference { get; }

        [JsonIgnore]
        public DocumentReferenceViewModel DeclaredIn { get; }

        [JsonIgnore]
        public IEntity UnderlyingEntity { get; }

        protected DocumentViewModel(DocumentIdViewModel id,
                                    DocumentUriViewModel uri,
                                    TitlesViewModel titles,
                                    SyntaxViewModel syntax,
                                    CommentViewModel comment,
                                    DocumentReferenceViewModel declaredIn,
                                    IEntity underlyingEntity)
        {
            Id = id;
            Uri = uri;
            Titles = titles;
            Syntax = syntax;
            Comment = comment;
            Reference = new DocumentReferenceViewModel(id, uri, titles);
            DeclaredIn = declaredIn;
            UnderlyingEntity = underlyingEntity;
        }

        public bool ShouldSerializeSyntax() => Syntax != null && Syntax.ShouldSerialize();
        public bool ShouldSerializeComment() => Comment != null && Comment.ShouldSerialize();
    }
}
