using System;
using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.UXIL;

namespace Uno.UX.Markup.CodeGeneration
{
    class Scope
    {
        public UXIL.DocumentScope DocumentScope { get; }

        readonly List<UXIL.UXPropertySource> _uxPropertySources = new List<UXIL.UXPropertySource>();
        public string Register(UXIL.UXPropertySource ps)
        {
            if (!_uxPropertySources.Any(x => x.Node == ps.Node && x.Property.Property == ps.Property.Property))
            {
                _uxPropertySources.Add(ps);
            }

            return PropertySourceIdentifier(ps);
        }

        readonly Dictionary<string, int> _selectors = new Dictionary<string, int>();
        public IEnumerable<KeyValuePair<string,int>> Selectors {  get { return _selectors; } }
        public string Register(Selector sel)
        {
            if (!_selectors.ContainsKey(sel.Value))
                _selectors.Add(sel.Value, _selectors.Count);

            return SelectorToName(sel.Value);
        }

        public string SelectorToName(string s)
        {
            return "__selector" + _selectors[s];
        }

        public string PropertySourceIdentifier(UXIL.UXPropertySource uxps)
        {
            return GetUniqueIdentifier(uxps.Node) + "_" + uxps.Property.Property.Facet.Name.Replace('.', '_') + "_inst";
        }

        public IEnumerable<UXIL.UXPropertySource> UXNodeProperties => _uxPropertySources;

        class Identifiers
        {
            HashSet<string> _usedNames = new HashSet<string>();
            Dictionary<Node, string> _identifiers = new Dictionary<Node, string>();
            
            public void Add(Node n, string identifier)
            {
                _usedNames.Add(identifier);
                _identifiers.Add(n, identifier);
            }
            public bool ContainsNode(Node n)
            {
                return _identifiers.ContainsKey(n);
            }
            public string Get(Node n)
            {
                return _identifiers[n];
            }
            public bool ContainsIdentifier(string id)
            {
                return _usedNames.Contains(id);
            }
        }

        readonly Identifiers _identifiers = new Identifiers();

        readonly Dictionary<UXIL.Node, string> _templateIdentifiers = new Dictionary<Node, string>();

        static readonly string[] reservedWords = {
                                     "this", "__self"
                                 };

        public string Self
        {
            get
            {
                if (DocumentScope is UXIL.ClassNode)
                {
                    return "this";
                }
                else if (DocumentScope is UXIL.TemplateNode)
                {
                    return "__self";
                }
                else throw new Exception();
            }
        }

        public TypeNameHelper GetUniqueTemplateName(UXIL.TemplateNode templateScope)
        {
            return new TypeNameHelper(GetUniqueNodeName(templateScope, "Template"));
        }

        string GetUniqueNodeName(UXIL.Node template, string baseId)
        {
            var id = baseId;

            var c = 0;

            while (true)
            {
                if (IsIdentifierUnused(id))
                {
                    _templateIdentifiers.Add(template, id);
                    return id;
                }

                c++;
                id = baseId + c;
            }
        }

        bool IsIdentifierUnused(string id)
        {
            bool unusedLocally = 
                _templateIdentifiers.Values.All(x => x != id) &&
                !reservedWords.Contains(id) &&
                (id != DocumentScope.GeneratedClassName.Surname);

            if (unusedLocally)
            {
                if (ParentScope != null) return ParentScope.IsIdentifierUnused(id);
            }

            return unusedLocally;
        }

        public string GetUniqueIdentifier(UXIL.Node n, Node pathOrigin = null)
        {
            if (n == DocumentScope) return Self;

            var on = n as PropertyNode;
            if (@on?.Name != null)
            {
                return on.Name;
            }

            if (n is UXIL.ResourceRefNode)
            {
                return ((UXIL.ResourceRefNode)n).StaticRefId;
            }

            if (_identifiers.ContainsNode(n))
            {
                if (pathOrigin != null)
                {
                    return GetUnique(n, n.Name.ToIdentifier(), pathOrigin);
                }

                return _identifiers.Get(n);
            }

            if (n.Name == null)
            {
                return GetUnique(n, "temp", null);
            }
            else
            {
                return GetUnique(n, n.Name.ToIdentifier(), pathOrigin);
            }
        }

        string GetUnique(UXIL.Node n, string baseId, Node pathOrigin)
        {
            var id = baseId;

            if ((n.InstanceType == InstanceType.Local || n.InstanceType == InstanceType.None) && !DocumentScope.ContainsNode(n) && (pathOrigin != null))
            {
	            var ps = pathOrigin.ParentScope;

                if (n == ps && ps is TemplateNode)
                    return "__parentInstance";

                var prefix = "__parent";

                while (ps != null)
                {
                    if (n == ps) return prefix;

		            if (ps.ContainsNode(n))
					   return prefix + "." + id;

                    ps = ps.ParentScope;
                    prefix = prefix + ".__parent";
                }
                
                throw new Exception(id + " cannot be accessed from this scope");
            }

             var c = 0;

            while (true)
            {
                if (!_identifiers.ContainsIdentifier(id) &&
                    !reservedWords.Contains(id) &&
                    (id != DocumentScope.GeneratedClassName.Surname))
                {
                    
                    if (_identifiers.ContainsNode(n))
                        return _identifiers.Get(n);
                       
                    _identifiers.Add(n, id);

                    return id;
                }

                c++;
                id = baseId + c;
            }
        }

        public Scope ParentScope { get; }

        internal Scope(UXIL.DocumentScope docScope, Scope parentScope)
        {
            DocumentScope = docScope;
            ParentScope = parentScope;
        }
    }
}
