using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.Backends.UnoDoc.Builders;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public abstract class CommentViewModelBase
    {
        protected CommentAttributesViewModel ParseAttributes(SourceComment sourceComment)
        {
            if (sourceComment == null)
            {
                return null;
            }

            var parameters = (sourceComment.Attributes.Params ?? new List<SourceComment.CommentAttributeParameter>())
                    .Select(e =>
                    {
                        var result = ParseCommentWithTypeHint(e.Description);
                        return new CommentAttributesViewModel.ParameterCommentViewModel(e.Name, result.Item1, result.Item2);
                    })
                    .ToList();

            var scriptMethod = sourceComment.Attributes.ScriptMethod == null
                                       ? null
                                       : new CommentAttributesViewModel.ScriptMethodCommentViewModel(sourceComment.Attributes.ScriptMethod.Name,
                                                                                                     sourceComment.Attributes.ScriptMethod.Parameters);

            CommentAttributesViewModel.ReturnCommentViewModel returns = null;
            if (!string.IsNullOrWhiteSpace(sourceComment.Attributes.Returns))
            {
                var result = ParseCommentWithTypeHint(sourceComment.Attributes.Returns);
                returns = new CommentAttributesViewModel.ReturnCommentViewModel(result.Item1, result.Item2);
            }

            var seeAlso = new List<string>();
            if (sourceComment.Attributes.SeeAlso != null && sourceComment.Attributes.SeeAlso.Count > 0)
            {
                seeAlso.AddRange(sourceComment.Attributes.SeeAlso);
            }

            return new CommentAttributesViewModel(sourceComment.Attributes.Advanced,
                                                  sourceComment.Attributes.ScriptModule,
                                                  scriptMethod,
                                                  sourceComment.Attributes.ScriptProperty,
                                                  sourceComment.Attributes.ScriptEvent,
                                                  returns,
                                                  sourceComment.Attributes.Published,
                                                  parameters,
                                                  seeAlso,
                                                  sourceComment.Attributes.Topic,
                                                  sourceComment.Attributes.Experimental,
                                                  sourceComment.Attributes.Deprecated);
        }

        private static Tuple<string, string> ParseCommentWithTypeHint(string comment)
        {
            string typeHint = null;

            var trimmed = comment.TrimStart();
            if (trimmed.StartsWith("(") && trimmed.Contains(")"))
            {
                typeHint = trimmed.Substring(1, trimmed.IndexOf(")", StringComparison.Ordinal) - 1);
                trimmed = trimmed.Substring(typeHint.Length + 2).TrimStart();
            }

            return new Tuple<string, string>(typeHint, trimmed);
        }
    }
}