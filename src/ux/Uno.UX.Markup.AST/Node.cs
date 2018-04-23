using System.Collections.Generic;
using System.Linq;

namespace Uno.UX.Markup.AST
{
    public sealed class Property
    {
        public string Name { get; }
        public string Value { get; }
        public string Namespace { get; }

        public FileSourceInfo Source { get; }

        public Property(string name, string value, FileSourceInfo source, string ns = null)
        {
            Name = name;
            Value = value;
            Namespace = ns;

            Source = source;
        }
    }

    public abstract class Node
    {
        public string Namespace { get; }
        public string TypeName { get; }
        public string UXName { get; set; }
        public string UXValue { get; }

        public bool ExplicitAutoBindFalse { get; }
        public string Binding { get; }

        public Element Parent { get; internal set; }

        public FileSourceInfo Source { get; }

        protected internal Node(string ns, string typeName, string uxName, string uxValue, string binding, bool explicitAutoBindFalse, FileSourceInfo source)
        {
            Namespace = ns;
            TypeName = typeName;
            UXName = uxName;
            UXValue = uxValue;
            Binding = binding;
            ExplicitAutoBindFalse = explicitAutoBindFalse;

            Source = source;
        }

        public override string ToString()
        {
            return TypeName + (UXName != null ? " (" + UXName + ")" : "");
        }

        public virtual IEnumerable<Node> DescendantNodesAndSelf()
        {
            yield return this;
        }
    }

    public sealed class ReferenceNode: Node
    {
        public string RefId { get; }

        internal ReferenceNode(string ns, string typeName, string uxName, string binding, string refId, bool explicitAutoBindFalse, FileSourceInfo source)
            : base(ns, typeName, uxName, null, binding, explicitAutoBindFalse, source)
        {
            RefId = refId;
        }
    }

    public sealed class Text : Node
    {
        public Text(string value, FileSourceInfo source)
            : base(Configuration.DefaultNamespace, "string", null, value, null, false, source)
        {
        }
    }

    public enum ElementType
    {
        Object,
        Dependency,
        Template,
        Property
    }


    public sealed class Element: Node
    {
        public string UXCondition { get; }
        public string UXPath { get; }
        public string ClearColor { get; }
        public string UXKey { get; }
        public ElementType ElementType { get; }

        public bool IsProjectRoot => Parent == null;

        readonly List<Property> _props;

        readonly List<Node> _children = new List<Node>();

        public Element(string ns, string typeName, string uxCondition, string uxName, ElementType elmType, string uxKey, string binding, string uxPath, string uxValue, string clearColor, Property[] props, Generator generator, Node[] children, bool explicitAutoBindFalse, FileSourceInfo source)
            : base(ns, typeName, uxName, uxValue, binding, explicitAutoBindFalse, source)
        {
            UXCondition = uxCondition;
            UXKey = uxKey;
            UXPath = uxPath;
            ElementType = elmType;
            ClearColor = clearColor;
            _children.AddRange(children);
            foreach (var c in children) c.Parent = this;
            Generator = generator;
            _props = props != null ? new List<Property>(props) : null;
        }

        public IEnumerable<Property> Properties => _props;

        public void AddProperty(Property p)
        {
            _props.Add(p);
        }

        public void RemoveProperty(Property p)
        {
            _props.Remove(p);
        }

        public Generator Generator { get; }

        public void AddChild(Node node)
        {
            node.Parent = this;
            _children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            node.Parent = null;
            _children.Remove(node);
        }

        public IEnumerable<Node> Children => _children;

        public override IEnumerable<Node> DescendantNodesAndSelf()
        {
            foreach (var n in base.DescendantNodesAndSelf())
                yield return n;

            foreach (var c in Children)
            {
                foreach (var cc in c.DescendantNodesAndSelf())
                    yield return cc;
            }
        }
    }

   
}
