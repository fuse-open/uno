using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public abstract partial class DocumentScope : Node
    {
        public Vector<float> ClearColor { get; private set; }

        public TypeNameHelper GeneratedClassName { get; set; }

        internal DocumentScope(FileSourceInfo source, string name, TypeNameHelper generatedClassName, IDataType resultingType, Vector<float> clearColor, InstanceType instanceType, IEnumerable<RawProperty> rawProperties)
            : base(source, name, resultingType, instanceType, rawProperties)
        {
            GeneratedClassName = generatedClassName;
            ClearColor = clearColor;
        }

        public void VerifyNamesUnique(Compiler c)
        {
            var names = new Dictionary<string, Node>();
            VerifyNamesUnique(c, this, names);
        }

        public void VerifyNamesUnique(Compiler c, Node n, Dictionary<string, Node> names)
        {
            if (n.Name != null)
            {
                if (names.ContainsKey(n.Name))
                {
                    var other = names[n.Name];
                    var otherFile = other.Source.FileName;
                    var thisFile = n.Source.FileName;
                    var otherDef = (thisFile != otherFile ? " " + System.IO.Path.GetFileName(otherFile) : "") + " line " + other.Source.LineNumber;
                    var otherType = new TypeNameHelper(other.DeclaredType.FullName).Surname;
                    var docScopeDef = " " + System.IO.Path.GetFileName(Source.FileName) + " line " + Source.LineNumber;
                    var docScopeType = new TypeNameHelper(DeclaredType.FullName).Surname;
                    c.ReportError(n.Source, "Names must be unique within each document scope. In this scope '" + docScopeType + "' starting on" + docScopeDef + " the name '" + n.Name + "' is already used for " + otherType + " on" + otherDef);
                }
                else
                {
                    names.Add(n.Name, n);
                }
            }

            foreach (var ch in n.Children)
            {
                var ds = ch as DocumentScope;
                if (!(ch is DocumentScope)) VerifyNamesUnique(c, ch, names);
            }
        }

        public IEnumerable<DocumentScope> AllScopes
        {
            get
            {
                var scopes = new List<DocumentScope>();
                FindScopes(this, scopes);
                return scopes;
            }
        }
        static void FindScopes(Node n, List<DocumentScope> scopes)
        {
            if (n is DocumentScope) scopes.Add((DocumentScope)n);
            foreach (var c in n.Children) FindScopes(c, scopes);
        }

        static Node FindNodeRecursive(Node parent, string name)
        {
            if (parent.Name == name)
                return parent;

            foreach (var c in parent.Children)
            {
                if (!(c is DocumentScope))
                {
                    var n = FindNodeRecursive(c, name);
                    if (n != null)
                        return n;
                }
            }

            return null;
        }

        Node SearchNode(string name)
        {
            var n = FindNodeRecursive(this, name);
            if (n != null)
                return n;

            if (ParentScope != null && !(this is ClassNode && !((ClassNode)this).IsInnerClass))
                n = ParentScope.SearchNode(name);

            return n;
        }

        readonly Dictionary<string, Node> _nodeCache = new Dictionary<string, Node>();
        public Node FindNode(string name)
        {
            Node ret;
            if (_nodeCache.TryGetValue(name, out ret))
                return ret;

            ret = SearchNode(name);
            _nodeCache.Add(name, ret);
            return ret;
        }


        public IEnumerable<Node> NodesIncludingRoot
        {
            get
            {
                var nodes = new List<Node>();

                foreach (var c in Children)
                    FindNodes(c, nodes);

                // Note - 'this' must be last, otherwise fuselibs will break
                // Not really instantiation/rooting-order independent :(
                // https://github.com/fusetools/fuselibs-private/issues/422
                nodes.Add(this);
                return nodes;
            }
        }

        public IEnumerable<Node> NodesExcludingRoot
        {
            get
            {
                var nodes = new List<Node>();

                foreach (var c in Children)
                    FindNodes(c, nodes);
                return nodes;
            }
        }

        static void FindNodes(Node n, List<Node> result)
        {
            result.Add(n);
            if (n is DocumentScope) return;

            foreach (var c in n.Children) FindNodes(c, result);
        }

        public bool ContainsNode(Node n)
        {
            return NodesIncludingRoot.Any(x => x == n);
        }

        NameTableNode _nameTable;

        public NameTableNode GetNameTable(Compiler c)
        {
            if (_nameTable == null)
            {
                var pt = ParentScope?.GetNameTable(c);
                _nameTable = new NameTableNode(Source, c._uxProperties, this as ClassNode, pt, c.NameTableType, this, RawProperties);
                c.AddNode(new object(), _nameTable);
                AddChild(_nameTable);
                _nameTable.CreateMembers();
            }
            return _nameTable;
        }
    }
}
