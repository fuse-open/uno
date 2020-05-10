using System;
using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public sealed class RawProperty
    {
        public string Name { get; }
        public string Value { get; }
        public string Namespace { get; }

        public FileSourceInfo Source { get; }

        public RawProperty(string name, string value, FileSourceInfo source, string ns = null)
        {
            Name = name;
            Value = value;
            Namespace = ns;

            Source = source;
        }

        internal RawProperty(AST.Property astProp)
        {
            Name = astProp.Name;
            Value = astProp.Value;
            Namespace = astProp.Namespace;

            Source = astProp.Source;
        }
    }

    public abstract class Property
    {
        public IProperty Facet { get; }
        public abstract bool HasValue { get;  }

        internal Property(IProperty facet)
        {
            Facet = facet;
        }

        public override string ToString()
        {
            return Facet.Name;
        }
    }

    public sealed class AtomicProperty: Property
    {
        AtomicValue _value;
        public AtomicValue Value
        {
            get { return _value ?? FallbackValue; }
            set
            {
                _value = value;
            }
        }

        public override bool HasValue => _value != null;

        public AtomicValue FallbackValue { get; set; }

        internal AtomicProperty(IProperty facet): base(facet)
        {
        }
    }


    public sealed class DelegateProperty : Property
    {
        public string Method { get; set; }

        public override bool HasValue => Method != null;

        internal DelegateProperty(IProperty facet): base(facet)
        {

        }
    }

    public abstract class BindableProperty: Property
    {
        protected BindableProperty(IProperty facet) : base(facet) { }

        public abstract IDataType BindableType { get; }

        public abstract void Bind(Node source);
        public abstract void Bind(Node sourceNode, Property sourceProperty);
        public abstract UXPropertyAccessorSource Bind(Property property);
        public abstract void BindToBundleFile(string path);
    }

    public abstract class ValueSource
    {

    }

    public sealed class AtomicValueSource: ValueSource
    {
        public AtomicValue Value { get; private set; }
        public AtomicValueSource(AtomicValue value) { Value = value; }
    }

    public abstract class ReferenceSource : ValueSource
    {
        public virtual Node Node => null;
    }

    public sealed class NodeSource: ReferenceSource
    {
        public override Node Node => Source;
        public Node Source { get; }
        public NodeSource(Node src)
        {
            if (src == null) throw new Exception();
            Source = src;
        }
    }

    public sealed class BundleFileSource: ReferenceSource
    {
        public string Path { get; private set; }
        public BundleFileSource(string path)
        {
            Path = path;
        }
    }

    public sealed class UXPropertySource: ReferenceSource
    {
        public UXPropertyClass Property { get; private set; }
        public UXPropertySource(Node sourceNode, UXPropertyClass prop)
        {
            Node = sourceNode;
            Property = prop;
        }

        public override Node Node { get; }
    }

    public sealed class UXPropertyAccessorSource : ReferenceSource
    {
        public Property Property { get; private set; }
        public UXPropertyAccessorSource(Property prop)
        {
            Property = prop;
        }

        public override Node Node => null;

        public string ClassName =>
            Property.Facet.DeclaringType.FullName.Replace('.', '_').Replace('<', '_').Replace('>', '_') + "_" + Property.Facet.Name.Replace('.', '_');

        public string Singleton => ClassName + ".Singleton";

        public override bool Equals(object obj)
        {
            var uxpa = obj as UXPropertyAccessorSource;
            if (uxpa == null) return false;
            return ClassName == uxpa.ClassName;
        }

        public override int GetHashCode()
        {
            return ClassName.GetHashCode();
        }
    }


    public sealed class ReferenceProperty : BindableProperty
    {
        public ReferenceSource Source { get; private set; }

        public override void Bind(Node source)
        {
            Source = new NodeSource(source);
        }

        public override bool HasValue => Source != null;

        public override void BindToBundleFile(string path)
        {
            Source = new BundleFileSource(path);
        }

        public override void Bind(Node sourceNode, Property property)
        {
            Source = new UXPropertySource(sourceNode, new UXPropertyClass(property, sourceNode));
        }

        public override UXPropertyAccessorSource Bind(Property property)
        {
            var x = new UXPropertyAccessorSource(property);
            Source = x;
            return x;
        }

        internal ReferenceProperty(IProperty facet): base(facet)
        {
        }

        public override IDataType BindableType => Facet.DataType;
    }

    public sealed class ListProperty : BindableProperty
    {
        readonly List<ReferenceSource> _sources = new List<ReferenceSource>();

        public IEnumerable<ReferenceSource> Sources => _sources;

        public override bool HasValue => _sources.Count > 0;

        public override void Bind(Node source)
        {
            _sources.Add(new NodeSource(source));
        }

        internal ListProperty(IProperty facet): base(facet)
        {

        }

        public override void BindToBundleFile(string path)
        {
            _sources.Add(new BundleFileSource(path));
        }

        public override void Bind(Node sourceNode, Property sourceProperty)
        {
            _sources.Add(new UXPropertySource(sourceNode, new UXPropertyClass(sourceProperty, sourceNode)));
        }

        public override UXPropertyAccessorSource Bind(Property property)
        {
            var x = new UXPropertyAccessorSource(property);
            _sources.Add(x);
            return x;
        }

        public override IDataType BindableType => Facet.ListItemType;
    }

    public abstract class EventHandler
    {

    }

    public sealed class EventMethod: EventHandler
    {
        public string Name { get; private set; }
        public EventMethod(string name) { Name = name; }
    }

    public sealed class EventBinding: EventHandler
    {
        public Node Binding { get; private set; }
        public EventBinding(Node binding) { Binding = binding; }
    }

    public sealed class Event
    {
        public IEvent Facet { get; }

        public EventHandler Handler { get; set; }

        public Event(IEvent facet)
        {
            Facet = facet;
        }
    }


    public abstract class Node
    {
        public ClassNode ContainingClass => this is ClassNode ? this as ClassNode : Parent?.ContainingClass;

        public Node Root { get { if (Parent != null) return Parent.Root; return this; } }

        public string Path { get; set; }

        public string ContentString { get; set; }

        public IDataType ResultingType { get; }

        public IEnumerable<RawProperty> RawProperties { get; }

        AtomicProperty[] _atomicProperties;
        DelegateProperty[] _delegateProperties;
        ReferenceProperty[] _referenceProperties;
        ListProperty[] _listProperties;

        public IEnumerable<AtomicProperty> AtomicProperties => _atomicProperties;

        Event[] _events;

        BindableProperty[] _contentProps;
        internal IEnumerable<BindableProperty> ContentProperties => _contentProps ?? (_contentProps = BindableProperties.Where(x => x.Facet.AutoBindingType == AutoBindingType.Content).ToArray());

        BindableProperty[] _componentsProp;
        internal IEnumerable<BindableProperty> ComponentProperties => _componentsProp ?? (_componentsProp = BindableProperties.Where(x => x.Facet.AutoBindingType == AutoBindingType.Components).ToArray());


        BindableProperty[] _primaryProp;
        internal IEnumerable<BindableProperty> PrimaryProperties => _primaryProp ?? (_primaryProp = BindableProperties.Where(x => x.Facet.AutoBindingType == AutoBindingType.Primary).ToArray());

        internal IEnumerable<BindableProperty> BindableProperties
        {
            get
            {
                CreateMembers();
                return _referenceProperties.Cast<BindableProperty>().Concat(_listProperties.Cast<BindableProperty>());
            }
        }

        public IEnumerable<DelegateProperty> DelegatePropertiesWithValues
        {
            get { return _delegateProperties.Where(x => x.Method != null); }
        }

        public IEnumerable<DocumentScope> DocumentScopes
        {
            get
            {
                var list = new List<DocumentScope>();
                FindDocumentScopes(list);
                return list;
            }
        }

        void FindDocumentScopes(List<DocumentScope> list)
        {
            if (this is DocumentScope)
                list.Add(this as DocumentScope);

            foreach (var c in Children)
                c.FindDocumentScopes(list);
        }


        public IEnumerable<Property> Properties => _atomicProperties.Cast<Property>().Concat(
            _delegateProperties.Cast<Property>().Concat(
                _referenceProperties.Cast<Property>().Concat(
                    _listProperties.Cast<Property>())));


        public IEnumerable<AtomicProperty> MutableAtomicPropertiesWithValues { get { return _atomicProperties.Where(x => x.Value != null && x.Facet is IMutableProperty); } }
        public IEnumerable<ReferenceProperty> MutableReferencePropertiesWithValues { get { return _referenceProperties.Where(x => x.Source != null && x.Facet is IMutableProperty); } }
        public IEnumerable<AtomicProperty> AtomicPropertiesWithValues { get { return _atomicProperties.Where(x => x.HasValue); } }
        public IEnumerable<ReferenceProperty> ReferencePropertiesWithValues { get { return _referenceProperties.Where(x => x.Source != null); } }
        public IEnumerable<ListProperty> ListPropertiesWithValues { get { return _listProperties.Where(x => x.Sources.Any()); } }

        public IEnumerable<Event> EventsWithHandler { get { return _events.Where(x => x.Handler != null); } }

        public AtomicProperty NameProperty
        {
            get
            {
                return Properties.OfType<AtomicProperty>().FirstOrDefault(x => x.Facet.IsUXNameProperty);
            }
        }

        public AtomicProperty FileNameProperty
        {
            get
            {
                return Properties.OfType<AtomicProperty>().FirstOrDefault(x => x.Facet.IsUXFileNameProperty);
            }
        }

        public object TryFindPropertyOrEvent(FileSourceInfo src, Compiler c, string name)
        {
            var prop = Properties.FirstOrDefault(x => x.Facet.Name == name && (!(x.Facet is DeclaredUXDependency) || x.Facet.DeclaringType.QualifiedName == DeclaredType.QualifiedName));

            if (prop != null) return prop;

            var ev = _events.FirstOrDefault(x => x.Facet.Name == name);

            if (ev != null) return ev;

            // Check if there is a non-ambiguous attached property/event which match the
            // name if the qualifier is dropped
            var evq = EventsWithoutQualifier(name);
            var pq = PropertiesWithoutQualifier(name);

            if (evq.Count() + pq.Count() == 1)
            {
                if (evq.Count() == 1)
                {
                    return evq.First();
                }
                else
                {
                    return pq.First();
                }

            }
            else if (evq.Count() + pq.Count() > 1)
            {
                c.ReportError(src, "The name '" + name + "' is ambiguous here, please qualify one of : " + evq.Select(x => x.Facet.Name).Union(pq.Select(x => x.Facet.Name)).Aggregate((x, y) => x + ", " + y));
            }

            return null;
        }

        IEnumerable<Event> EventsWithoutQualifier(string name)
        {
            return _events.Where(x => {
                if (x.Facet.Name.Contains('.'))
                {
                    var t = new TypeNameHelper(x.Facet.Name).Surname;
                    return t == name;
                }
                return false;
            });
        }

        IEnumerable<Property> PropertiesWithoutQualifier(string name)
        {
            return Properties.Where(x =>
            {
                if (x.Facet.Name.Contains('.'))
                {
                    var t = new TypeNameHelper(x.Facet.Name).Surname;
                    return t == name;
                }
                return false;
            });
        }


        public BindableProperty TryFindBindableProperty(ClassNode currentClass, string name)
        {
            return BindableProperties.FirstOrDefault(x => x.Facet.Name == name && !(x.Facet is DeclaredUXDependency && x.Facet.DeclaringType == currentClass.BaseType)); 
        }

        public FileSourceInfo Source { get; }

        public string Name { get; set; }

        public InstanceType InstanceType { get; private set; }

        protected Node(FileSourceInfo source, string name, IDataType resultingType, InstanceType instanceType, IEnumerable<RawProperty> rawProperties = null)
        {
            Source = source;

            InstanceType = instanceType;

            Name = name;

            ResultingType = resultingType;

            RawProperties = rawProperties ?? new RawProperty[0];
        }

        public abstract IDataType MemberSource { get; }

        public void CreateMembers()
        {
            if (_atomicProperties == null)
            {
                var ms = MemberSource;
                _atomicProperties = ms.Properties.Where(x => x.PropertyType == PropertyType.Atomic).Select(x => new AtomicProperty(x)).ToArray();
                _delegateProperties = ms.Properties.Where(x => x.PropertyType == PropertyType.Delegate).Select(x => new DelegateProperty(x)).ToArray();
                _referenceProperties = ms.Properties.Where(x => x.PropertyType == PropertyType.Reference).Select(x => new ReferenceProperty(x)).ToArray();
                _listProperties = ms.Properties.Where(x => x.PropertyType == PropertyType.List).Select(x => new ListProperty(x)).ToArray();

                _events = ms.Events.Select(x => new Event(x)).ToArray();
            }
        }

        public override string ToString()
        {
            return ResultingType.FullName + (Name != null ? " (" + Name + ")" : "");
        }

        public abstract IDataType DeclaredType { get; }

        internal Node Parent { get; private set; }

        readonly List<Node> _children = new List<Node>();
        internal void AddChild(Node n)
        {
            _children.Add(n);
            n.Parent = this;
        }

        internal void RemoveChild(Node n)
        {
            _children.Remove(n);
            n.Parent = null;
        }

        public DocumentScope ParentScope
        {
            get
            {
                // Non-inner classes are roots!
                if (this is ClassNode && !((ClassNode)this).IsInnerClass)
                    return null;

                var s = this.Parent;
                
                while (s != null)
                {
                    if (s is DocumentScope) return (DocumentScope)s;
                    s = s.Parent;
                }
                return null;
            }
        }

        public DocumentScope Scope
        {
            get
            {
                var s = this;
                while (s != null)
                {
                    if (s is DocumentScope) return (DocumentScope)s;
                    s = s.Parent;
                }
                return null;
            }
        }

        public IEnumerable<Node> Children => _children;
    }

    public sealed class TemplateNode : DocumentScope
    {
        public string Case { get; set; }
        public bool IsDefaultCase { get; set; }

        public IDataType ProducedType { get; }

        internal TemplateNode(FileSourceInfo source, string name, string caseMatch, bool isDefaultCase, IDataType producedType, IDataType reflectedType, TypeNameHelper generatedClassName, Vector<float> clearColor, InstanceType instanceType, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, generatedClassName, reflectedType, clearColor, instanceType, rawProperties)
        {
            Case = caseMatch;
            IsDefaultCase = isDefaultCase;
            ProducedType = producedType;
        }

        public override IDataType MemberSource => ProducedType;

        public override IDataType DeclaredType => ProducedType;
    }

	public abstract class ParameterNode: Node
	{
		internal ParameterNode(FileSourceInfo source, string name, IDataType dataType, InstanceType instanceType, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, dataType, instanceType, rawProperties)
        {

		}

		public override IDataType MemberSource => ResultingType;
		public override IDataType DeclaredType => ResultingType;
	}

    public sealed class PropertyNode: ParameterNode
    {
        internal PropertyNode(FileSourceInfo source, string name, IDataType dataType, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, dataType, InstanceType.None, rawProperties)
        {
            
        }
    }

	public sealed class DependencyNode : ParameterNode
	{
		internal DependencyNode(FileSourceInfo source, string name, IDataType dataType, IEnumerable<RawProperty> rawProperties = null)
			: base(source, name, dataType, InstanceType.Local, rawProperties)
		{
		}
	}

	public abstract class ObjectNode : Node
    {
        public IDataType DataType { get; }

        public override IDataType MemberSource => DataType;


        internal ObjectNode(FileSourceInfo source, string name, IDataType dataType, InstanceType instanceType, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, dataType, instanceType, rawProperties)
        {
            DataType = dataType;
        }

        public IEnumerable<ReferenceSource> ConstructorDependencies
        {
            get
            {
                var args = DataType.Properties.Where(x => x.IsConstructorArgument);

                var mp = new List<string>();
                foreach (var arg in args)
                {
                    var p = Properties.FirstOrDefault(x => x.Facet == arg);

                    if (p is ReferenceProperty)
                    {
                        var rp = (ReferenceProperty)p;
                        if (rp.Source is NodeSource)
                        {
                            yield return rp.Source;
                        }
                        else if (rp.Source is UXPropertySource)
                        {
                            yield return rp.Source;
                        }
                        else if (rp.Source is UXPropertyAccessorSource)
                        {
                            // Do nothing
                        }
                        else if (rp.Source is BundleFileSource)
                        {
                            // Do nothing
                        }
                        else if (rp.Source == null)
                        {
                            // Do nothing
                        }
                        else throw new Exception("Unhandled source type: " + rp.Source);
                    }
                }
            }
        }
    }


    public sealed class ResourceRefNode : ObjectNode
    {
        public string StaticRefId { get; }

        internal ResourceRefNode(FileSourceInfo source, string staticRefId, string uxPropName, IDataType dataType, InstanceType type, IEnumerable<RawProperty> rawProperties = null)
            : base(source, uxPropName, dataType, type, rawProperties)
        {
            StaticRefId = staticRefId;
        }

        public override IDataType DeclaredType => ResultingType;
    }

    public sealed class NewObjectNode : ObjectNode
    {
        internal NewObjectNode(FileSourceInfo source, string name, IDataType dataType, InstanceType type, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, dataType, type, rawProperties)
        {
        }

        public override IDataType DeclaredType => ResultingType;
    }

    public sealed class BoxedValueNode : ObjectNode
    {
        public AtomicValue Value { get; }

        internal BoxedValueNode(FileSourceInfo source, string name, IDataType dataType, AtomicValue value, InstanceType type, IEnumerable<RawProperty> rawProperties = null)
            : base(source, name, dataType, type, rawProperties)
        {
            Value = value;
        }

        public override IDataType DeclaredType => ResultingType;
    }
}
