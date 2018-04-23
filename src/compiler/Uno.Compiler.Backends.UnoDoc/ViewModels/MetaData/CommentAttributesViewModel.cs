using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class CommentAttributesViewModel
    {
        public bool Advanced { get; }
        public string ScriptModule { get; }
        public ScriptMethodCommentViewModel ScriptMethod { get; }
        public string ScriptProperty { get; }
        public string ScriptEvent { get; }
        public ReturnCommentViewModel Returns { get; }
        public bool Published { get; }
        public List<ParameterCommentViewModel> Parameters { get; }
        public List<string> SeeAlso { get; }
        public string Topic { get; }
        public bool Experimental { get; }
        public bool Deprecated { get; }

        public CommentAttributesViewModel(bool advanced,
                                          string scriptModule,
                                          ScriptMethodCommentViewModel scriptMethod,
                                          string scriptProperty,
                                          string scriptEvent,
                                          ReturnCommentViewModel returns,
                                          bool published,
                                          List<ParameterCommentViewModel> parameters,
                                          List<string> seeAlso,
                                          string topic,
                                          bool experimental,
                                          bool deprecated)
        {
            Advanced = advanced;
            ScriptModule = scriptModule;
            ScriptMethod = scriptMethod;
            ScriptProperty = scriptProperty;
            ScriptEvent = scriptEvent;
            Returns = returns;
            Published = published;
            Parameters = parameters;
            SeeAlso = seeAlso;
            Topic = topic;
            Experimental = experimental;
            Deprecated = deprecated;
        }

        public bool ShouldSerialize()
        {
            return Advanced ||
                   !string.IsNullOrWhiteSpace(ScriptModule) ||
                   ScriptMethod != null ||
                   !string.IsNullOrWhiteSpace(ScriptProperty) ||
                   !string.IsNullOrWhiteSpace(ScriptEvent) ||
                   Returns != null ||
                   Published ||
                   (Parameters != null && Parameters.Count > 0) ||
                   (SeeAlso != null && SeeAlso.Count > 0) ||
                   !string.IsNullOrWhiteSpace(Topic) ||
                   Experimental ||
                   Deprecated;
        }

        public bool ShouldSerializeParameters()
        {
            return Parameters != null && Parameters.Count > 0;
        }

        public bool ShouldSerializeSeeAlso()
        {
            return SeeAlso != null && SeeAlso.Count > 0;
        }

        public class ParameterCommentViewModel
        {
            public string Name { get; }
            public string TypeHint { get; }
            public string Description { get; }

            public ParameterCommentViewModel(string name, string typeHint, string description)
            {
                Name = name;
                TypeHint = typeHint;
                Description = description;
            }
        }

        public class ScriptMethodCommentViewModel
        {
            public string Name { get; }
            public List<string> Parameters { get; }

            public ScriptMethodCommentViewModel(string name, List<string> parameters)
            {
                Name = name;
                Parameters = parameters;
            }
        }

        public class ReturnCommentViewModel
        {
            public string TypeHint { get; }
            public string Text { get; }

            public ReturnCommentViewModel(string typeHint, string text)
            {
                TypeHint = typeHint;
                Text = text;
            }
        }
    }
}