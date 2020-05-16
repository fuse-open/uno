using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class SourceComment
    {
        public string Raw { get; }
        public string Brief { get; }
        public string Full { get; }
        public string Remarks { get; }
        public string Examples { get; }
        public string Ux { get; }
        public CommentAttributes Attributes { get; } = new CommentAttributes();

        public bool HasValue => !string.IsNullOrWhiteSpace(Raw) ||
                                !string.IsNullOrWhiteSpace(Brief) ||
                                !string.IsNullOrWhiteSpace(Full) ||
                                !string.IsNullOrWhiteSpace(Remarks) ||
                                !string.IsNullOrWhiteSpace(Examples) ||
                                !string.IsNullOrWhiteSpace(Ux) ||
                                Attributes.HasValue;

        public SourceComment() {}

        public SourceComment(string rawText,
                             string brief,
                             string full,
                             string remarks,
                             string examples,
                             string ux,
                             IList<Tuple<string, StringBuilder>> macros)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return;
            }

            Raw = rawText;
            Brief = brief;
            Full = full;
            Remarks = remarks;
            Examples = examples;
            Ux = ux;
            Attributes = ParseMacros(macros);
        }

        private static CommentAttributes ParseMacros(IList<Tuple<string, StringBuilder>> macros)
        {
            var macroNames = new HashSet<string>(macros.Select(e => e.Item1.ToLowerInvariant()));
            var macrosByKey = new Dictionary<string, List<string>>();
            foreach (var macro in macros)
            {
                var key = macro.Item1.ToLowerInvariant();
                if (!macrosByKey.ContainsKey(key))
                {
                    macrosByKey.Add(key, new List<string>());
                }

                macrosByKey[key].Add(macro.Item2.ToString().Replace("\r\n", "\n"));
            }

            var advanced = macroNames.Contains("@advanced");
            var scriptModule = GetMacroValue(macrosByKey, "@scriptmodule", true);
            var scriptProperty = GetMacroValue(macrosByKey, "@scriptproperty", true);
            var scriptEvent = GetMacroValue(macrosByKey, "@scriptevent", true);
            var returns = GetMacroValue(macrosByKey, "@return");
            var published = macroNames.Contains("@published");
            var uxProperty = macroNames.Contains("@ux-property");
            var topic = GetMacroValue(macrosByKey, "@topic");
            var deprecated = macroNames.Contains("@deprecated");
            var experimental = macroNames.Contains("@experimental");

            CommentAttributeScriptMethod scriptMethod = null;
            var scriptMethodValue = GetMacroValue(macrosByKey, "@scriptmethod");
            if (!string.IsNullOrWhiteSpace(scriptMethodValue))
            {
                if (scriptMethodValue.Contains("("))
                {
                    var name = scriptMethodValue.Substring(0, scriptMethodValue.IndexOf("("));
                    var paramValue = scriptMethodValue.Substring(scriptMethodValue.IndexOf("(") + 1);
                    if (paramValue.Contains(")"))
                    {
                        paramValue = paramValue.Substring(0, paramValue.IndexOf(")"));
                    }
                    scriptMethod = new CommentAttributeScriptMethod(name, paramValue.Trim().Split(',').Select(e => e.Trim()).ToList());
                }
                else
                {
                    scriptMethod = new CommentAttributeScriptMethod(scriptMethodValue, new List<string>());
                }
            }

            var paramValues = macrosByKey.ContainsKey("@param")
                                      ? macrosByKey["@param"]
                                      : new List<string>();
            var parameters = new List<CommentAttributeParameter>();
            foreach (var value in paramValues)
            {
                var paramName = value.Contains(" ")
                                        ? value.Substring(0, value.IndexOf(" ", StringComparison.Ordinal))
                                        : value;
                var paramDesc = value.Contains(" ")
                                        ? value.Substring(value.IndexOf(" ", StringComparison.Ordinal)).Trim()
                                        : null;
                parameters.Add(new CommentAttributeParameter(paramName, paramDesc));
            }

            var seeAlso = macrosByKey.ContainsKey("@seealso")
                                  ? macrosByKey["@seealso"]
                                  : new List<string>();
            if (macrosByKey.ContainsKey("@see"))
            {
                seeAlso.AddRange(macrosByKey["@see"]);
            }

            return new CommentAttributes(advanced,
                                         scriptModule,
                                         scriptMethod,
                                         scriptProperty,
                                         scriptEvent,
                                         returns,
                                         published,
                                         parameters,
                                         seeAlso,
                                         uxProperty,
                                         topic,
                                         deprecated,
                                         experimental);
        }

        private static string GetMacroValue(IDictionary<string, List<string>> macrosByKey, string key, bool requiresReturnValue = false)
        {
            var result = macrosByKey.ContainsKey(key)
                                 ? macrosByKey[key].Last() ?? ""
                                 : null;
            if (requiresReturnValue && string.IsNullOrWhiteSpace(result))
            {
                result = null;
            }

            return result;
        }

        public class CommentAttributes
        {
            public bool HasValue => Advanced ||
                                    !string.IsNullOrWhiteSpace(ScriptModule) ||
                                    ScriptMethod != null ||
                                    !string.IsNullOrWhiteSpace(ScriptProperty) ||
                                    !string.IsNullOrWhiteSpace(ScriptEvent) ||
                                    !string.IsNullOrWhiteSpace(Returns) ||
                                    Published ||
                                    (Params != null && Params.Count > 0) ||
                                    UxProperty ||
                                    !string.IsNullOrWhiteSpace(Topic) ||
                                    Deprecated ||
                                    Experimental;

            public bool Advanced { get; }
            public string ScriptModule { get; }
            public CommentAttributeScriptMethod ScriptMethod { get; }
            public string ScriptProperty { get; }
            public string ScriptEvent { get; }
            public string Returns { get; }
            public bool Published { get; }
            public List<CommentAttributeParameter> Params { get; }
            public List<string> SeeAlso { get; }
            public bool UxProperty { get; }
            public string Topic { get; }
            public bool Deprecated { get; }
            public bool Experimental { get; }

            public CommentAttributes() {}

            public CommentAttributes(bool advanced,
                                     string scriptModule,
                                     CommentAttributeScriptMethod scriptMethod,
                                     string scriptProperty,
                                     string scriptEvent,
                                     string returns,
                                     bool published,
                                     List<CommentAttributeParameter> parameters,
                                     List<string> seeAlso,
                                     bool uxProperty,
                                     string topic,
                                     bool deprecated,
                                     bool experimental)
            {
                Advanced = advanced;
                ScriptModule = scriptModule;
                ScriptMethod = scriptMethod;
                ScriptProperty = scriptProperty;
                ScriptEvent = scriptEvent;
                Returns = returns;
                Published = published;
                Params = parameters ?? new List<CommentAttributeParameter>();
                SeeAlso = seeAlso ?? new List<string>();
                UxProperty = uxProperty;
                Topic = topic;
                Deprecated = deprecated;
                Experimental = experimental;
            }
        }

        public class CommentAttributeParameter
        {
            public string Name { get; }
            public string Description { get; }

            public CommentAttributeParameter(string name, string description)
            {
                Name = name;
                Description = description;
            }
        }

        public class CommentAttributeScriptMethod
        {
            public string Name { get; }
            public List<string> Parameters { get; }

            public CommentAttributeScriptMethod(string name, List<string> parameters)
            {
                Name = name;
                Parameters = parameters;
            }
        }
    }
}
